using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;

public class EnemyShootingController : MonoBehaviour
{
    [SerializeField]
    bool debug;
    public List<GunData> availableGuns;  // 在生成時就設定好


    [Header("射擊點")]
    GameObject shooter;
    [SerializeField]
    List<GameObject> shooterList;
    [SerializeField]
    GameObject spinShooter;
    [SerializeField]
    List<GameObject> spinShooterList;

    public bool useAllmode;

    // 內部快取
    private List<SubGunData> modes;
    [SerializeField]
    private int currentModeIndex = 0;
    public bool IsAttackLooping;
    public GunDataList gunDataList;

    private void Awake()
    {
        shooter = transform.GetChild(0).gameObject;
        spinShooter = transform.Find("4-waySpinGun").gameObject;
        for(int i=0; i < transform.childCount; i++)
        {
            shooterList.Add(transform.GetChild(i).gameObject);
        }
        for (int i = 0; i < spinShooter.transform.childCount; i++)
        {
            spinShooterList.Add(spinShooter.transform.GetChild(i).gameObject);
        }
        
    }
    private void Start()
    {
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
        IsAttackLooping = true;
        StartAttacking();
    }
    /// <summary>
    /// 開始攻擊
    /// </summary>
    /// <param name="useAllAtOnce">true=同時啟動所有模式；false=單模式輪流</param>
    public void StartAttacking()
    {
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
            var mode = modes[currentModeIndex];
            Debug.Log(currentModeIndex+mode.patternType);
            yield return StartCoroutine(HandleMode(mode));
            currentModeIndex = (currentModeIndex + 1) % modes.Count;
            yield return new WaitForSeconds(gunDataList.gunDatas[currentModeIndex].delayTime);
        }
    }
    public void StopAttacking()
    {
        IsAttackLooping = false;
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

    // 一次波次內，根據 bulletAmount & patternType 生成子彈
    private void FireOnce(SubGunData gunData)
    {
        switch (gunData.patternType)
        {
            case ShootingPatternType.Straight:
                //shooter = shooterList[0];
                ActiveBullet(shooter.transform.position, shooter.transform.rotation, gunData.bulletType);
                break;

            case ShootingPatternType.tracking:
                shooter = shooterList[1];
                ActiveBullet(shooter.transform.position, shooter.transform.rotation, gunData.bulletType);
                break;

            case ShootingPatternType.Spread:
                //shooter = shooterList[2];
                SpreadFire(gunData);
                break;

            case ShootingPatternType.Spiral:
                //shooter = shooterList[3];
                SpiralFire(gunData);
                break;
            case ShootingPatternType.shotgun:
                //shooter = shooterList[4];
                StartCoroutine(ShotGun(gunData));
                break;
            case ShootingPatternType.FourWay:
                foreach(var shooter in spinShooterList)
                {
                    ActiveBullet(shooter.transform.position, shooter.transform.rotation, gunData.bulletType);
                }
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
    private void SpreadFire(SubGunData g)
    {
        float half = (g.bulletAmount - 1) * 0.5f;
        for (int i = 0; i < g.bulletAmount; i++)
        {
            float angle = (i - half) * g.spinSpeed;
            var rot = shooter.transform.rotation * Quaternion.Euler(0, angle, 0);
            ActiveBullet(shooter.transform.position, rot, g.bulletType);
        }
    }

    // 螺旋射擊


    private void SpiralFire(SubGunData g)
    {
        // 用 Time.time 來持續變化角度，或者每次呼叫從外面帶進來一個累加器
        float angle = Time.time * g.spinSpeed;
        var rot = shooter.transform.rotation * Quaternion.Euler(0, angle, 0);
        ActiveBullet(shooter.transform.position, rot, g.bulletType);
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
            case BulletType.Purple:
                GameObject purple = BulletPool.poolInstance.GetBulletPoolInstance(type);
                if (purple == null)
                {
                    Debug.LogError("Purple bullet is null");
                    return;
                }
                purple.transform.position = pos;
                purple.transform.rotation = rot;
                purple.SetActive(true);
                //PurpleBulletMove purpleBulletMove = purple.GetComponent<PurpleBulletMove>();
                //purpleBulletMove.Initial();
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
