using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class PlayerAim : MonoBehaviour
{
    Vector3 playerOriPos;
    [SerializeField]
    Camera playerCamera;
    [SerializeField]
    GameObject aimmingImage,lockImage;
    public GameObject lockedEnemy;
    public GameObject emptyAimObject;
    Vector2 aimInput;
    Vector3 aimPos;
    float MaxYBottom, MaxYTop,MaxXLeft,MaxXRight,ab;
    Vector3 aimOriPos;
    [SerializeField]
    float aimSpeed;
    public bool isLocked = false;
    //Vector3 direction;
    public GameObject _lockedEnemy => lockedEnemy;
    [SerializeField]
    LayerMask raycastIgnore;
    private void Awake()
    {
        //RaycastIgnore = LayerMask.GetMask("Bullet");
    }
    // Start is called before the first frame update
    void Start()
    {
        playerOriPos = transform.position;
        aimOriPos = playerOriPos;
        aimOriPos.z = playerOriPos.z+200f;
        ab = aimOriPos.z - playerOriPos.z;
        CalculateMaxYBottom();
        CalculateMaxYTop();
        CalculateMaxXLeft();
        CalculateMaxXRight();
    }
    void CalculateMaxYBottom()
    {
        MaxYBottom = ab * Mathf.Tan(50f * Mathf.Deg2Rad);
        //MaxYBottom = playerOriPos.y-MaxYBottom;
        //Debug.Log("ab : " +ab);
        //Debug.Log(MaxYBottom);
    }
    void CalculateMaxYTop()
    {
        MaxYTop = ab * Mathf.Tan(70f * Mathf.Deg2Rad);
        //MaxYTop = Mathf.Abs(MaxYTop);
        //MaxYTop = playerOriPos.y + MaxYTop;
        
        //Debug.Log(MaxYTop);
    }
    void CalculateMaxXLeft()
    {
        MaxXLeft = ab * Mathf.Tan(82.8f * Mathf.Deg2Rad);
        //MaxXLeft = playerOriPos.x - MaxXLeft;
        //Debug.Log(MaxXLeft);
    }
    void CalculateMaxXRight()
    {
        MaxXRight = ab * Mathf.Tan(85.26f * Mathf.Deg2Rad);
        //MaxXRight = playerOriPos.x + MaxXRight;
        //Debug.Log(MaxXRight);
    }
    public void GetAimInput(InputAction.CallbackContext context)
    {
        aimInput = context.ReadValue<Vector2>();
    }
    // Update is called once per frame
    void FixedUpdate()
    {
        if (aimInput.x > 0.2f || aimInput.y > 0.2f || aimInput.x < -0.2f || aimInput.y < -0.2f)
        {
            aimPos = new Vector3(aimInput.x, aimInput.y, 0);
        }
        else
        {
            aimPos = Vector3.zero;
        }
        emptyAimObject.transform.Translate(new Vector3(aimPos.x * aimSpeed , aimPos.y * aimSpeed, 0));
        emptyAimObject.transform.position = new Vector3(Mathf.Clamp(emptyAimObject.transform.position.x, MaxXLeft, MaxXRight), Mathf.Clamp(emptyAimObject.transform.position.y, MaxYBottom, MaxYTop), emptyAimObject.transform.position.z);
        aimmingImage.transform.position = playerCamera.WorldToScreenPoint(emptyAimObject.transform.position);

        if (isLocked)
        {
            if (lockedEnemy != null)
            {
                lockImage.SetActive(true);
                lockImage.transform.position = playerCamera.WorldToScreenPoint(lockedEnemy.transform.position);
            }
            else if(lockedEnemy == null)
            {
                isLocked = false;
                lockImage.SetActive(false);
                lockedEnemy = emptyAimObject;
            }
        }
        else
        {
            lockImage.SetActive(false);
        }
    }
    Vector3 GetPointAtZ(Vector3 origin, Vector3 end, float point)
    {
        // 計算線段上 Z 坐標為 z 的點的比例
        float t = (point - origin.z) / (end.z - origin.z);

        // 使用線性插值計算該點的 X 和 Y 坐標
        float x = Mathf.Lerp(origin.x, end.x, t);
        float y = Mathf.Lerp(origin.y, end.y, t);

        return new Vector3(x, y, point);
    }
    //IEnumerator FillAndScale()
    //{
    //    aimmingImage.transform.localScale = Vector3.one;
    //    // Fill the image
    //    float elapsedTime = 0f;
    //    while (elapsedTime < fillDuration)
    //    {
    //        elapsedTime += Time.deltaTime;
    //        aimmingImage.fillAmount = Mathf.Clamp01(elapsedTime / fillDuration);
    //        yield return null;
    //    }

    //    // Ensure fill amount is exactly 1
    //    aimmingImage.fillAmount = 1f;

    //    elapsedTime = 0f;
    //    while (elapsedTime < scaleDuration)
    //    {
    //        elapsedTime += Time.deltaTime;
    //        float scale = Mathf.Lerp(1f, 1.2f, elapsedTime / (scaleDuration / 2));
    //        aimmingImage.transform.localScale = Vector3.one * scale;
    //        yield return null;
    //    }

    //    elapsedTime = 0f;
    //    while (elapsedTime < scaleDuration)
    //    {
    //        elapsedTime += Time.deltaTime;
    //        float scale = Mathf.Lerp(1.2f, 0.9f, elapsedTime / (scaleDuration / 2));
    //        aimmingImage.transform.localScale = Vector3.one * scale;
    //        yield return null;
    //    }

    //    // Ensure the scale is exactly 0.9
    //    aimmingImage.transform.localScale = Vector3.one * 0.9f;
    //    aimmed = true;
    //}
}

