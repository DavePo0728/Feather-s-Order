using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;
using System.Drawing;
using System.Reflection;

public class EnemyShootingController : MonoBehaviour
{
    public bool debug;
    public List<GunData> availableGuns;  // 在生成時就設定好


    [Header("射擊點")]
    GameObject shooter;
    [SerializeField]
    List<GameObject> shooterList;
    [SerializeField]
    GameObject spinShooter;
    FourWayGunSpin fourWayGunSpin;
    [SerializeField]
    List<GameObject> spinShooterList;

    public bool useAllmode;

    // 內部快取
    private List<SubGunData> modes;
    int addtionalCount;
    [SerializeField]
    private int currentModeIndex = 0;
    public bool IsAttackLooping =false;
    public GunDataList gunDataList;

    private void Awake()
    {
        shooter = transform.GetChild(0).gameObject;
        spinShooter = transform.Find("4-waySpinGun").gameObject;
        fourWayGunSpin = spinShooter.GetComponent<FourWayGunSpin>();
        for (int i = 0; i < transform.childCount; i++)
        {
            shooterList.Add(transform.GetChild(i).gameObject);
        }
        for (int i = 0; i < spinShooter.transform.childCount; i++)
        {
            spinShooterList.Add(spinShooter.transform.GetChild(i).gameObject);
        }
        addtionalCount = 0;
        if (debug)
        {
            gunDataList = Resources.Load<GunDataList>("GunData/GunPattern/TestGunDataList");
            for (int i = 0; i < gunDataList.gunDatas.Length; i++)
            {
                availableGuns.Add(gunDataList.gunDatas[i].Data);
            }
            modes = new List<SubGunData>();
            foreach (var gd in availableGuns)
            {
                modes.Add(gd.data);
            }
        }
        else
        {
            if (gunDataList == null)
            {
                Debug.LogError("GunDataList is null");
                return;
            }
            for (int i = 0; i < gunDataList.gunDatas.Length; i++)
            {
                availableGuns.Add(gunDataList.gunDatas[i].Data);
            }
            // 一次取出所有 SubGunData
            modes = new List<SubGunData>();
            foreach (var gd in availableGuns)
            {
                modes.Add(gd.data);
            }
        }
    }
    private void Start()
    {

    }
    private void OnEnable()
    {
        StartAttacking();
        //Debug.Log("EnemyShootingController enabled, starting attack loop.");
    }
    private void OnDisable()
    {
        StopAttacking();
        //Debug.Log("EnemyShootingController disabled, stopping attack loop.");
    }
    public void FireExtraMode()
    {
        var extra = gunDataList.gunDatas[currentModeIndex].addtionalGunData[addtionalCount].additionalData.data;
        StartCoroutine(HandleMode(extra));
        //Debug.Log(addtionalCount);
        if(addtionalCount< gunDataList.gunDatas[currentModeIndex].addtionalGunData.Count-1)
        {
            addtionalCount++;
        }
        if(addtionalCount> gunDataList.gunDatas[currentModeIndex].addtionalGunData.Count)
        {
            addtionalCount = 0;
        }
    }
    /// <summary>
    /// 開始攻擊
    /// </summary>
    /// <param name="useAllAtOnce">true=同時啟動所有模式；false=單模式輪流</param>
    public void StartAttacking()
    {
        IsAttackLooping = true;
        StartCoroutine(AttackRotate());
    }
    IEnumerator TriggerAllMode()
    {
        var mode = modes[currentModeIndex];
        yield return StartCoroutine(AttackAllModes(mode));
    }
    // 同時所有模式各啟動一次 Coroutine（互不干擾）
    private IEnumerator AttackAllModes(SubGunData gunData)
    {
        for (int wave = 0; wave < gunData.MaxShootWave; wave++)
        {
            foreach (var mode in modes)
            {
                StartCoroutine(HandleMode(mode));
            }
        }
        yield break;
    }

