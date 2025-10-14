using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class AutoScrollOnSelect : MonoBehaviour
{
    [Header("References")]
    public ScrollRect scrollRect;
    [Tooltip("選到第一/最後一個項目時自動對齊頂/底")]
    public bool snapOnBoundary = true;
    [Header("Behavior")]
    [Tooltip("可見邊緣內縮像素 (padding)")]
    public float padding = 6f;
    [Tooltip("勾選後改為把選取項目置中；不勾則只要確保看得到")]
    public bool centerOnSelect = false;
    [Tooltip("當尺寸/版面計算失敗時，用索引近似捲動（等高項目很準）")]
    public bool useIndexFallback = true;

    private GameObject _lastSelected;

    private void Reset()
    {
        if (!scrollRect) scrollRect = GetComponentInChildren<ScrollRect>();
    }

    private void Update()
    {
        var es = EventSystem.current;
        if (es == null || scrollRect == null) return;

        var selected = es.currentSelectedGameObject;
        if (selected == null || selected == _lastSelected) return;

        // 只處理屬於這個 ScrollRect.content 裡面的選取物件
        if (selected.transform.IsChildOf(scrollRect.content))
        {
            var target = selected.GetComponent<RectTransform>();
            if (!target) return;
            Canvas.ForceUpdateCanvases();
            LayoutRebuilder.ForceRebuildLayoutImmediate(scrollRect.content);
            if (snapOnBoundary)
            {
                var item = GetDirectChildUnderContent(target);
                var content = scrollRect.content;
                int index = item.GetSiblingIndex();
                int last = content.childCount - 1;

                scrollRect.StopMovement();
                scrollRect.velocity = Vector2.zero;

                if (scrollRect.vertical)
                {
                    if (index == 0)
                    {
                        var pos = content.anchoredPosition;
                        pos.y = 0f;
                        content.anchoredPosition = pos;
                        scrollRect.verticalNormalizedPosition = 1f; // 同步內部狀態
                        _lastSelected = selected;
                        return; // 已完成本幀處理
                    }
                    else if (index == last)
                    {
                        // 直下：y = contentHeight - viewportHeight
                        float maxY = Mathf.Max(0, content.rect.height - (scrollRect.viewport ? scrollRect.viewport.rect.height : ((RectTransform)scrollRect.transform).rect.height));
                        var pos = content.anchoredPosition;
                        pos.y = maxY;
                        content.anchoredPosition = pos;
                        scrollRect.verticalNormalizedPosition = 0f;
                    }
                }
            }
            if (!TryEnsureVisible(target) && useIndexFallback)
                ScrollToByIndex(target); // 等高項目的近似法
            _lastSelected = selected;
        }

    }

    // 讓目標保持在 Viewport 可視範圍（必要時才移動 content）
    private bool TryEnsureVisible(RectTransform target)
    {
        Canvas.ForceUpdateCanvases();
        LayoutRebuilder.ForceRebuildLayoutImmediate(scrollRect.content);
        var viewport = scrollRect.viewport != null ? scrollRect.viewport : (RectTransform)scrollRect.transform;
        var content = scrollRect.content;

        // 以 viewport 為座標系，計算目標與視窗的 bounds
        var viewBounds = new Bounds(viewport.rect.center, viewport.rect.size);
        var targetBounds = RectTransformUtility.CalculateRelativeRectTransformBounds(viewport, target);

        Vector2 offset = Vector2.zero;

        if (scrollRect.vertical)
        {
            if (centerOnSelect)
            {
                offset.y = targetBounds.center.y - viewBounds.center.y;
            }
            else
            {
                if (targetBounds.max.y > viewBounds.max.y - padding)
                    offset.y = targetBounds.max.y - (viewBounds.max.y - padding);
                else if (targetBounds.min.y < viewBounds.min.y + padding)
                    offset.y = targetBounds.min.y - (viewBounds.min.y + padding);
            }
        }
        if (scrollRect.horizontal)
        {
            if (centerOnSelect)
            {
                offset.x = targetBounds.center.x - viewBounds.center.x;
            }
            else
            {
                if (targetBounds.min.x < viewBounds.min.x + padding)
                    offset.x = targetBounds.min.x - (viewBounds.min.x + padding);
                else if (targetBounds.max.x > viewBounds.max.x - padding)
                    offset.x = targetBounds.max.x - (viewBounds.max.x - padding);
            }
        }

        if (offset == Vector2.zero) return true; // 已在可視範圍內

        // 把「在 viewport 座標系的偏移量」轉成「content.anchoredPosition 的位移」
        var deltaInContentSpace = (Vector2)content.InverseTransformVector(viewport.TransformVector(offset));
        var newPos = content.anchoredPosition - deltaInContentSpace; // 方向相反：移 content 才能把目標推進視窗

        // 夾住避免超出
        if (scrollRect.vertical)
        {
            float maxY = Mathf.Max(0, content.rect.height - viewport.rect.height);
            newPos.y = Mathf.Clamp(newPos.y, 0, maxY);
        }
        if (scrollRect.horizontal)
        {
            float maxX = Mathf.Max(0, content.rect.width - viewport.rect.width);
            newPos.x = Mathf.Clamp(newPos.x, 0, maxX);
        }

        content.anchoredPosition = newPos;
        return true;
    }
    private RectTransform GetDirectChildUnderContent(RectTransform target)
    {
        var content = scrollRect.content;
        Transform t = target.transform;
        while (t != null && t.parent != content && t != content)
            t = t.parent;
        return (t as RectTransform) ?? target;
    }
    // 等高列表的簡易近似法：用索引換算 normalizedPosition
    private void ScrollToByIndex(RectTransform target)
    {
        var content = scrollRect.content;
        int index = target.GetSiblingIndex();
        int count = content.childCount;

        if (scrollRect.vertical)
        {
            float t = (count <= 1) ? 1f : 1f - (index / Mathf.Max(1f, (count - 1f)));
            scrollRect.verticalNormalizedPosition = Mathf.Clamp01(t);
        }
        if (scrollRect.horizontal)
        {
            float t = (count <= 1) ? 0f : (index / Mathf.Max(1f, (count - 1f)));
            scrollRect.horizontalNormalizedPosition = Mathf.Clamp01(t);
        }
    }
}
