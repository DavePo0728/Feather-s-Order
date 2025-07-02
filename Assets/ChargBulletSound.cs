using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChargBulletSound : MonoBehaviour
{
	private bool hasHit = false;
	private List<Collider> enemyHits = new List<Collider>();
	private void OnTriggerEnter(Collider other)
	{
		if (hasHit) return;

		if (other.CompareTag("Enemy"))
		{
			Debug.Log("Hit enemy: " + other.name);
			enemyHits.Add(other);

			hasHit = true; // 先暫時阻擋重複觸發

			// 延遲一點點時間再處理
			CancelInvoke(nameof(HandleHit));
			enemyHits[0].GetComponent<EnemyHp>().PlayhitXAudio();
		}
	}

	private void HandleHit()
	{
		if (enemyHits.Count == 0)
		{
			hasHit = false;
			return;
		}

		// 依照距離排序敵人
		enemyHits.Sort((a, b) => {
			float distA = Vector3.Distance(transform.position, a.transform.position);
			float distB = Vector3.Distance(transform.position, b.transform.position);
			return distA.CompareTo(distB);
		});

		// 範例處理：印出最近的敵人
		Collider nearestEnemy = enemyHits[0];
		Debug.Log("最近的敵人是: " + nearestEnemy.name);

		// 清空列表，避免殘留資料影響下一次
		enemyHits.Clear();
		hasHit = false; // 重置旗標，允許下一次偵測
	}
}
