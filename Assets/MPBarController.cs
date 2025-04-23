using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class MPBarController : MonoBehaviour
{
    [Header("血條元件")]
    public Image realFill;
    public Image fakeFill;

    [Header("虛血設定")]
    public float fakeDelay = 0.3f;
    public float fakeSpeed = 0.5f;

    private Coroutine fakeCoroutine;

    /// <summary>
    /// 立即更新真實 MP，不產生虛血動畫（如回復、擦彈）
    /// </summary>
    public void SetMP(float normalized)
    {
        realFill.fillAmount = normalized;
        fakeFill.fillAmount = Mathf.Max(fakeFill.fillAmount, normalized); // 保證虛血不超前
    }

    /// <summary>
    /// 產生虛血動畫，用於 MP 消耗
    /// </summary>
    public void ConsumeMP(float normalized)
    {
        realFill.fillAmount = normalized;

        if (fakeCoroutine != null)
            StopCoroutine(fakeCoroutine);
        fakeCoroutine = StartCoroutine(UpdateFakeMP(normalized));
    }

    IEnumerator UpdateFakeMP(float target)
    {
        yield return new WaitForSeconds(fakeDelay);

        while (fakeFill.fillAmount > target)
        {
            fakeFill.fillAmount = Mathf.MoveTowards(fakeFill.fillAmount, target, Time.deltaTime * fakeSpeed);
            yield return null;
        }
    }
}