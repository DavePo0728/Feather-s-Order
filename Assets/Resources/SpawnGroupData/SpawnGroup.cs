using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
[CreateAssetMenu(fileName = "New SpawnGroupData", menuName = "SpawnGroupData/New SpawnGroupData")]
public class SpawnGroup : SpawnGroupController
{
	public SpawnDataList spawnDataList;
	public CustomPathDataList customPathDataList;
	public List<SpawnGroupData> spawnGroupDataList;
    public override IEnumerator GenerateGroup(WaveManager manager)
    {
        Debug.Log("SpawnGroup Start");
        foreach (var group in spawnGroupDataList)
        {
            var entry = manager.CreateEntryBehaviour(group.entryType);
            var move = manager.CreateMoveBehaviour(group.moveType);
            var leave = manager.CreateLeaveBehaviour(group.leaveType);
            manager.NewSpawn_WithRecord(group.enemyData, (int)group.spawnData, group.gunData, entry, move, leave, group.spawnType);
            //Debug.Log("原始直"+ group.spawnData + "修改直: = " +(int)group.spawnDataType);
            yield return new WaitForSeconds(group.delayTime);
        }
    }
}


[Serializable]
public class SpawnGroupData
{
	public int enemyData;
	public int spawnData;


	public GunDataList gunData;
	public EntryType entryType;
	public MoveType moveType;
	public LeaveType leaveType;
	public float delayTime;
	public WaveManager.SpawnType spawnType;
}
public enum EntryType
{
    EntryTypeA
}
public enum MoveType
{
    MoveTypeA, MoveTypeB, MoveTypeC, MoveTypeD, MoveTypeE,
}
public enum LeaveType
{
    LeaveTypeA
}
