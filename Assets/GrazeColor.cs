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

	private IEnumerator Transition(SkinnedMeshRenderer renderer, Color fromColor, Color toColor, float fromRim, float toRim, bool handleEmission = false, Color fromEmission = default, Color toEmission = default)
	{
		if (renderer == null)
			yield break;

		float timer = 0f;
		Material[] materials = renderer.materials;

		while (timer < transitionDuration)
		{
			float t = timer / transitionDuration;

			foreach (Material mat in materials)
			{
				if (mat.HasProperty("_BaseColor"))
					mat.SetColor("_BaseColor", Color.Lerp(fromColor, toColor, t));

				if (mat.HasProperty("_RimLightStrength"))
					mat.SetFloat("_RimLightStrength", Mathf.Lerp(fromRim, toRim, t));

				if (mat.HasProperty("_RimLightOn"))
					mat.SetFloat("_RimLightOn", 1f);

				if (handleEmission && mat.HasProperty("_EmissionColor"))
					mat.SetColor("_EmissionColor", Color.Lerp(fromEmission, toEmission, t));
			}

			timer += Time.deltaTime;
			yield return null;
		}

		foreach (Material mat in materials)
		{
			if (mat.HasProperty("_BaseColor"))
				mat.SetColor("_BaseColor", toColor);

			if (mat.HasProperty("_RimLightStrength"))
				mat.SetFloat("_RimLightStrength", toRim);

			if (mat.HasProperty("_RimLightOn"))
				mat.SetFloat("_RimLightOn", toRim > 0f ? 1f : 0f);

			if (handleEmission && mat.HasProperty("_EmissionColor"))
				mat.SetColor("_EmissionColor", toEmission);
		}
	}


	public void ApplyGrazeEffect()
	{
		StartCoroutine(GrazeAndResetRoutine());
	}

	private IEnumerator GrazeAndResetRoutine()
	{
		StartCoroutine(Transition(Body, besaColor, grazeColor, 0.4f, RimStrengthBody));
		StartCoroutine(Transition(Wing, besaColor, grazeColor, 0f, RimStrengthWing, true, Color.white, Color.black));
		StartCoroutine(Transition(Dress, besaColor, grazeColor, 0f, RimStrengthDress));
		StartCoroutine(Transition(Hair, besaColor, grazeColor, 0.4f, RimStrengthHair));
		StartCoroutine(Transition(Leg, besaColor, grazeColor, 0.4f, RimStrengthBody));
		yield return new WaitForSeconds(transitionDuration);

		ResetColor();
	}

	public void ResetColor()
	{
		StartCoroutine(Transition(Body, grazeColor, besaColor, RimStrengthBody, 0.4f));
		StartCoroutine(Transition(Wing, grazeColor, besaColor, RimStrengthWing, 0f, true, Color.black, Color.white));
		StartCoroutine(Transition(Dress, grazeColor, besaColor, RimStrengthDress, 0f));
		StartCoroutine(Transition(Hair, grazeColor, besaColor, RimStrengthHair, 0.4f));
		StartCoroutine(Transition(Leg, grazeColor, besaColor, RimStrengthBody, 0.4f));
	}
}


