using UnityEngine;
using System.Collections;

public class AnimSpeedByCurve : MonoBehaviour
{
	public Animator anim;
	public string stateName;
	public AnimationCurve speedCurve;
	public string baseSpeedParam = "BeforeDashSpeed"; // Animator 參數名稱

	private void Start()
	{
		anim = GetComponent<Animator>();
	}
	public void PlayWithCurve()
	{
		//anim.Play(stateName, 0, 0f);
		StartCoroutine(SpeedControl());
		Debug.Log($"呼叫");
	}

	IEnumerator SpeedControl()
	{
		AnimatorStateInfo info = anim.GetCurrentAnimatorStateInfo(0);
		float baseSpeed = anim.GetFloat(baseSpeedParam); // 從 Animator 讀取

		while (info.IsName(stateName) && info.normalizedTime < 1f)
		{
			info = anim.GetCurrentAnimatorStateInfo(0);

			anim.SetFloat(baseSpeedParam, baseSpeed * speedCurve.Evaluate(info.normalizedTime));
			Debug.Log($"Normalized Time: {info.normalizedTime}, Speed: {anim.GetFloat(baseSpeedParam)}");
			yield return null;
		}

		anim.SetFloat(baseSpeedParam, baseSpeed); // 播完恢復
	}
}
