using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum curveType
{
	Vertical ,
	Horizontal,
	VerticalAndHorizontal
}
[CreateAssetMenu(fileName = "New SpawnData", menuName = "SpawnData/New SpawnData")]
public class SpawnData : ScriptableObject
{
    public SpData data;
}
[System.Serializable]
public class SpData
{
    [Header("Spawn Data")]
    public Vector3 spawnPosition;
    public Vector3 endPosition;
    public Vector3 LeavePositon;
    public float curveHeight;
	public curveType curveType;
	[Header("typeA")]
    public float randomMoveRadius;
    [Header("typeB")]
    public int pathNum;
    [Header("typeC")]
    public int customPathNum;
    public float pointWaitTime;
}
