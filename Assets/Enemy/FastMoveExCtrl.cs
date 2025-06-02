using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]  // <-- 加這個，讓 Unity 可以序列化這個類別
public class FastMoveData
{
	public int ModifyNum;         // 一次控制幾個節點
	public float pointWaitTime;  // 這些節點的等待時間
	//
	public float EndpointWaitTime;//最後節點的停留時間
	public float moveTime;       // 每個節點移動所需的時間
	//
	public bool Invisibility;
}

public class FastMoveExCtrl : MonoBehaviour
{
	public float entryTime;

	//
	[SerializeField]
	public List<FastMoveData> data;  // <-- 現在這個會出現在 Inspector 中了
	//
	public float leaveTime;

	public bool Invisibility;

	public EnemyMove EnemyMove;

	public EnterExitSchedule enterExitSchedule;
	public GameObject SharpMissileBlue;
	private void Start()
	{
		EnemyMove.entryTime = entryTime;
		EnemyMove.leaveTime = leaveTime;
		StartCoroutine(MoveSequenceCoroutine());
	}
	private IEnumerator MoveSequenceCoroutine()
	{
		// 初始延遲（進場）
		yield return new WaitForSeconds(entryTime);

		for (int i = 0; i < data.Count; i++)
		{
			FastMoveData current = data[i];

			// Step 1: 基本設定
			EnemyMove.moveTime = current.moveTime;
			EnemyMove.pointWaitTime = current.pointWaitTime;

			//if (current.Invisibility)
			//	InvisibilityModeOn();

			// Step 2: 移動等待與 EndpointWaitTime 切換

				// 正常情況：多於1個點，先等前段時間
			float partialMoveTime = current.moveTime * (current.ModifyNum - 1);
			yield return new WaitForSeconds(partialMoveTime);
			EnemyMove.pointWaitTime = current.EndpointWaitTime;
			yield return new WaitForSeconds(current.moveTime * 0.5f);

			// Step 3: 切 Endpoint 停留時間


			// Step 4: 隱形轉回來
			//if (current.Invisibility)
			//{
			//	yield return new WaitForSeconds(current.moveTime * 0.5f);
			//	InvisibilityModeOff();
			//}

			// Step 5: 完整等待本段結束
			yield return new WaitForSeconds(current.moveTime * current.ModifyNum);
		}
	}

	//public void InvisibilityModeOn()
	//{
	//	enterExitSchedule.FadeOut();
	//	SharpMissileBlue.SetActive(true);
	//}
	//public void InvisibilityModeOff()
	//{
	//	enterExitSchedule.FadeIn();
	//	SharpMissileBlue.SetActive(false);
	//}
}
