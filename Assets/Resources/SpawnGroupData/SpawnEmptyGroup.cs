using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "New EmptyGroupData", menuName = "SpawnGroupData/New EmptyGroupData")]
public class SpawnEmptyGroup : SpawnGroupController
{
    public List<EmptyGroupGroupData> spawnGroupDataList;
    public override IEnumerator GenerateGroup(WaveManager manager)
    {
        Debug.Log("SpawnGroup Start");
        foreach (var group in spawnGroupDataList)
        {
            yield return new WaitForSeconds(group.delayTime);
        }
    }
}
[System.Serializable]
public class EmptyGroupGroupData
{
    public float delayTime;
}
