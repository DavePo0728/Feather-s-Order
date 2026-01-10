using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Cinemachine;
using DG.Tweening;
using UnityEngine.Windows;
using AfterimageFX;
public class PlayerMove : MonoBehaviour
{
    [SerializeField]
    CinemachineVirtualCamera playerVCam,SceneVCam;
    CinemachineFramingTransposer playerVCamFramingTransposer;
    [SerializeField]
    Animator playerAnimator;
    [Space(height: 20)]
    [SerializeField]
    AudioSource DashAudioSource;
    AudioClip DashAudioClip;
    [SerializeField]
    float dashForce;
    Rigidbody playerRigidbody;
    [SerializeField]
    float moveHspeed,moveVspeed, maxVelocity, moveHspeedMultiplier, moveVspeedMultiplier;
    [SerializeField]
    float zSpeed,oriZSpeed;
    //[SerializeField]
    //float leanAngle, lerpTime, lerpMultiplier;
    [SerializeField]
    float dutchMultiplier;
    [SerializeField]
    float fallBackTime;
    float fallbackspeed;
    //Dash
    bool canDash = true;
    bool isDashing = false;
    float dashingTime = 0.2f;
    [SerializeField]
    float dashCooldown;
    AudioSource dashSound;
    [SerializeField]
    GameObject body;
    Vector2 movementInput;
    float leanInput;
    //bool manualLean = true;
    [Header("Effect")]
    Vector3 movement;
    [SerializeField]
    PlayerSlashAttack playerSlashAttack;
    PlayerAim playerAim;
    [SerializeField]
    private float maxLookAngle, rotateSpeed, maxRollAngle, maxPitchAngle;
    float currentLookAngle;
    private float currentLookYaw = 0f;
    private float currentLookRoll = 0f;
    private float currentPitch = 0f;
    /*
     * Asuisui
        動畫控制
    */
    [HideInInspector]
	public float ChangeTimer = 0;
	//飛行待機動作時長
	float ChangeCD = 3f;
	//揮動翅膀揮動所需時間
	float FlapStayCD = 2.0f;
	//揮動翅膀狀態
	bool OnFlap = false;

	public List<GameObject> outline = new List<GameObject>();

    
	private void Awake()
    {
        playerRigidbody = GetComponent<Rigidbody>();
        zSpeed = oriZSpeed;
        playerSlashAttack = GetComponent<PlayerSlashAttack>();
        playerVCamFramingTransposer = playerVCam.GetCinemachineComponent<CinemachineFramingTransposer>();
        playerAim = GetComponent<PlayerAim>();
        //manualLean = false;
    }
    // Start is called before the first frame update
    void Start()
    {
        //currentEnergy = maxEnergy;
        //UpdateUI();
        


	}
    public void GetMove(InputAction.CallbackContext context)
    {
        movementInput = context.ReadValue<Vector2>();
    }
    public void GetDash(InputAction.CallbackContext context)
    {
        if (context.performed && playerSlashAttack.slashState == PlayerSlashAttack.SlashState.Idle)
        {
            if (canDash/*&&currentEnergy>=20&&!isOutBurst*/&& playerRigidbody.velocity.x != 0)
            {
				StartCoroutine(OnDash());
                
                
				//currentEnergy -= 20;
				//UpdateUI();
				// timeSinceLastEnergyUse = 0f;   // 重置時間計數器
				//isRegening = false;
			}
        }
    }
    //public void GetBounce(InputAction.CallbackContext context)
    //{
    //    if (context.performed)
    //    {
    //        rotateStartTime = Time.time;
    //        isBouncing = true;
    //        isRotating = true;
    //        StartCoroutine(MuTeKiTime(0.2f));
    //        //currentEnergy -= 20;
    //        //UpdateUI();
    //        //timeSinceLastEnergyUse = 0f;   // 重置時間計數器
    //        //isRegening = false;
    //    }
    //}


