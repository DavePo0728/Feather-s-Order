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
    public ShootingPatternType patternType;
    [Header("Gun Data")]
    public bool seperateMode;
    [HideInInspector]
    public int gunIndex;
    public float rpm;
    //public float shootingCoolDown;
    public float bulletAmount;
    [Header("ShotGun")]
    public float spreadAngle;
    public float spinSpeed;
    public BulletType bulletType;
    public float MaxShootWave;
}
public enum ShootingPatternType
{
    Straight,  // 直線
    tracking,  // 鎖定
    shotgun,   // 散彈
    Spread,    // 散射
    Spiral,    // 螺旋
    FourWay,   // 四向
    Homing,    // 追蹤
    All,       // 全彈
}
public enum BulletType
{
    Black,
    Red,
    Purple,
    BlackRed,
}