    // 一種模式打完一波之後再換下一種，循環往復
    private IEnumerator AttackRotate()
    {
        while (IsAttackLooping)
        {
            if (gunDataList != null) 
            {
                if (gunDataList.gunDatas[currentModeIndex].IsAdditonalAttack)
                {
                    Invoke("FireExtraMode", gunDataList.gunDatas[currentModeIndex].addtionalGunData[addtionalCount].additionalDelayTime);
                }
                var mode = modes[currentModeIndex];
                //Debug.Log(mode.patternType);
                yield return StartCoroutine(HandleMode(mode));
                currentModeIndex = (currentModeIndex + 1) % modes.Count;
                yield return new WaitForSeconds(gunDataList.gunDatas[currentModeIndex].delayTime);
            }
            else
            {
                Debug.LogError("GunDataList is null, cannot start attack loop.");
                yield break;
            }
        }
    }

    public void StopAttacking()
    {
        //IsAttackLooping = false;
        StopCoroutine(AttackRotate());
    }
    // 根據 SubGunData 執行一個「波」的射擊
    private IEnumerator HandleMode(SubGunData gunData)
    {
        float interval = 60f / gunData.rpm;  // 每顆子彈間隔
        for (int wave = 0; wave < gunData.MaxShootWave; wave++)
        {
            FireOnce(gunData);
            yield return new WaitForSeconds(interval);
        }
    }
    // 散彈射擊
    private IEnumerator ShotGun(SubGunData g)
    {
        for (int i = 0; i < g.bulletAmount; i++)
        {
            // 隨機在 [-half, +half] 度範圍內抖動 Yaw（左右）與 Pitch（上下）
            float half = g.spreadAngle * 0.5f;
            float yaw = Random.Range(-half, half);
            float pitch = Random.Range(-half, half);

            // 把偏航與俯仰疊加到火點的朝向上
            Quaternion randomRot = shooter.transform.rotation * Quaternion.Euler(pitch, yaw, 0);
            ActiveBullet(shooter.transform.position, randomRot, g.bulletType);
        }
        yield return new WaitForSeconds(60f / g.rpm);
    }

