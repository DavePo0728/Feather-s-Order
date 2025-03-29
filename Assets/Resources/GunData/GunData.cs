using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "New GunData", menuName = "GunData/New GunData")]
public class GunData : ScriptableObject
{
    public SubGunData data;
}
[System.Serializable]
public class SubGunData
{
    [Header("Gun Data")]
    public int gunIndex;
    public float rpm;
    public float shootingCoolDown;
    public float bulletAmount;
    public float spinSpeed;
    public BulletType bulletType;
    public float MaxShootWave;
}
