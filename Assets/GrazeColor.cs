using System.Collections;
using UnityEngine;

public class GrazeColor : MonoBehaviour
{
	public Color besaColor = Color.white;
	public Color grazeColor = Color.white;

	private float RimStrengthBody = 1f;
	private float RimStrengthWing = 0.8f;
	private float RimStrengthDress = 1f;
	private float RimStrengthHair = 0.88f;

	[Header("角色部位 Renderer")]
	public SkinnedMeshRenderer Body;
	public SkinnedMeshRenderer Leg;
	public SkinnedMeshRenderer Wing;
	public SkinnedMeshRenderer Dress;
	public SkinnedMeshRenderer Hair;

	[Header("動畫時間（秒）")]
	public float transitionDuration = 1f;

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.G))
			ApplyGrazeEffect();
	}

	private IEnumerator Transition(SkinnedMeshRenderer renderer, Color fromColor, Color toColor, float fromRim, float toRim)
	{
		if (renderer == null)
		{
			yield break;
		}

		float timer = 0f;
		Material[] materials = renderer.materials;

		while (timer < transitionDuration)
		{
			float t = timer / transitionDuration;

			foreach (Material mat in materials)
			{
				if (mat.HasProperty("_BaseColor")) // 用於修改 Albedo 顏色
					mat.SetColor("_BaseColor", Color.Lerp(fromColor, toColor, t));

				if (mat.HasProperty("_RimLightStrength")) // 用於修改 Rim Strength
					mat.SetFloat("_RimLightStrength", Mathf.Lerp(fromRim, toRim, t));

				if (mat.HasProperty("_RimLightOn")) // 啟用 Rim Light
					mat.SetFloat("_RimLightOn", 1f);
			}

			timer += Time.deltaTime;
			yield return null;
		}

		// 最終值修正
		foreach (Material mat in materials)
		{
			if (mat.HasProperty("_BaseColor"))
				mat.SetColor("_BaseColor", toColor);

			if (mat.HasProperty("_RimLightStrength"))
				mat.SetFloat("_RimLightStrength", toRim);

			if (mat.HasProperty("_RimLightOn"))
				mat.SetFloat("_RimLightOn", toRim > 0f ? 1f : 0f);  // 若強度為 0 就關閉 Rim Light
		}
	}

	public void ApplyGrazeEffect()
	{
		StartCoroutine(GrazeAndResetRoutine());
	}

	private IEnumerator GrazeAndResetRoutine()
	{
		// 執行 graze 變化
		StartCoroutine(Transition(Body, besaColor, grazeColor, 0f, RimStrengthBody));
		StartCoroutine(Transition(Wing, besaColor, grazeColor, 0f, RimStrengthWing));
		StartCoroutine(Transition(Dress, besaColor, grazeColor, 0f, RimStrengthDress));
		StartCoroutine(Transition(Hair, besaColor, grazeColor, 0f, RimStrengthHair));
		StartCoroutine(Transition(Leg, besaColor, grazeColor, 0f, RimStrengthBody));
		// 等待變化完成
		yield return new WaitForSeconds(transitionDuration);

		// 執行還原
		ResetColor();
	}

	public void ResetColor()
	{
		StartCoroutine(Transition(Body, grazeColor, besaColor, RimStrengthBody, 0f));
		StartCoroutine(Transition(Wing, grazeColor, besaColor, RimStrengthWing, 0f));
		StartCoroutine(Transition(Dress, grazeColor, besaColor, RimStrengthDress, 0f));
		StartCoroutine(Transition(Hair, grazeColor, besaColor, RimStrengthHair, 0f));
		StartCoroutine(Transition(Leg, grazeColor, besaColor, RimStrengthBody, 0f));
	}
}


