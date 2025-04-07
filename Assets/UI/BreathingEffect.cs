using UnityEngine;
using UnityEngine.UI;

public class BreathingEffect : MonoBehaviour
{
    public Image targetImage;      // 要做呼吸效果的圖片
    public float minAlpha = 0.3f;  // 最暗的透明度
    public float maxAlpha = 1.0f;  // 最亮的透明度
    public float speed = 1.5f;     // 呼吸速度

    void Update()
    {
        if (targetImage != null)
        {
            float alpha = Mathf.Lerp(minAlpha, maxAlpha, (Mathf.Sin(Time.time * speed) + 1f) / 2f);
            Color color = targetImage.color;
            color.a = alpha;
            targetImage.color = color;
        }
    }
}
