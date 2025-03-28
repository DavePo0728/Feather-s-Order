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
    public GameObject aimmingImage,FarLockImage,NearLockImage;
    public GameObject lockedEnemy;
    public GameObject emptyAimObject;
    NearLockTweenAnim nearLockAnim;
    FarLockTweenAnim farLockAnim;
    [SerializeField]
    public float zOffset;
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
        nearLockAnim =NearLockImage.GetComponent<NearLockTweenAnim>();
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
        emptyAimObject.transform.position=new Vector3(transform.position.x, transform.position.y, transform.position.z+zOffset);
        //emptyAimObject.transform.position = new Vector3(Mathf.Clamp(emptyAimObject.transform.position.x, MaxXLeft, MaxXRight), Mathf.Clamp(emptyAimObject.transform.position.y, MaxYBottom, MaxYTop), emptyAimObject.transform.position.z);
        aimmingImage.transform.position = playerCamera.WorldToScreenPoint(emptyAimObject.transform.position);

        if (isLocked)
        {
            if (lockedEnemy != null)
            {
                FarLockImage.SetActive(true);
                FarLockImage.transform.position = playerCamera.WorldToScreenPoint(lockedEnemy.transform.position);
            }
            else if(lockedEnemy == null)
            {
                isLocked = false;
                FarLockImage.SetActive(false);
                lockedEnemy = emptyAimObject;
            }
        }
        else
        {
            FarLockImage.SetActive(false);
        }
    }
    public void CallFarReactive()
    {
        farLockAnim.ReActive();
    }
    
    //void CalculateMaxYBottom()
    //{
    //    MaxYBottom = ab * Mathf.Tan(50f * Mathf.Deg2Rad);
    //    //MaxYBottom = playerOriPos.y-MaxYBottom;
    //    //Debug.Log("ab : " +ab);
    //    //Debug.Log(MaxYBottom);
    //}
    //void CalculateMaxYTop()
    //{
    //    MaxYTop = ab * Mathf.Tan(70f * Mathf.Deg2Rad);
    //    //MaxYTop = Mathf.Abs(MaxYTop);
    //    //MaxYTop = playerOriPos.y + MaxYTop;

    //    //Debug.Log(MaxYTop);
    //}
    //void CalculateMaxXLeft()
    //{
    //    MaxXLeft = ab * Mathf.Tan(82.8f * Mathf.Deg2Rad);
    //    //MaxXLeft = playerOriPos.x - MaxXLeft;
    //    //Debug.Log(MaxXLeft);
    //}
    //void CalculateMaxXRight()
    //{
    //    MaxXRight = ab * Mathf.Tan(85.26f * Mathf.Deg2Rad);
    //    //MaxXRight = playerOriPos.x + MaxXRight;
    //    //Debug.Log(MaxXRight);
    //}
    //public void GetAimInput(InputAction.CallbackContext context)
    //{
    //    aimInput = context.ReadValue<Vector2>();
    //}
    // Update is called once per frame
}

