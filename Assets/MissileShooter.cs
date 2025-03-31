using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MissileShooter : MonoBehaviour
{
    [SerializeField]
    float shootCost;
    bool canShoot;
    [SerializeField]
    BulletGraze bulletGraze;
    [SerializeField]
    Collider missileCoillder;
    [SerializeField]
    Camera playerVcam;
    [SerializeField]
    GameObject missile;
    bool startCharge;
    float chargeTimer;
    [SerializeField]
    GameObject lockImage, canvas;
    [SerializeField]
    float maxChargeTime;
    [SerializeField]
    GameObject chargeAimCollider;
    [SerializeField]
    float maxScaleX, maxScaleY;
    Tweener scaleX, scaleY;
    bool startScaleX, startScaleY;
    [SerializeField]
    List<GameObject> lockedEnemies = new List<GameObject>();
    [SerializeField]
    List<GameObject> lockEnemyImageList = new List<GameObject>();
    // Start is called before the first frame update
    private void Awake()
    {
        startScaleX = false; 
        startScaleY =false;
        missileCoillder = GetComponent<Collider>();
    }
    void Start()
    {
        scaleX = chargeAimCollider.transform.DOScaleX(maxScaleX, maxChargeTime).OnStart(() => startScaleX = true).OnRewind(() => startScaleX = false).OnComplete(()=> startScaleX =false).SetEase(Ease.Linear).SetAutoKill(false);
        scaleY = chargeAimCollider.transform.DOScaleY(maxScaleY, maxChargeTime).OnStart(() => startScaleY = true).OnRewind(() => startScaleY = false).OnComplete(() => startScaleX = false).SetEase(Ease.Linear).SetAutoKill(false);
    }
    public void GetMissileShootInput(InputAction.CallbackContext context)
    {

        if (context.performed)
        {
            if (bulletGraze.CheckGrazeEnergy(shootCost))
            {
                startCharge = true;
                missileCoillder.enabled = true;
                canShoot = true;
            }
        }
        if (context.canceled)
        {
            if (canShoot)
            {
                
                startCharge = false;
                missileCoillder.enabled = false;
                if (lockedEnemies.Count > 0)
                {
                    bulletGraze.UpdateGrazeEnergyOutside(shootCost);
                    ShootMissile(lockedEnemies.Count);
                }
                lockedEnemies.Clear();
                foreach (var image in lockEnemyImageList)
                {
                    Destroy(image);
                }
                lockEnemyImageList.Clear();
                canShoot = false;
            }
        }
    }
    // Update is called once per frame
    void FixedUpdate()
    {
        if (startCharge)
        {
            chargeTimer += Time.deltaTime;
            if (startScaleX == false)
            {
                scaleX.Play();
                //Debug.Log("playX");
            }

            if (startScaleY == false)
            {
                scaleY.Play();
            }
            //Debug.Log(chargeTimer);
        }
        else
        {
            chargeTimer = 0;
            if (scaleX != null)
            {
                //Debug.Log("CallXPaused");
                scaleX.Rewind();
                //chargeAimCollider.transform.localScale = Vector3.one;
            }

            if (scaleY != null)
            {
                scaleY.Rewind();
            }
        }
    }
    void ShootMissile(float missileAmount)
    {

        for (int i = 0; i < missileAmount; i++) {
            float angle = i * (360 / missileAmount);
            float radian = angle * Mathf.Deg2Rad;
            Vector3 direction = new Vector3(Mathf.Cos(radian), Mathf.Sin(radian), 0);
            GameObject temp = Instantiate(missile, transform.position, Quaternion.identity);
            PlayerMissileMove playerMissileMove = temp.GetComponent<PlayerMissileMove>();
            playerMissileMove.Initialize(direction, lockedEnemies[i]);
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            if (!lockedEnemies.Contains(other.gameObject))
            {
                Vector3 lockPos = playerVcam.WorldToScreenPoint(other.gameObject.transform.position);
                lockedEnemies.Add(other.gameObject);
                GameObject temp = Instantiate(lockImage, lockPos,Quaternion.identity,canvas.transform);
                LockImageUpdate lockImageUpdate = temp.GetComponent<LockImageUpdate>();
                lockImageUpdate.SetTarget(other.gameObject);
                lockEnemyImageList.Add(temp);
            }
        }
    }
}
