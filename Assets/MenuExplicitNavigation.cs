
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MenuExplicitNavigation : MonoBehaviour
{
    public enum Orientation { Vertical, Horizontal }

    [Header("Buttons (in order, will be循環)")]
    public List<Selectable> order = new List<Selectable>(); // 依 UI 排列順序填入（例如：Continue, New, Settings, Quit）

    [Header("Options")]
    public Orientation orientation = Orientation.Vertical;
    public bool applyOnEnable = true;

    void OnEnable()
    {
        if (applyOnEnable) Apply();
    }

    public void Apply()
    {
        // 清洗清單（null 剔除）
        order = order.Where(s => s != null).ToList();
        if (order.Count == 0) return;

        // 建立「有效」名單（可互動且啟用）
        bool IsValid(Selectable s) => s != null && s.IsActive() && s.IsInteractable();
        var valid = order.Where(IsValid).ToList();

        // 幫每顆按鈕設置 Navigation：有效者串成循環；無效者不讓別人指到它
        for (int i = 0; i < order.Count; i++)
        {
            var cur = order[i];
            var nav = cur.navigation; nav.mode = Navigation.Mode.Explicit;

            // 尋找上一顆/下一顆有效按鈕（循環）
            Selectable PrevValid()
            {
                for (int k = 1; k <= order.Count; k++)
                {
                    var idx = (i - k + order.Count) % order.Count;
                    if (IsValid(order[idx])) return order[idx];
                }
                return null;
            }
            Selectable NextValid()
            {
                for (int k = 1; k <= order.Count; k++)
                {
                    var idx = (i + k) % order.Count;
                    if (IsValid(order[idx])) return order[idx];
                }
                return null;
            }

            var prev = PrevValid();
            var next = NextValid();

            if (orientation == Orientation.Vertical)
            {
                nav.selectOnUp = prev;
                nav.selectOnDown = next;
                // 左/右也接同樣，避免水平移動跑偏
                nav.selectOnLeft = prev;
                nav.selectOnRight = next;
            }
            else
            {
                nav.selectOnLeft = prev;
                nav.selectOnRight = next;
                nav.selectOnUp = prev;   // 視需要也可設為 null
                nav.selectOnDown = next; // 視需要也可設為 null
            }

            //// 若自己「無效」，避免任何人能指到它：把其他人導覽時已跳過它；
            //// 自己的 nav 設成 null，不影響（只是防禦性，核心在於鄰居不指向它）。
            //if (!IsValid(cur))
            //{
            //    nav.selectOnUp = null; nav.selectOnDown = null; nav.selectOnLeft = null; nav.selectOnRight = null;
            //}
            cur.navigation = nav;
        }
    }

    // 範例：在檢查到有無存檔後，呼叫這個方法切換 Continue 狀態並重建導覽
    public void SetSelectableInteractable(Selectable s, bool interactable)
    {
        if (s == null) return;
        s.interactable = interactable;
        Apply();
        // 若當前焦點停在一顆變成無效的按鈕上，移到下一個有效者
        var curSel = EventSystem.current?.currentSelectedGameObject;
        if (curSel != null)
        {
            var sel = curSel.GetComponent<Selectable>();
            if (sel != null && (!sel.IsActive() || !sel.IsInteractable()))
            {
                var next = order.FirstOrDefault(o => o != null && o.IsActive() && o.IsInteractable());
                if (next != null) EventSystem.current.SetSelectedGameObject(next.gameObject);
                else EventSystem.current.SetSelectedGameObject(null);
            }
        }
    }
}