    // 扇形射擊 
    private IEnumerator SpreadFire(SubGunData g) ////有問題
    {
        // 1. 計算張角的一半
        float half = g.spreadAngle * 0.5f;
        // 2. 計算每顆之間該間隔多少度 (N-1 段)
        float step = (g.bulletAmount > 1)
            ? (g.spreadAngle / (g.bulletAmount - 1))
            : 0f;
        shooter.transform.rotation = Quaternion.Euler(shooter.transform.eulerAngles.x, shooter.transform.eulerAngles.y, g.shooterRotation); // 設定射擊點的旋轉
        // 3. 一次把所有顆都生在同一幀
        for (int i = 0; i < g.bulletAmount; i++)
        {
            // 從 -half 開始，每顆向右加 step
            float yaw = -half + step * i;
            Quaternion rot = shooter.transform.rotation
                             * Quaternion.Euler(0f, yaw, 0f);

            ActiveBullet(shooter.transform.position, rot, g.bulletType);
        }

        // 4. 最後再等冷卻
        yield return new WaitForSeconds(60f / g.rpm);
    }
    // 螺旋射擊
    private void SpiralFire(SubGunData g)
    {
        int count = Mathf.Max(1, g.bulletAmount);

        for (int i = 0; i < count; i++)
        {
            // 均分 360°
            float angleDeg = i * (360f / count);
            float angleRad = angleDeg * Mathf.Deg2Rad;

            // 在本地 XY 平面上計算圓環位置 (X = cos, Y = sin, Z = 0)
            Vector3 localDir = new Vector3(
                Mathf.Cos(angleRad),
                Mathf.Sin(angleRad),
                0f
            );

            // 轉到世界座標並乘以半徑
            Vector3 worldOffset = shooter.transform.TransformDirection(localDir) * g.ringRadius;
            Vector3 spawnPos = shooter.transform.position + worldOffset;

            // 讓子彈朝向「圓心外側方向」飛出
            // up 向量選 Z 軸(環軸) 讓旋轉更直覺
            Quaternion spawnRot = Quaternion.LookRotation(
            Vector3.back,  // 子彈 forward → -Z
            Vector3.up       // up 定為全域 Y 軸
           );

            ActiveBullet(spawnPos, spawnRot, g.bulletType);
        }
    }
    private void FanSwing(SubGunData g)
    {
        // 1. 計算擺動角度：sin 會在 -1…+1 之間擺動
        float angle = Mathf.Sin(Time.time * g.swingSpeed * Mathf.PI * 2f)
                      * g.swingAngle;

        // 2. 把這個角度加到火點本身的旋轉上 (繞 X 軸做 Pitch)
        Quaternion rot = shooter.transform.rotation* Quaternion.Euler(angle, 0f, 0f);

        // 3. 一次生成一顆子彈（如果要一次多顆，就把下方這行放到 for 迴圈裡）
        ActiveBullet(shooter.transform.position, rot, g.bulletType);
    }
    public void CrossSpin(SubGunData g)
    {
        // 1. 算出當前旋轉角度
        float angle = Time.time * g.spinSpeed;
        Quaternion rotZ = Quaternion.AngleAxis(angle, Vector3.forward); // 繞 Z 軸

        // 2. 定義本地十字基準方向（右、上、左、下）
        Vector3[] baseDirs = new Vector3[] {
        Vector3.right,
        Vector3.up,
        Vector3.left,
        Vector3.down
    };

        // 3. 每條臂上依 bulletsPerArm 排列子彈
        for (int i = 0; i < baseDirs.Length; i++)
        {
            // 旋轉後的方向向量
            Vector3 dir = rotZ * baseDirs[i];

            for (int j = 1; j <= g.bulletsPerArm; j++)
            {
                // 排列位置 = 火點 + dir * spacing * j
                Vector3 spawnPos = shooter.transform.position + dir * (g.crossSpacing * j);

                // 子彈朝向 -Z
                Quaternion spawnRot = Quaternion.LookRotation(Vector3.back, Vector3.up);

                ActiveBullet(spawnPos, spawnRot, g.bulletType);
            }
        }
    }
    private void HomingMissile(SubGunData g)
    {
        for (int i = 0; i < g.bulletAmount; i++)
        {
            float angle = i * (360 / g.bulletAmount);
            if (g.bulletAmount == 1)
                angle = 90f; // 如果只有一顆子彈，則直接朝上發射
            float radian = (angle * Mathf.Deg2Rad);
            //print(radian);
            Vector3 direction = new Vector3(Mathf.Cos(radian), Mathf.Sin(radian), 0);
            ActiveMissile(g,shooter.transform.position, shooter.transform.rotation,direction);
        }
    }

