using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class PlayerAim : MonoBehaviour
{
    //Vector3 playerOriPos;
    [SerializeField]
    Camera playerCamera;
    [SerializeField]
    public GameObject aimmingImage, FarLockImage, NearLockImage;
    public GameObject lockedEnemy;
    public GameObject emptyAimObject;
    NearLockTweenAnim nearLockAnim;
    FarLockTweenAnim farLockAnim;
    [SerializeField]
    public float zOffset;
    EnemyHp enemyHp;
    //Vector2 aimInput;
    //Vector3 aimPos;
    //float MaxYBottom, MaxYTop,MaxXLeft,MaxXRight,ab;
    //Vector3 aimOriPos;
    //[SerializeField]
    //float aimSpeed;
    public bool isLocked = false;
    //Vector3 direction;
    public GameObject _lockedEnemy => lockedEnemy;
    //[SerializeField]
    //LayerMask raycastIgnore;
    [SerializeField] AudioClip farLockSFX;
    [SerializeField] AudioClip nearLockSFX;
    [SerializeField] AudioSource audioSource;
    private GameObject lastLockedEnemy = null;
    private void Awake()
    {
        nearLockAnim = NearLockImage.GetComponent<NearLockTweenAnim>();
        farLockAnim = FarLockImage.GetComponent<FarLockTweenAnim>();
        //RaycastIgnore = LayerMask.GetMask("Bullet");
    }
    // Start is called before the first frame update
    void Start()
    {
        //playerOriPos = transform.position;
        //aimOriPos = playerOriPos;
        //aimOriPos.z = playerOriPos.z+200f;
        //ab = aimOriPos.z - playerOriPos.z;
        //CalculateMaxYBottom();
        //CalculateMaxYTop();
        //CalculateMaxXLeft();
        //CalculateMaxXRight();
    }
    void FixedUpdate()
    {
        emptyAimObject.transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z + zOffset);
        aimmingImage.transform.position = playerCamera.WorldToScreenPoint(emptyAimObject.transform.position);

        if (isLocked)
        {
            if (lockedEnemy != null)
            {
                FarLockImage.SetActive(true);
                FarLockImage.transform.position = playerCamera.WorldToScreenPoint(lockedEnemy.transform.position);
                enemyHp = lockedEnemy.GetComponent<EnemyHp>();

                //  音效判斷：只有在第一次鎖定新敵人時播放
                if (lockedEnemy != lastLockedEnemy)
                {
                    lastLockedEnemy = lockedEnemy;

                    if (enemyHp.corrupted && !enemyHp.corruption_P || enemyHp.haveshield)
                    {
                        audioSource.PlayOneShot(nearLockSFX);
                    }
                    else
                    {
                        audioSource.PlayOneShot(farLockSFX);
                    }
                }

                if (enemyHp.corrupted && !enemyHp.corruption_P || enemyHp.haveshield)
                {
                    NearLockImage.SetActive(true);
                    NearLockImage.transform.position = playerCamera.WorldToScreenPoint(lockedEnemy.transform.position);
                }
                else
                {
                    NearLockImage.SetActive(false);
                }
            }
            else
            {
                isLocked = false;
                FarLockImage.SetActive(false);
                NearLockImage.SetActive(false);
                lockedEnemy = emptyAimObject;
                lastLockedEnemy = null; //  清除紀錄
            }
        }
        else
        {
            FarLockImage.SetActive(false);
            NearLockImage.SetActive(false);
            lastLockedEnemy = null; //  清除紀錄
        }
    }
    public void CallFarReactive()
    {
        farLockAnim.ReActive();
    }
    public void CallNearReactive()
    {
        nearLockAnim.ReActive();
    }
    public bool CheckLockedEnemy()
    {
        if (lockedEnemy.tag == "Enemy")
        {
            Debug.Log("Locked " + lockedEnemy.tag);
            return true;
        }
        else
        {
            Debug.Log("Missed " + lockedEnemy.tag);
            return false;
        }
    }
}

