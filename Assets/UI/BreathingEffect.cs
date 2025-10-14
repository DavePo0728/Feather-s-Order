using UnityEngine;
using UnityEngine.UI;

public class BreathingEffect : MonoBehaviour
{
	public Image targetImage;      // 要做呼吸效果的圖片
	public Image textImage;        // 相反效果的圖片
	public float minAlpha = 0.3f;  // 最暗透明度
	public float maxAlpha = 1.0f;  // 最亮透明度
	public float speed = 1.5f;     // 呼吸速度
	bool isFadingOut = false;

    void Update()
	{
        if (isFadingOut) return;
        float t = (Mathf.Sin(Time.time * speed) + 1f) / 2f; // 在 0~1 間來回
		float alpha = Mathf.Lerp(minAlpha, maxAlpha, t);    // targetImage 的透明度
		float invertedAlpha = maxAlpha + minAlpha - alpha;  // textImage 的反向透明度

		if (targetImage != null)
		{
			Color color = targetImage.color;
			color.a = alpha;
			targetImage.color = color;
		}

		if (textImage != null)
		{
			Color color = textImage.color;
			color.a = invertedAlpha;
			textImage.color = color;
		}
	}
	public void fadeOut()
	{
        isFadingOut = true;
        if (targetImage != null)
		{
			Color color = targetImage.color;
			color.a = 0;
			targetImage.color = color;
		}
		if (textImage != null)
		{
			Color color = textImage.color;
			color.a = 0;
			textImage.color = color;
		}
	}
}
