using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "New GunDataList", menuName = "GunData/New GunDataList")]
public class GunDataList : ScriptableObject
{
    public GunListWithDelay[] gunDatas;
}
[System.Serializable]
public class GunListWithDelay
{
    public GunData Data;
    public float delayTime;
    public bool IsAdditonalAttack;
    public GunData additionalData;
    public float additionalDelayTime;
}
