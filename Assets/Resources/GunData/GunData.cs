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
    public bool trackPlayer;
    [HideInInspector]
    public int gunIndex;
    public float rpm;
    public BulletType bulletType;
    public float MaxShootWave;
    //public float shootingCoolDown;
    public int bulletAmount;
    [Header("ShotGun&SpreadShot")]
    public float spreadAngle;
    public float shooterRotation;
    [Header("4Way")]
    public float spinSpeed;
    public float bulletsPerArm;
    public float crossSpacing;
	public float initialAngleOffset = 0f;
	[Header("ring")]
    public float ringRadius;
    [Header("FanSwing")]
    public float swingSpeed;
    public float swingAngle;
    [Header("Homing")]
    public GameObject missilePrefab;
    public float trackDuration;
    public float maxRotationSpeed;
    public float spreadDuration;
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
    FanSwing,
    All,       // 全彈
}
public enum BulletType
{
    Red,
    Black,
    Purple,
    BlackRed,
}