	//public void GetLean(InputAction.CallbackContext context)
	//{
	//    if (context.performed)
	//    {
	//        leanInput = context.ReadValue<float>();
	//        Debug.Log(leanInput);
	//        manualLean = true;
	//    }
	//    if (context.canceled)
	//    {
	//        manualLean = false;
	//        leanInput = 0;
	//    }
	//}
	private void Update()
	{

		ChangeTimer += Time.deltaTime;
		if (OnFlap == false && ChangeTimer >= ChangeCD)
		{
			ChangeTimer = 0;
			OnFlap = true;
			playerAnimator.SetTrigger("flap");
			//TriggerFlap();
		}
		if (OnFlap == true && ChangeTimer >= FlapStayCD)
		{
			ChangeTimer = 0;
			OnFlap = false;
		}

	}
	// Update is called once per frame
	void FixedUpdate()
    {
        
        //transform.Translate(Vector3.forward* zSpeed * Time.deltaTime);
        //Debug.Log(movementInput.x);
        //playerRigidbody.velocity = Vector3.right * moveSpeed;
        //playerRigidbody.velocity = Vector3.ClampMagnitude(playerRigidbody.velocity, maxVelocity);
        //Debug.Log(Vector3.forward * zSpeed * Time.deltaTime);
        // Normal Movement
        if (!isDashing)
        {
            //transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0, 0, 90 * leanInput), lerpTime * Time.deltaTime * lerpMultiplier);
            if (movementInput.x>0.2f||movementInput.y>0.2f|| movementInput.x < -0.2f || movementInput.y < -0.2f)
            {
                movement = new Vector3(movementInput.x, movementInput.y, 0);
            }
            else
            {
                movement = Vector3.zero;
            }
            if (playerSlashAttack.slashState == PlayerSlashAttack.SlashState.FallingBack)
            {
                if (transform.position.z > 13)
                {
                    playerRigidbody.velocity = new Vector3(movement.x * moveHspeed, movement.y * moveVspeed, -fallbackspeed);
                    Vector2 clampedXY = Vector2.ClampMagnitude(new Vector2(playerRigidbody.velocity.x, playerRigidbody.velocity.y), maxVelocity);
                    playerRigidbody.velocity = new Vector3(clampedXY.x, clampedXY.y, playerRigidbody.velocity.z);
                }
                else
                {
                    playerSlashAttack.FallBackFinish();
                }
            }
            else if (playerSlashAttack.slashState == PlayerSlashAttack.SlashState.Idle)
            {
                playerRigidbody.velocity = new Vector3(movement.x * moveHspeed, movement.y * moveVspeed, 0);
                playerRigidbody.velocity = Vector3.ClampMagnitude(playerRigidbody.velocity, maxVelocity);
            }
            float targetYaw = movementInput.x * maxLookAngle;   // 左右轉頭 (Y 軸)
            float targetRoll = -movementInput.x * maxRollAngle; // 左右傾斜 (Z 軸)，左傾為正 or 負視需求
            float targetPitch = -movementInput.y * maxPitchAngle;

            // 2. 平滑插值
            currentLookYaw = Mathf.Lerp(currentLookYaw, targetYaw, Time.deltaTime * rotateSpeed);
            currentLookRoll = Mathf.Lerp(currentLookRoll, targetRoll, Time.deltaTime * rotateSpeed);
            currentPitch = Mathf.Lerp(currentPitch, targetPitch, Time.deltaTime * rotateSpeed);

            // 3. 套用旋轉（Y 為左右看，Z 為左右傾斜）
            body.transform.localRotation = Quaternion.Euler(currentPitch, currentLookYaw, currentLookRoll);


            //Debug.Log(playerRigidbody.velocity);
            // flying lean
            if (playerRigidbody.velocity.x < -0.2f)
            {
                //leanStartTime = Time.time;
                //body.transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0, 0, leanAngle), lerpTime * Time.deltaTime * lerpMultiplier);
                //body.transform.LookAt(playerAim.emptyAimObject.transform);
                //float t = (Time.time - rotateStartTime) / 2.0f;
                //vCam.m_Lens.Dutch = Mathf.SmoothStep(vCam.m_Lens.Dutch, -10, t);
                playerVCam.m_Lens.Dutch = Mathf.Lerp(playerVCam.m_Lens.Dutch, -10, 0.2f * Time.deltaTime * 8);
                if (SceneVCam != null)
                {
                    SceneVCam.m_Lens.Dutch = Mathf.Lerp(playerVCam.m_Lens.Dutch, -10, 0.2f * Time.deltaTime * 8);
                }
            }
            else if (playerRigidbody.velocity.x > 0.2f)
            {
              //float t = (Time.time - rotateStartTime) / 2.0f;
                //vCam.m_Lens.Dutch = Mathf.SmoothStep(vCam.m_Lens.Dutch, 10, t);
                playerVCam.m_Lens.Dutch = Mathf.Lerp(playerVCam.m_Lens.Dutch, 10, 0.2f * Time.deltaTime * 8);
                if (SceneVCam != null)
                {
                    SceneVCam.m_Lens.Dutch = Mathf.Lerp(playerVCam.m_Lens.Dutch, 10, 0.2f * Time.deltaTime * 8);
                }
            }
            else if (playerRigidbody.velocity.x == 0 && !isDashing)
            {
                //leanStartTime = Time.time;
                //transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0, 0, 0), lerpTime * Time.deltaTime * lerpMultiplier);
                //float t = (Time.time - rotateStartTime) / 2.0f;
                //vCam.m_Lens.Dutch = Mathf.SmoothStep(vCam.m_Lens.Dutch, 0, t);
                playerVCam.m_Lens.Dutch = Mathf.Lerp(playerVCam.m_Lens.Dutch, 0, 0.2f * Time.deltaTime * 8);
                if (SceneVCam != null)
                {
                    SceneVCam.m_Lens.Dutch = Mathf.Lerp(playerVCam.m_Lens.Dutch, 0, 0.2f * Time.deltaTime * 8);
                }
            }
        }
    }
    public void CalculateFallbackSpeed()
    {
        // 計算目標點 Z 軸的距離
        float distance = Mathf.Abs(13f - transform.position.z);
        //Debug.Log($"Calculated fallbackspeed: {distance}");
        // 速度 = 距離 ÷ 時間（0.5秒）
        fallbackspeed = distance / fallBackTime;

        //Debug.Log($"Calculated fallbackspeed: {fallbackspeed}");
    }
    IEnumerator OnDash()
    {
        
        //Debug.Log(playerRigidbody.velocity.x);
		canDash = false;
        isDashing = true;
        playerVCamFramingTransposer.m_SoftZoneWidth = 0.8f;
        playerVCamFramingTransposer.m_XDamping = 1.2f;
		DashAudioSource.Play();
		if (playerRigidbody.velocity.x != 0)
        {
            playerRigidbody.AddForce(playerRigidbody.velocity * dashForce, ForceMode.Impulse);
            GetComponent<AfterimageController>().StartDash();
			foreach (var item in outline)
            {
				item.SetActive(true);

			}
	
        }

        yield return new WaitForSeconds(dashingTime);
        isDashing = false;
        //DOTween.To(() => leanAngle, x => leanAngle = x, 30f, 1.5f).Play();
        DOTween.To(() => playerVCamFramingTransposer.m_XDamping, x => playerVCamFramingTransposer.m_XDamping = x, 0.4f, 0.4f).Play();
        DOTween.To(() => playerVCamFramingTransposer.m_SoftZoneWidth, x => playerVCamFramingTransposer.m_SoftZoneWidth = x, 0.4f, 0.4f).Play();
        StartCoroutine(StartCountdown());
		foreach (var item in outline)
		{
			item.SetActive(false);

		}
		yield return new WaitForSeconds(dashCooldown);
        canDash = true;
    }
    IEnumerator StartCountdown()
    {
        float duration = dashCooldown;
        float totalTime = 0;
        float startTime = Time.time;

        while (totalTime <= duration)
        {
            totalTime = Time.time - startTime;
            float currentValue = totalTime / duration;
            yield return null;
        }
    }
    IEnumerator MuTeKiTime(float mutekiTime)
    {
        //isMuteki = true;
        Physics.IgnoreLayerCollision(8, 6, true);
        yield return new WaitForSeconds(mutekiTime);
        //isMuteki = false;
        Physics.IgnoreLayerCollision(8, 6, false);
    }
}
