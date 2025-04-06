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
        //if (aimInput.x > 0.2f || aimInput.y > 0.2f || aimInput.x < -0.2f || aimInput.y < -0.2f)
        //{
        //    aimPos = new Vector3(aimInput.x, aimInput.y, 0);
        //}
        //else
        //{
        //    aimPos = Vector3.zero;
        //}
        emptyAimObject.transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z + zOffset);
        //emptyAimObject.transform.position = new Vector3(Mathf.Clamp(emptyAimObject.transform.position.x, MaxXLeft, MaxXRight), Mathf.Clamp(emptyAimObject.transform.position.y, MaxYBottom, MaxYTop), emptyAimObject.transform.position.z);
        aimmingImage.transform.position = playerCamera.WorldToScreenPoint(emptyAimObject.transform.position);

        if (isLocked)
        {
            if (lockedEnemy != null)
            {
                FarLockImage.SetActive(true);
                FarLockImage.transform.position = playerCamera.WorldToScreenPoint(lockedEnemy.transform.position);
                enemyHp = lockedEnemy.GetComponent<EnemyHp>();
               
                if (enemyHp.corrupted&&!enemyHp.corruption_P||enemyHp.haveshield)
                {
                    NearLockImage.SetActive(true);
                    NearLockImage.transform.position = playerCamera.WorldToScreenPoint(lockedEnemy.transform.position);
                }
                else
                {
                    NearLockImage.SetActive(false);
                }
            }
            else if (lockedEnemy == null)
            {
                isLocked = false;
                FarLockImage.SetActive(false);
                NearLockImage.SetActive(false);
                lockedEnemy = emptyAimObject;
            }
        }
        else
        {
            FarLockImage.SetActive(false);
            NearLockImage.SetActive(false);
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

