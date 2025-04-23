using UnityEngine;

public class WorldToCanvas : MonoBehaviour
{
    public Camera mainCamera;          // 主相機
    public Canvas canvas;              // 目標 Canvas
    public RectTransform uiElement;    // 要移動的 UI 元素
    public Transform target;           // 世界空間中的目標

    void Update()
    {
        if (mainCamera == null || canvas == null || uiElement == null || target == null)
            return;

        // 將世界座標轉為螢幕座標
        Vector3 screenPos = mainCamera.WorldToScreenPoint(target.position);

        // 將螢幕座標轉為 Canvas 上的 UI 座標
        Vector2 uiPos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
        canvas.transform as RectTransform,
        screenPos,
        canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : mainCamera,
        out uiPos);

        // 更新 UI 元素的位置
        uiElement.localPosition = uiPos;
    }
}