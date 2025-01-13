using UnityEngine;
using System.Collections;

public class TriggerWaving : MonoBehaviour
{
	public Animator animator; // 关联Animator组件
	public string triggerName = "waving"; // 触发器的名称
	public float interval = 4f; // 触发间隔
	public GameObject TailEffectL;
	public GameObject TailEffectR;
	public Animation AniTail;
	public DynamicBone DBTailL;
	public DynamicBone DBTailR;
	public Coroutine TriggernCoroutine;
	private void Start()
	{
		// 开始协程调用触发方法
		TriggernCoroutine = StartCoroutine(TriggerAnimationCoroutine());
	}
	private IEnumerator TriggerAnimationCoroutine()
	{
		while (true) // 无限循环，类似于 InvokeRepeating
		{
			yield return new WaitForSeconds(interval); // 等待 interval 秒

			if (animator != null && !string.IsNullOrEmpty(triggerName))
			{
				animator.SetTrigger(triggerName);
				TailEffectL.SetActive(false);
				TailEffectR.SetActive(false);
				DBTailL.enabled = false;
				DBTailR.enabled = false;
				print("off");
			}
		}
	}
	private void OnDestroy()
	{

		// 确保在对象销毁时停止重复调用
		TriggernCoroutine = null;
	}
	public void TailEffectSetActive()
	{
		TailEffectL.SetActive(true);
		TailEffectR.SetActive(true);
		AniTail.Play();
		print("on");
	}

}
