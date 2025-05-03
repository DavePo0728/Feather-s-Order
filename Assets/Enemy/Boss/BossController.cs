using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class BossController : MonoBehaviour
{

	//[Header("一般控制滑條")]
	[Range(0f, 1f)] public float trigger1 = 0;
	[Range(0f, 1f)] public float trigger2 = 0;
	[Range(0f, 1f)] public float trigger3 = 0;
	[Range(0f, 1f)] public float trigger4 = 0;
	[Range(0f, 1f)] public float specialTrigger = 0;
	[Range(0f, 1f)] public float newTrigger = 0;
	[Range(0f, 1f)] public float outTrigger = 0;

	//[Header("觸手葉子滑條 (四個方向)")]
	[Range(0f, 1f)] public float tentacleLeftUptrigger = 0;
	[Range(0f, 1f)] public float tentacleRightUptrigger = 0;
	[Range(0f, 1f)] public float tentacleLeftDowntrigger = 0;
	[Range(0f, 1f)] public float tentacleRightDowntrigger = 0;

	//[Header("觸手花瓣滑條 (四個方向)")]
	[Range(0f, 1f)] public float tentacleflowerLeftUptrigger = 0;
	[Range(0f, 1f)] public float tentacleflowerRightUptrigger = 0;
	[Range(0f, 1f)] public float tentacleflowerLeftDowntrigger = 0;
	[Range(0f, 1f)] public float tentacleflowerRightDowntrigger = 0;

	//[Header("一般眼睛控制群組")]
	public List<Transform> targetGroup1 = new List<Transform>();
	public List<Transform> targetGroup2 = new List<Transform>();
	public List<Transform> targetGroup3 = new List<Transform>();
	public List<Transform> targetGroup4 = new List<Transform>();

	//[Header("中央眼睛群組")]
	public List<Transform> downList = new List<Transform>();
	public Transform target;

	//[Header("眼睛花瓣旋轉控制")]
	public List<Transform> rotationGroup1 = new List<Transform>();
	public List<Transform> rotationGroup2 = new List<Transform>();
	public List<Transform> rotationGroup3 = new List<Transform>();

	public float maxRotation1 = 90f;
	public float maxRotation2 = 45f;
	public float maxRotation3 = 60f;

	//[Header("眼睛朝向設定")]
	public Transform Look_Target;
	public List<Transform> EyesGroup = new List<Transform>();

	//[Header("觸手葉片群組 (每個方向5個List)")]
	public List<Transform> tentacleLeftUpGroup1 = new List<Transform>();
	public List<Transform> tentacleLeftUpGroup2 = new List<Transform>();
	public List<Transform> tentacleLeftUpGroup3 = new List<Transform>();
	public List<Transform> tentacleLeftUpGroup4 = new List<Transform>();
	public List<Transform> tentacleLeftUpGroup5 = new List<Transform>();

	public List<Transform> tentacleRightUpGroup1 = new List<Transform>();
	public List<Transform> tentacleRightUpGroup2 = new List<Transform>();
	public List<Transform> tentacleRightUpGroup3 = new List<Transform>();
	public List<Transform> tentacleRightUpGroup4 = new List<Transform>();
	public List<Transform> tentacleRightUpGroup5 = new List<Transform>();

	public List<Transform> tentacleLeftDownGroup1 = new List<Transform>();
	public List<Transform> tentacleLeftDownGroup2 = new List<Transform>();
	public List<Transform> tentacleLeftDownGroup3 = new List<Transform>();
	public List<Transform> tentacleLeftDownGroup4 = new List<Transform>();
	public List<Transform> tentacleLeftDownGroup5 = new List<Transform>();

	public List<Transform> tentacleRightDownGroup1 = new List<Transform>();
	public List<Transform> tentacleRightDownGroup2 = new List<Transform>();
	public List<Transform> tentacleRightDownGroup3 = new List<Transform>();
	public List<Transform> tentacleRightDownGroup4 = new List<Transform>();
	public List<Transform> tentacleRightDownGroup5 = new List<Transform>();

	//[Header("觸手葉片最大旋轉 (每個5個最大值)")]
	public float maxLeafRotation1 = 90f;
	public float maxLeafRotation2 = 80f;
	public float maxLeafRotation3 = 70f;
	public float maxLeafRotation4 = 60f;
	public float maxLeafRotation5 = 50f;

	//[Header("觸手花瓣群組 (每個方向3個List)")]
	public List<Transform> tentacleflowerLeftUpGroup1 = new List<Transform>();
	public List<Transform> tentacleflowerLeftUpGroup2 = new List<Transform>();
	public List<Transform> tentacleflowerLeftUpGroup3 = new List<Transform>();

	public List<Transform> tentacleflowerRightUpGroup1 = new List<Transform>();
	public List<Transform> tentacleflowerRightUpGroup2 = new List<Transform>();
	public List<Transform> tentacleflowerRightUpGroup3 = new List<Transform>();

	public List<Transform> tentacleflowerLeftDownGroup1 = new List<Transform>();
	public List<Transform> tentacleflowerLeftDownGroup2 = new List<Transform>();
	public List<Transform> tentacleflowerLeftDownGroup3 = new List<Transform>();

	public List<Transform> tentacleflowerRightDownGroup1 = new List<Transform>();
	public List<Transform> tentacleflowerRightDownGroup2 = new List<Transform>();
	public List<Transform> tentacleflowerRightDownGroup3 = new List<Transform>();

	//[Header("觸手花瓣最大旋轉 (每個3個最大值)")]
	public float maxFlowerRotation1 = 90f;
	public float maxFlowerRotation2 = 60f;
	public float maxFlowerRotation3 = 30f;
	private Dictionary<Transform, Quaternion> originalRotations = new Dictionary<Transform, Quaternion>();

	public MultiAimConstraint UpRightAim;
	public MultiAimConstraint UpLeftAim;
	public MultiAimConstraint DownLeftAim;
	public MultiAimConstraint DownRightAim;


	public Transform Tentacle_ShootingUpRight;
	public Transform Tentacle_ShootingUpLeft;
	public Transform Tentacle_ShootingDownRight;
	public Transform Tentacle_ShootingDownLeft;

	//[Range(-30f, 30f)] public float TentacleRotaRightUpX = 0f;  // 控制右上方向的 X 偏移
	//[Range(-30f, 30f)] public float TentacleRotaRightUpY = 0f;  // 控制右上方向的 Y 偏移

	//[Range(-30f, 30f)] public float TentacleRotaLeftDownX = 0f; // 控制左下方向的 X 偏移
	//[Range(-30f, 30f)] public float TentacleRotaLeftDownY = 0f; // 控制左下方向的 Y 偏移

	//[Range(-30f, 30f)] public float TentacleRotaLeftUpX = 0f;   // 控制左上方向的 X 偏移
	//[Range(-30f, 30f)] public float TentacleRotaLeftUpY = 0f;   // 控制左上方向的 Y 偏移

	//[Range(-30f, 30f)] public float TentacleRotaRightDownX = 0f; // 控制右下方向的 X 偏移
	//[Range(-30f, 30f)] public float TentacleRotaRightDownY = 0f; // 控制右下方向的 Y 偏移

	[Range(0f, 1f)] public float EyesTrigger1 = 0;  // 跟隨強度
	[Range(0f, 1f)] public float EyesTrigger2 = 0;  // 跟隨強度
	[Range(0f, 1f)] public float EyesTrigger3 = 0;  // 跟隨強度
	[Range(0f, 1f)] public float EyesTrigger4 = 0;  // 跟隨強度

	public Transform MainSight1;
	public Transform MainSight2;
	public Transform MainSight3;
	public Transform MainSight4;

	public float movementSpeed = 2f; // 移動速度
	public float maxMoveDistance = 10f; // 最大移動範圍，設定為10f

	private Transform[] sights;
	private Vector3[] originalPositions;
	private void Start()
	{
		//StartCoroutine(LoopTestModesRightUp());
		// 記錄所有物件的初始旋轉
		CacheOriginalRotations();

		//sights = new Transform[] { MainSight1, MainSight2, MainSight3, MainSight4 };
		//originalPositions = new Vector3[sights.Length];

		//// 記錄每個物件的原始位置
		//for (int i = 0; i < sights.Length; i++)
		//{
		//	originalPositions[i] = sights[i].position;
		//}

		//// 開始隨機移動
		//foreach (var sight in sights)
		//{
		//	StartCoroutine(MoveRandomly(sight, System.Array.IndexOf(sights, sight)));
		//}
	}

	private void CacheOriginalRotations()
	{
		// 把所有 group 的物件都存起來
		List<List<Transform>> allGroups = new List<List<Transform>>
		{
            // 觸手葉片
            tentacleLeftUpGroup1, tentacleLeftUpGroup2, tentacleLeftUpGroup3, tentacleLeftUpGroup4, tentacleLeftUpGroup5,
			tentacleRightUpGroup1, tentacleRightUpGroup2, tentacleRightUpGroup3, tentacleRightUpGroup4, tentacleRightUpGroup5,
			tentacleLeftDownGroup1, tentacleLeftDownGroup2, tentacleLeftDownGroup3, tentacleLeftDownGroup4, tentacleLeftDownGroup5,
			tentacleRightDownGroup1, tentacleRightDownGroup2, tentacleRightDownGroup3, tentacleRightDownGroup4, tentacleRightDownGroup5,

            // 觸手花瓣
            tentacleflowerLeftUpGroup1, tentacleflowerLeftUpGroup2, tentacleflowerLeftUpGroup3,
			tentacleflowerRightUpGroup1, tentacleflowerRightUpGroup2, tentacleflowerRightUpGroup3,
			tentacleflowerLeftDownGroup1, tentacleflowerLeftDownGroup2, tentacleflowerLeftDownGroup3,
			tentacleflowerRightDownGroup1, tentacleflowerRightDownGroup2, tentacleflowerRightDownGroup3
		};

		foreach (var group in allGroups)
		{
			foreach (var t in group)
			{
				if (t != null && !originalRotations.ContainsKey(t))
				{
					originalRotations.Add(t, t.localRotation);
				}
			}
		}
	}

	private void Update()
	{
		UpdateGroup(targetGroup1, trigger1);
		UpdateGroup(targetGroup2, trigger2);
		UpdateGroup(targetGroup3, trigger3);
		UpdateGroup(targetGroup4, trigger4);

		UpdateDown();
		UpdateTarget();
		//UpdateEyesGroupPosition();
		//UpdateTentacleLeaf();
		//UpdateTentacleFlower();
		UpdateRotation(rotationGroup1, outTrigger, maxRotation1, Vector3.forward);
		UpdateRotation(rotationGroup2, outTrigger, maxRotation2, Vector3.forward);
		UpdateRotation(rotationGroup3, outTrigger, maxRotation3, Vector3.forward);

		// 左上
		UpdateRotation(tentacleflowerLeftUpGroup1, tentacleflowerLeftUptrigger, maxFlowerRotation1, Vector3.forward);
		UpdateRotation(tentacleflowerLeftUpGroup2, tentacleflowerLeftUptrigger, maxFlowerRotation2, Vector3.forward);
		UpdateRotation(tentacleflowerLeftUpGroup3, tentacleflowerLeftUptrigger, maxFlowerRotation3, Vector3.forward);

		UpdateRotation(tentacleLeftUpGroup1, tentacleLeftUptrigger, maxLeafRotation1, Vector3.forward);
		UpdateRotation(tentacleLeftUpGroup2, tentacleLeftUptrigger, maxLeafRotation2, Vector3.forward);
		UpdateRotation(tentacleLeftUpGroup3, tentacleLeftUptrigger, maxLeafRotation3, Vector3.forward);
		UpdateRotation(tentacleLeftUpGroup4, tentacleLeftUptrigger, maxLeafRotation4, Vector3.forward);
		UpdateRotation(tentacleLeftUpGroup5, tentacleLeftUptrigger, maxLeafRotation5, Vector3.forward);

		// 右上
		UpdateRotation(tentacleflowerRightUpGroup1, tentacleflowerRightUptrigger, maxFlowerRotation1, Vector3.forward);
		UpdateRotation(tentacleflowerRightUpGroup2, tentacleflowerRightUptrigger, maxFlowerRotation2, Vector3.forward);
		UpdateRotation(tentacleflowerRightUpGroup3, tentacleflowerRightUptrigger, maxFlowerRotation3, Vector3.forward);

		UpdateRotation(tentacleRightUpGroup1, tentacleRightUptrigger, maxLeafRotation1, Vector3.forward);
		UpdateRotation(tentacleRightUpGroup2, tentacleRightUptrigger, maxLeafRotation2, Vector3.forward);
		UpdateRotation(tentacleRightUpGroup3, tentacleRightUptrigger, maxLeafRotation3, Vector3.forward);
		UpdateRotation(tentacleRightUpGroup4, tentacleRightUptrigger, maxLeafRotation4, Vector3.forward);
		UpdateRotation(tentacleRightUpGroup5, tentacleRightUptrigger, maxLeafRotation5, Vector3.forward);

		// 左下
		UpdateRotation(tentacleflowerLeftDownGroup1, tentacleflowerLeftDowntrigger, maxFlowerRotation1, Vector3.forward);
		UpdateRotation(tentacleflowerLeftDownGroup2, tentacleflowerLeftDowntrigger, maxFlowerRotation2, Vector3.forward);
		UpdateRotation(tentacleflowerLeftDownGroup3, tentacleflowerLeftDowntrigger, maxFlowerRotation3, Vector3.forward);

		UpdateRotation(tentacleLeftDownGroup1, tentacleLeftDowntrigger, maxLeafRotation1, Vector3.forward);
		UpdateRotation(tentacleLeftDownGroup2, tentacleLeftDowntrigger, maxLeafRotation2, Vector3.forward);
		UpdateRotation(tentacleLeftDownGroup3, tentacleLeftDowntrigger, maxLeafRotation3, Vector3.forward);
		UpdateRotation(tentacleLeftDownGroup4, tentacleLeftDowntrigger, maxLeafRotation4, Vector3.forward);
		UpdateRotation(tentacleLeftDownGroup5, tentacleLeftDowntrigger, maxLeafRotation5, Vector3.forward);

		// 右下
		UpdateRotation(tentacleflowerRightDownGroup1, tentacleflowerRightDowntrigger, maxFlowerRotation1, Vector3.forward);
		UpdateRotation(tentacleflowerRightDownGroup2, tentacleflowerRightDowntrigger, maxFlowerRotation2, Vector3.forward);
		UpdateRotation(tentacleflowerRightDownGroup3, tentacleflowerRightDowntrigger, maxFlowerRotation3, Vector3.forward);

		UpdateRotation(tentacleRightDownGroup1, tentacleRightDowntrigger, maxLeafRotation1, Vector3.forward);
		UpdateRotation(tentacleRightDownGroup2, tentacleRightDowntrigger, maxLeafRotation2, Vector3.forward);
		UpdateRotation(tentacleRightDownGroup3, tentacleRightDowntrigger, maxLeafRotation3, Vector3.forward);
		UpdateRotation(tentacleRightDownGroup4, tentacleRightDowntrigger, maxLeafRotation4, Vector3.forward);
		UpdateRotation(tentacleRightDownGroup5, tentacleRightDowntrigger, maxLeafRotation5, Vector3.forward);

		//UpdateMultiAimConstraint();


	}

	private void UpdateTentacleLeaf()
	{
		RotateGroup(tentacleLeftUptrigger, tentacleLeftUpGroup1, maxLeafRotation1);
		RotateGroup(tentacleLeftUptrigger, tentacleLeftUpGroup2, maxLeafRotation2);
		RotateGroup(tentacleLeftUptrigger, tentacleLeftUpGroup3, maxLeafRotation3);
		RotateGroup(tentacleLeftUptrigger, tentacleLeftUpGroup4, maxLeafRotation4);
		RotateGroup(tentacleLeftUptrigger, tentacleLeftUpGroup5, maxLeafRotation5);

		RotateGroup(tentacleRightUptrigger, tentacleRightUpGroup1, maxLeafRotation1);
		RotateGroup(tentacleRightUptrigger, tentacleRightUpGroup2, maxLeafRotation2);
		RotateGroup(tentacleRightUptrigger, tentacleRightUpGroup3, maxLeafRotation3);
		RotateGroup(tentacleRightUptrigger, tentacleRightUpGroup4, maxLeafRotation4);
		RotateGroup(tentacleRightUptrigger, tentacleRightUpGroup5, maxLeafRotation5);

		RotateGroup(tentacleLeftDowntrigger, tentacleLeftDownGroup1, maxLeafRotation1);
		RotateGroup(tentacleLeftDowntrigger, tentacleLeftDownGroup2, maxLeafRotation2);
		RotateGroup(tentacleLeftDowntrigger, tentacleLeftDownGroup3, maxLeafRotation3);
		RotateGroup(tentacleLeftDowntrigger, tentacleLeftDownGroup4, maxLeafRotation4);
		RotateGroup(tentacleLeftDowntrigger, tentacleLeftDownGroup5, maxLeafRotation5);

		RotateGroup(tentacleRightDowntrigger, tentacleRightDownGroup1, maxLeafRotation1);
		RotateGroup(tentacleRightDowntrigger, tentacleRightDownGroup2, maxLeafRotation2);
		RotateGroup(tentacleRightDowntrigger, tentacleRightDownGroup3, maxLeafRotation3);
		RotateGroup(tentacleRightDowntrigger, tentacleRightDownGroup4, maxLeafRotation4);
		RotateGroup(tentacleRightDowntrigger, tentacleRightDownGroup5, maxLeafRotation5);
	}
	//void UpdateMultiAimConstraint()
	//{
	//	// 將 X 和 Y 偏移應用到 MultiAimConstraint 中，注意這裡的假設是 Y 對應 Z 軸
	//	UpRightAim.data.offset = new Vector3(-TentacleRotaRightUpX, UpRightAim.data.offset.y, TentacleRotaRightUpY);
	//	DownLeftAim.data.offset = new Vector3(-TentacleRotaLeftDownX, DownLeftAim.data.offset.y, TentacleRotaLeftDownY);
	//	UpLeftAim.data.offset = new Vector3(-TentacleRotaLeftUpX, UpLeftAim.data.offset.y, TentacleRotaLeftUpY);
	//	DownRightAim.data.offset = new Vector3(-TentacleRotaRightDownX, DownRightAim.data.offset.y, TentacleRotaRightDownY);
	//}

	private void UpdateTentacleFlower()
	{
		RotateGroup(tentacleflowerLeftUptrigger, tentacleflowerLeftUpGroup1, maxFlowerRotation1);
		RotateGroup(tentacleflowerLeftUptrigger, tentacleflowerLeftUpGroup2, maxFlowerRotation2);
		RotateGroup(tentacleflowerLeftUptrigger, tentacleflowerLeftUpGroup3, maxFlowerRotation3);

		RotateGroup(tentacleflowerRightUptrigger, tentacleflowerRightUpGroup1, maxFlowerRotation1);
		RotateGroup(tentacleflowerRightUptrigger, tentacleflowerRightUpGroup2, maxFlowerRotation2);
		RotateGroup(tentacleflowerRightUptrigger, tentacleflowerRightUpGroup3, maxFlowerRotation3);

		RotateGroup(tentacleflowerLeftDowntrigger, tentacleflowerLeftDownGroup1, maxFlowerRotation1);
		RotateGroup(tentacleflowerLeftDowntrigger, tentacleflowerLeftDownGroup2, maxFlowerRotation2);
		RotateGroup(tentacleflowerLeftDowntrigger, tentacleflowerLeftDownGroup3, maxFlowerRotation3);

		RotateGroup(tentacleflowerRightDowntrigger, tentacleflowerRightDownGroup1, maxFlowerRotation1);
		RotateGroup(tentacleflowerRightDowntrigger, tentacleflowerRightDownGroup2, maxFlowerRotation2);
		RotateGroup(tentacleflowerRightDowntrigger, tentacleflowerRightDownGroup3, maxFlowerRotation3);
	}
	void UpdateGroup(List<Transform> targets, float trigger)
	{
		float scaleValue = 1f - trigger;

		foreach (Transform t in targets)
		{
			t.localScale = new Vector3(scaleValue, scaleValue, scaleValue);
		}
	}
	void UpdateDown()
	{
		float downScale = Mathf.Lerp(1f, 0.05f, specialTrigger);

		foreach (Transform t in downList)
		{
			t.localScale = new Vector3(downScale, downScale, downScale);
		}
	}
	void UpdateTarget()
	{
		if (target != null)
		{
			float targetScale = 1f - newTrigger;
			target.localScale = new Vector3(targetScale, targetScale, targetScale);
		}
	}
	private void RotateGroup(float trigger, List<Transform> group, float maxRotation)
	{
		foreach (Transform t in group)
		{
			if (t != null && originalRotations.ContainsKey(t))
			{
				Quaternion baseRotation = originalRotations[t];
				Quaternion addRotation = Quaternion.Euler(maxRotation * trigger, 0f, 0f);
				t.localRotation = baseRotation * addRotation;
			}
		}
	}
	void UpdateRotation(List<Transform> targets, float trigger, float maxRotation, Vector3 axis)
	{
		foreach (Transform t in targets)
		{
			if (t != null && originalRotations.ContainsKey(t))
			{
				Quaternion original = originalRotations[t];
				float addRotation = Mathf.Lerp(0f, maxRotation, trigger);

				// 按照指定軸向產生旋轉
				Quaternion deltaRotation = Quaternion.Euler(axis.normalized * addRotation);

				// 套用原本的旋轉 + 累加旋轉
				t.localRotation = original * deltaRotation;
			}
		}
	}
	//void UpdateEyesGroupPosition()
	//{
	//	// 控制 MainSight1
	//	UpdateMainSight(MainSight1, EyesTrigger1);

	//	// 控制 MainSight2
	//	UpdateMainSight(MainSight2, EyesTrigger2);

	//	// 控制 MainSight3
	//	UpdateMainSight(MainSight3, EyesTrigger3);

	//	// 控制 MainSight4
	//	UpdateMainSight(MainSight4, EyesTrigger4);
	//}

	//void UpdateMainSight(Transform mainSight, float eyesTrigger)
	//{
	//	if (mainSight != null && Look_Target != null)
	//	{
	//		Vector3 targetPosition = Look_Target.position;

	//		根據 EyesTrigger 的值來決定跟隨的強度
	//		if (eyesTrigger == 0)
	//		{
	//			完全不跟隨
	//			mainSight.position = mainSight.position;
	//		}
	//		else if (eyesTrigger == 1)
	//		{
	//			直接跟隨目標位置
	//			mainSight.position = targetPosition;
	//		}
	//		else
	//		{
	//			慣性跟隨，使用插值來平滑過渡
	//			使用 Time.deltaTime 來確保跟隨的速度固定
	//			float speed = eyesTrigger; // 使用 EyesTrigger 來控制跟隨的速度
	//			mainSight.position = Vector3.Lerp(mainSight.position, targetPosition, speed * Time.deltaTime);
	//		}
	//	}
	//}
	private IEnumerator MoveRandomly(Transform sight, int index)
	{
		while (true)
		{
			// 隨機生成一個目標位置，並限制最大距離
			Vector3 targetPosition = GetRandomPosition(index);

			// 移動到目標位置
			while (Vector3.Distance(sight.position, targetPosition) > 0.1f)
			{
				sight.position = Vector3.MoveTowards(sight.position, targetPosition, movementSpeed * Time.deltaTime);
				yield return null;
			}

			// 等待一段時間後回到原點
			yield return new WaitForSeconds(1f);

			// 慢慢回到原本的位置
			while (Vector3.Distance(sight.position, originalPositions[index]) > 0.1f)
			{
				sight.position = Vector3.MoveTowards(sight.position, originalPositions[index], movementSpeed * Time.deltaTime);
				yield return null;
			}

			// 等待一段時間後再次隨機移動
			yield return new WaitForSeconds(1f);
		}
	}

	private Vector3 GetRandomPosition(int index)
	{
		// 隨機生成X和Y值，並確保它們在範圍內
		float randomX = Random.Range(-maxMoveDistance, maxMoveDistance);
		float randomY = Random.Range(-maxMoveDistance, maxMoveDistance);

		// 保證移動範圍不會超過原始位置的10f範圍
		Vector3 randomPosition = originalPositions[index] + new Vector3(randomX, randomY, 0);
		return ClampPosition(randomPosition, index);
	}

	private Vector3 ClampPosition(Vector3 targetPosition, int index)
	{
		// 限制目標位置在範圍內，確保距離原點不超過最大範圍
		Vector3 directionToTarget = targetPosition - originalPositions[index];
		if (directionToTarget.magnitude > maxMoveDistance)
		{
			directionToTarget = directionToTarget.normalized * maxMoveDistance;
		}

		return originalPositions[index] + directionToTarget;
	}
}



