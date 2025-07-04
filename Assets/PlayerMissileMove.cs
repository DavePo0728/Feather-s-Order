using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class PlayerMissileMove : MonoBehaviour
{
	[SerializeField]
	float MaxSpeed;
	[SerializeField]
	float speed;
	[SerializeField]
	float lifeTime;
	GameObject lockedEnemy;
	public float spreadDuration;
	private bool isTracking = false;
	[SerializeField]
	float maxRotationSpeed;
	public int corruptionDamage;
	public int BulletCount = 0;// 記錄子彈數量
	public void Initialize(Vector3 direction, GameObject enemy)
	{
		speed = 3f;
		// 設定初始發射方向
		transform.rotation = Quaternion.LookRotation(direction);
		lockedEnemy = enemy;
		StartCoroutine(CountDownInactive());
		// 啟動發射流程
		StartCoroutine(StartTrackingAfterDelay());
	}

	void FixedUpdate()
	{
		// 持續向前移動
		transform.Translate(Vector3.forward * speed);

		// 延遲後開始追蹤敵人
		if (isTracking && lockedEnemy != null)
		{
			if (speed < MaxSpeed)
			{
				speed += 0.1f;
			}
			//Debug.Log("Missile Speed: " + speed);
			Vector3 targetDirection = (lockedEnemy.transform.position - transform.position).normalized;
			Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
			transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, maxRotationSpeed * Time.deltaTime);
		}
	}

	private IEnumerator StartTrackingAfterDelay()
	{
		yield return new WaitForSeconds(spreadDuration);
		isTracking = true; // 啟動追蹤

	}
	private void OnTriggerEnter(Collider other)
	{
		if (other.CompareTag("Enemy"))
		{

			float result = 1f / BulletCount;
			float rounded = (float)Math.Round(result, 3);  // 四捨五入到小數點後3位
			rounded = Mathf.Max(rounded, 0.3f);
			Debug.Log("Hit enemy: " + rounded + "BulletCount:	" + BulletCount);
			other.GetComponent<EnemyHp>().PlayhitMisairuAudio(rounded);

			Destroy(this.gameObject);
		}
	}
	IEnumerator CountDownInactive()
	{
		yield return new WaitForSeconds(lifeTime);
		Destroy(this.gameObject);
	}
}
