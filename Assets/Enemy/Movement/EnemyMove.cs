using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PathCreation;
using System;

public class EnemyMove : MonoBehaviour
{
    IEntryBehaviour entryBehavior;
    IMoveBehaviour moveBehavior;
    ILeaveBehaviour leaveBehavior;
    //[SerializeField]
    public float moveSpeed;
    [SerializeField]
    float lifeTime;
    protected float angle;
    public Vector3 startPoint;
    public Vector3 endPoint;
    public Vector3 leavePoint;
    public float curveHeight;
    public float enterTime;
    public float stayTime;
    public float pointWaitTime;
    public Vector3 originPos;
    public bool isLeave = false;
    //[HideInInspector]
    public Vector3[] path;
    public Vector3[] moveB_PathList;
    public Vector3[] moveC_PathList;
    [SerializeField]
    bool isDebug;
    public bool isMove =false;

    public List<GameObject> gunList;
    GameObject activeGun, activeGun1;

    //public bool canShoot = false;
    public enum BulletType
    {
        Black,
        Red,
        Purple,
        BlackRed,
    }
    public BulletType bulletType;
    //[SerializeField]
    //protected float rotationSpeed;
    //[SerializeField]
    //protected float radius;

    public void SetBehaviours(IEntryBehaviour entry,IMoveBehaviour move,ILeaveBehaviour leave)
    {
        entryBehavior = entry;
        moveBehavior = move;
        leaveBehavior = leave;
    }
    private void Awake()
    {
        gunList = new List<GameObject>();
        for (int i = 0; i < transform.GetChild(3).childCount; i++)
        {
            gunList.Add(transform.GetChild(3).GetChild(i).gameObject);
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        
        //Debug.Log(gunList.Count);
        startPoint = gameObject.transform.position;
        StartCoroutine(TimeToLeave());
        //UpdateDebugLine();
        
        entryBehavior.Enter(this);
        
    }
    void FixedUpdate()
    {
        //if (!isDebug&&!isEnter)
        //{
        //    isEnter = true;
        //    entryBehavior.Enter(this);
        //}

        //UpdateDebugLine();
        //print(isLeave);
    }
    public void CallMove()
    {
        if(activeGun != null)
            activeGun.SetActive(true);
        originPos = transform.position;
        moveBehavior.Move(this);
        isMove = true;
    }
    public void ActiveGun(int gunIndex, float rpm, float bulletAmount, float spinSpeed,float shootingCoolDown,BulletType bulletType,float MaxShootWave)
    {
        if (gunIndex < 0 || gunIndex >= gunList.Count)
        {
            Debug.LogError($"Invalid gunIndex: {gunIndex}. It must be between 0 and {gunList.Count - 1}.");
            return;
        }
        
        switch (gunIndex)
        {
            
            case 0: //strightShooting
                SimpleShoot simpleShoot = gunList[gunIndex].GetComponent<SimpleShoot>();
                simpleShoot.rpm = rpm;
                simpleShoot.maxShots = bulletAmount;
                simpleShoot.shootingCoolDown = shootingCoolDown;
                simpleShoot.bulletType = (SimpleShoot.BulletType)bulletType;
                simpleShoot.maxShootWave = MaxShootWave;

                break;
            case 1: //trackShooting
                TrackShooting track = gunList[gunIndex].GetComponent<TrackShooting>();
                track.rpm = rpm;
                track.maxShots = bulletAmount;
                track.shootingCoolDown = shootingCoolDown;
                track.bulletType = (TrackShooting.BulletType)bulletType;
                track.maxShootWave = MaxShootWave;
                break;
            case 2: //shotGun
                ShootShotGun shootShotGun = gunList[gunIndex].GetComponent<ShootShotGun>();
                shootShotGun.bulletAmount = (int)bulletAmount;
                shootShotGun.shootingCoolDown = shootingCoolDown;
                shootShotGun.bulletType = (ShootShotGun.BulletType)bulletType;
                break;
            case 3: //SpreadShot
                SpreadShot spreadShot = gunList[gunIndex].GetComponent<SpreadShot>();
                spreadShot.bulletAmount = (int)bulletAmount;
                spreadShot.shootingCoolDown = shootingCoolDown;
                spreadShot.bulletType = (SpreadShot.BulletType)bulletType;
                spreadShot.MaxShootWave = MaxShootWave;
                break;
            case 4: //HomingShooter
                HomingShooter homingShooter = gunList[gunIndex].GetComponent<HomingShooter>();
                homingShooter.bulletAmount = (int)bulletAmount;
                homingShooter.shootingCoolDown = shootingCoolDown;
                homingShooter.bulletType = (HomingShooter.BulletType)bulletType;
                homingShooter.MaxShootWave = MaxShootWave;
                break;
            case 5: //FourWayGunSpin
                FourWayGunSpin fourWayGunSpin = gunList[gunIndex].GetComponent<FourWayGunSpin>();
                fourWayGunSpin.speed = spinSpeed;
                for (int i = 0; i < gunList[gunIndex].transform.childCount; i++)
                {
                    simpleShoot = gunList[gunIndex].transform.GetChild(i).GetComponent<SimpleShoot>();
                    simpleShoot.rpm = rpm;
                    simpleShoot.maxShots = bulletAmount;
                    simpleShoot.shootingCoolDown = shootingCoolDown;
                    simpleShoot.bulletType = (SimpleShoot.BulletType)bulletType;
                }
                break;
        }
        activeGun = gunList[gunIndex];
    }
    IEnumerator TimeToLeave()
    {
        yield return new WaitForSeconds(lifeTime);
        isLeave = true;
        foreach (GameObject gun in gunList)
        {
            if(gun.activeSelf)
            {
               gun.SetActive(false);
            }
        }
    }
    public void CallLeave()
    {
        leaveBehavior.Leave(this);
        foreach (GameObject gun in gunList)
        {
            if (gun.activeSelf)
            {
                gun.SetActive(false);
            }
        }
    }
    public void DestroyNow()
    {
        Destroy(gameObject);
    }
    private void OnDestroy()
    {
        if(moveBehavior != null)
            moveBehavior.StopMove();
        if (leaveBehavior != null)
            leaveBehavior.StopLeave();
        if (entryBehavior != null)
            entryBehavior.StopEnter();
    }
    void UpdateDebugLine()
    {
        
        CurvePathGenerator.pathInstance.DrawDebugLine(this.gameObject);
    }
    void OnDrawGizmos()
    {
        if (isMove)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(originPos, 10);
            //Debug.Log("Draw");
        }
    }
}
