using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// 守門員：萬一有地方仍把焦點設到了不可互動按鈕，
// 這支會在選取時立刻把焦點彈到下一個有效鄰居（依 Navigation）。
public class SkipNonInteractableOnSelect : MonoBehaviour, ISelectHandler
{
    public enum Axis { Vertical, Horizontal }
    public Axis axis = Axis.Vertical;

    public void OnSelect(BaseEventData eventData)
    {
        var sel = GetComponent<Selectable>();
        if (sel == null) return;
        if (!sel.IsActive() || !sel.IsInteractable())
        {
            StartCoroutine(DeferSkip(sel));
        }
    }

    IEnumerator DeferSkip(Selectable s)
    {
        yield return null; // 等一幀避免與原事件衝突
        var nav = s.navigation;
        // 先試「下一個」方向
        Selectable candidate = (axis == Axis.Vertical) ? nav.selectOnDown : nav.selectOnRight;
        // 循環尋找可用者，最多嘗試 8 次避免死循環
        for (int i = 0; i < 8 && candidate != null; i++)
        {
            if (candidate.IsActive() && candidate.IsInteractable()) break;
            var n2 = candidate.navigation;
            candidate = (axis == Axis.Vertical) ? n2.selectOnDown : n2.selectOnRight;
        }
        if (candidate != null && candidate.IsActive() && candidate.IsInteractable())
        {
            EventSystem.current?.SetSelectedGameObject(candidate.gameObject);
        }
        else
        {
            // 沒找到，乾脆清空焦點
            EventSystem.current?.SetSelectedGameObject(null);
        }
    }
}