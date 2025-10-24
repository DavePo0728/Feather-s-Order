using UnityEngine;

[RequireComponent(typeof(Animator))]
public class AnimationDayNightController : MonoBehaviour
{
	[Range(0f, 24f)]
	public float hour = 0f;   // 0 ~ 24 小時制

	public string stateName = "DayNightCycle"; // 你的動畫狀態名稱
	private Animator animator;
	private int stateHash;

	void Start()
	{
		animator = GetComponent<Animator>();
		animator.speed = 0f; // 停止自動播放
		stateHash = Animator.StringToHash(stateName);
	}

	void Update()
	{
		// 把 0~24 小時轉成 0~1 的 normalizedTime
		float progress = Mathf.Clamp01(hour / 24f);

		animator.Play(stateHash, 0, progress);
	}
}