    // 一次波次內，根據 bulletAmount & patternType 生成子彈
    private void FireOnce(SubGunData gunData)
    {
        switch (gunData.patternType)
        {
            case ShootingPatternType.Straight:
                shooter = shooterList[0];
                if (shooter != null)
                {
                    if (shooter.activeSelf == false)
                    {
                        shooter.SetActive(true);
                    }
                }
                ActiveBullet(shooter.transform.position, shooter.transform.rotation, gunData.bulletType);
                break;

            case ShootingPatternType.tracking:
                shooter = shooterList[1];
                if(shooter != null)
                {
                    if(shooter.activeSelf ==false)
                    {
                        shooter.SetActive(true);
                    }
                }
                ActiveBullet(shooter.transform.position, shooter.transform.rotation, gunData.bulletType);
                break;

            case ShootingPatternType.Spread:
                if (gunData.trackPlayer)
                {
                    shooter = shooterList[1];
                }
                else
                {
                    shooter = shooterList[0];
                }
                if (shooter != null)
                {
                    if (shooter.activeSelf == false)
                    {
                        shooter.SetActive(true);
                    }
                }
                StartCoroutine(SpreadFire(gunData));
                break;

            case ShootingPatternType.Spiral:
                if (gunData.trackPlayer)
                {
                    shooter = shooterList[1];
                }
                else
                {
                    shooter = shooterList[0];
                }
                if (shooter != null)
                {
                    if (shooter.activeSelf == false)
                    {
                        shooter.SetActive(true);
                    }
                }
                SpiralFire(gunData);
                break;
            case ShootingPatternType.shotgun:
                if (gunData.trackPlayer)
                {
                    shooter = shooterList[1];
                }
                else
                {
                    shooter = shooterList[0];
                }
                if (shooter != null)
                {
                    if (shooter.activeSelf == false)
                    {
                        shooter.SetActive(true);
                    }
                }
                StartCoroutine(ShotGun(gunData));
                break;
            case ShootingPatternType.FourWay:
                shooter = shooterList[0];
                if (shooter != null)
                {
                    if (shooter.activeSelf == false)
                    {
                        shooter.SetActive(true);
                    }
                }
                CrossSpin(gunData);
                break;
            case ShootingPatternType.FanSwing:
                if (gunData.trackPlayer)
                {
                    shooter = shooterList[1];
                }
                else
                {
                    shooter = shooterList[0];
                }
                if (shooter != null)
                {
                    if (shooter.activeSelf == false)
                    {
                        shooter.SetActive(true);
                    }
                }
                FanSwing(gunData);
                break;
            case ShootingPatternType.Homing:
                shooter = shooterList[0];
                if (shooter != null)
                {
                    if (shooter.activeSelf == false)
                    {
                        shooter.SetActive(true);
                    }
                }
                HomingMissile(gunData);
                break;
            case ShootingPatternType.All:
                StartCoroutine(TriggerAllMode());
                break;
            // 其他模式……
            default:
                // 預設當單發
                Debug.LogWarning("Unknown shooting pattern type: " + gunData.patternType);
                ActiveBullet(shooter.transform.position, shooter.transform.rotation, gunData.bulletType);
                break;
        }
    }
    public void ActiveMissile(SubGunData g,Vector3 pos, Quaternion rot,Vector3 direction)
    {
        Debug.Log("ActiveMissile");
        GameObject purple = Instantiate(g.missilePrefab, pos, rot);
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        HighSpeedVioletBulletMove purpleBulletMove = purple.GetComponent<HighSpeedVioletBulletMove>();
        purpleBulletMove.spreadDuration = g.spreadDuration;
        purpleBulletMove.trackDuration = g.trackDuration;
        purpleBulletMove.maxRotationSpeed = g.maxRotationSpeed;
        purpleBulletMove.Initialize(direction, player);
    }
    // 依照 BulletType 決定要 Instantiate 哪一個 Prefab
    private void ActiveBullet(Vector3 pos, Quaternion rot, BulletType type)
    {
        switch (type)
        {
            case BulletType.Black:
                GameObject black = BulletPool.poolInstance.GetBulletPoolInstance(type);
                if (black == null)
                {
                    Debug.LogError("Black bullet is null");
                    return;
                }
                black.transform.position = pos;
                black.transform.rotation = rot;
                black.SetActive(true);
                BlackBulletMove bulletMove = black.GetComponent<BlackBulletMove>();
                bulletMove.Initial();
                break;
            case BulletType.Red:
                GameObject red = BulletPool.poolInstance.GetBulletPoolInstance(type);
                if (red == null)
                {
                    Debug.LogError("Red bullet is null");
                    return;
                }
                red.transform.position = pos;
                red.transform.rotation = rot;
                red.SetActive(true);
                RedBulletMove redBulletMove = red.GetComponent<RedBulletMove>();
                redBulletMove.Initial();
                break;
            case BulletType.BlackRed:
                GameObject blackRed = BulletPool.poolInstance.GetBulletPoolInstance(type);
                if (blackRed == null)
                {
                    Debug.LogError("BlackRed bullet is null");
                    return;
                }
                blackRed.transform.position = pos;
                blackRed.transform.rotation = rot;
                blackRed.SetActive(true);
                //BlackRedBulletMove blackRedBulletMove = blackRed.GetComponent<BlackRedBulletMove>();
                //blackRedBulletMove.Initial();
                break;
            default:
                Debug.LogError("Unknown bullet type: " + type);
                break;
        }
    }
}
