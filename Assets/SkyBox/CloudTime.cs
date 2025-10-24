using UnityEngine;
using System.Collections.Generic;

public class CloudTime : MonoBehaviour
{
	[Range(0f, 24f)]
	public float hour = 0f;

	[Header("雲動畫控制")]
	public string cloudStateName = "DayNightCycle";
	public List<MeshRenderer> cloudRenderers = new List<MeshRenderer>();
	private int stateHash;

	[Header("雲顏色控制 (四個顏色分開)")]
	public Gradient cloudColorAGradient;
	public Gradient cloudColorBGradient;
	public Gradient cloudColorCGradient;
	public Gradient cloudColorDGradient;
	public Gradient edgeColorGradient;

	[Header("雲特效控制 (單一 float)")]
	public Gradient sunMoonGradient;     // 控制 _SunMoon alpha
	public Gradient cloudSDFGradient;    // 控制 _Cloud_SDF_TSb alpha

	[Header("太陽動畫控制")]
	public string sunStateName = "SunDir";
	public Animator SunAnimators;
	private int sunStateHash;

	[Header("天空材質 (ProceduralSky)")]
	public Material skybox;

	[Header("Skybox 顏色控制")]
	public Gradient topColorGradient;
	public Gradient horizonColorGradient;
	public Gradient bottomColorGradient;

	[Header("霧顏色控制")]
	public Gradient fogGradient;

	[Header("星星透明度控制")]
	public Gradient starAlphaGradient;

	private MaterialPropertyBlock mpb;

	void Start()
	{
		stateHash = Animator.StringToHash(cloudStateName);
		sunStateHash = Animator.StringToHash(sunStateName);

		mpb = new MaterialPropertyBlock();
	}

	void Update()
	{
		float progress = Mathf.Clamp01(hour / 24f);

		// 太陽動畫
		if (SunAnimators != null)
		{
			SunAnimators.Play(sunStateHash, 0, progress);
		}

		// 霧顏色
		if (fogGradient != null)
		{
			RenderSettings.fogColor = fogGradient.Evaluate(progress);
		}

		// 天空盒顏色與星星
		if (skybox != null)
		{
			skybox.SetColor("_TopColor", topColorGradient.Evaluate(progress));
			skybox.SetColor("_HorizonColor", horizonColorGradient.Evaluate(progress));
			skybox.SetColor("_BottomColor", bottomColorGradient.Evaluate(progress));

			if (starAlphaGradient != null)
			{
				float starAlpha = starAlphaGradient.Evaluate(progress).a;
				skybox.SetFloat("_StarAlpha", starAlpha);
			}
		}

		// 雲顏色與單一 float 屬性
		Color colorA = cloudColorAGradient.Evaluate(progress);
		Color colorB = cloudColorBGradient.Evaluate(progress);
		Color colorC = cloudColorCGradient.Evaluate(progress);
		Color colorD = cloudColorDGradient.Evaluate(progress);
		Color edgeColor = edgeColorGradient.Evaluate(progress);
		float sunMoonAlpha = sunMoonGradient.Evaluate(progress).a;
		float cloudSDFAlpha = cloudSDFGradient.Evaluate(progress).a;

		foreach (var rend in cloudRenderers)
		{
			rend.GetPropertyBlock(mpb);

			mpb.SetColor("_CloudColorA", colorA);
			mpb.SetColor("_CloudColorB", colorB);
			mpb.SetColor("_CloudColorC", colorC);
			mpb.SetColor("_CloudColorD", colorD);
			mpb.SetColor("_Cloud_edgeColor", edgeColor);

			mpb.SetFloat("_SunMoon", sunMoonAlpha);
			mpb.SetFloat("_Cloud_SDF_TSb", cloudSDFAlpha);

			rend.SetPropertyBlock(mpb);
		}
	}
}
