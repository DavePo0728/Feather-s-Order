using AfterimageFX;
using Cinemachine;
using DG.Tweening;
using FUnit.GameObjectExtensions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class PlayerSlashAttack : MonoBehaviour
{
    public enum SlashState { Idle, Dashing, Arrived, Attacking, FallingBack }
    public SlashState slashState = SlashState.Idle;
    PlayerMove playerMove;
    [SerializeField] CinemachineVirtualCamera playerVCam;
    Rigidbody playerRigidbody;
    [SerializeField] Collider slashCollider;
    [SerializeField] Vector3 slashTarget;
    PlayerAim playerAim;
    Collider aimCollider;
    AimDetect aimDetect;
    Vector3 PlayerOriginalPos;
    SlashDetect slashDetect;
    GameObject shieldEffect;
	[SerializeField] GameObject shieldEffectBIG;
    [SerializeField] GameObject slashEffectYellowObject, slashEffectRedObject;
    ParticleSystem slashEffectYellow, slashEffectRed;
    ParticleSystemRenderer slashEffectYellowR, slashEffectRedR;

    AudioSource slashAudio;
	[SerializeField] AudioClip slashClip1, slashClip2, slashClip3, slashClip4;
    [SerializeField] CinemachineImpulseSource impulseSource;
    [SerializeField] CinemachineFollowZoom followZoom;
    [SerializeField] GameObject flashImage;

    bool dashCounting = false;
    int hitCounter;
    float slashTimer;
    float slashCD = 0.2f;
    [SerializeField]
    float maxTime;
    [SerializeField]
    float attackTimer = 0f;
    bool isCounting = false;
    Tweener tweener;
    GameObject target;
    EnemyHp enemyHp;

    [SerializeField] Animator playerAnimator;
    public DynamicBone clothDB;
    public GameObject sword;
    bool IsReturnAnimation = false;
	[Header("Shader Effect")]
	public Material ScreenWaveShader;
    [Header("Dash Effect Time")]
    public float dashEffectDuration = 0.5f; // 動畫總時間（秒）
    [Header("Dasheffect")]
    public GameObject Dasheffect;

	private void Awake()
    {
        aimDetect = GameObject.Find("AimDetectCollider").GetComponent<AimDetect>();
        aimCollider = GameObject.Find("AimDetectCollider").GetComponent<Collider>();
        playerMove = GetComponent<PlayerMove>();
        playerAim = GetComponent<PlayerAim>();
        slashDetect = GameObject.Find("SlashCollider").GetComponent<SlashDetect>();
        playerRigidbody = GetComponent<Rigidbody>();
        shieldEffect = GameObject.Find("MagicShieldYellow");
        shieldEffectBIG = GameObject.Find("MagicShieldYellow_1");
        slashEffectYellow = slashEffectYellowObject.GetComponent<ParticleSystem>();
        slashEffectYellowR = slashEffectYellowObject.GetComponent<ParticleSystemRenderer>();
        slashEffectRed = slashEffectRedObject.GetComponent<ParticleSystem>();
        slashAudio = slashEffectYellowObject.GetComponent<AudioSource>();
        //slashClip1 = Resources.Load<AudioClip>("Sound/slash_v2_01");
        //slashClip2 = Resources.Load<AudioClip>("Sound/slash_v2_02");
        //slashClip3 = Resources.Load<AudioClip>("Sound/slash_v2_03");
        //slashClip4 = Resources.Load<AudioClip>("Sound/slash_v2_04");
        hitCounter = 0;
    }

    public void GetSlashInput(InputAction.CallbackContext context)
    {
        if (context.performed && slashState == SlashState.Idle && playerAim.isLocked)
        {
            if (playerAim.CheckLockedEnemy())
            {

                if (playerAim._lockedEnemy.CompareTag("Enemy"))
                {
                    target = playerAim._lockedEnemy;
                    enemyHp = target.GetComponent<EnemyHp>();
                }
                else
                {
                    target = playerAim.emptyAimObject;
                }

            }

            if (enemyHp != null)
            {

                if (enemyHp.haveshield)
                {
                    playerAnimator.SetTrigger("Dash");
                    sword.SetActive(true);
                    DashToShieldEnemy();
                    aimCollider.enabled = false;
                    aimDetect.ClearAimList();
                    playerAim.showLockUI = false;
                    playerAim.aimmingImage.SetActive(false);
                    playerAim.FarLockImage.SetActive(false);
                    playerAim.NearLockImage.SetActive(false);
                    return;
                }
                if (enemyHp.corrupted && !enemyHp.corruption_P)
                {
                    dashCounting = true;
                    playerAnimator.SetTrigger("Dash");
                    sword.SetActive(true);
                    DashToEnemy();
                    aimCollider.enabled = false;
                    aimDetect.ClearAimList();
                    playerAim.showLockUI = false;
                    playerAim.aimmingImage.SetActive(false);
                    playerAim.FarLockImage.SetActive(false);
                    playerAim.NearLockImage.SetActive(false);
                }
            }
        }
        if (context.canceled)
        {
            if (slashState == SlashState.Attacking && dashCounting == false)
            {
                Debug.Log("1");
                ReturnAnimation();
            }
        }
    }

    public void GetSlashAttackInput(InputAction.CallbackContext context)
    {
        if (context.performed && slashState == SlashState.Attacking && slashTimer >= slashCD)
        {
            if (hitCounter < 3)
            {
                //print(hitCounter);

                slashCD = 0.2f;
                TriggerSlash(hitCounter);
                switch (hitCounter)
                {
                    case 0:
                        //print("S1");
                        playerAnimator.Play("S1");
                        slashEffectYellowR.flip = new Vector3(0, 0, 0);
                        slashEffectYellowR.transform.localRotation = Quaternion.Euler(79f, 315f, 207f);

                        playerAnimator.SetBool("OnAttack", true);
                        break;
                    case 1:
                        //print("S2");
                        playerAnimator.Play("S2");
                        slashEffectYellowR.flip = new Vector3(0, 1, 0);
                        slashEffectYellowR.transform.localRotation = Quaternion.Euler(61f, 141f, 305f);

                        playerAnimator.SetBool("OnAttack", true);
                        break;
                    case 2:
                        //print("S3");
                        playerAnimator.Play("S3");
                        slashEffectYellowR.flip = new Vector3(0, 0, 0);
                        slashEffectYellowR.transform.localRotation = Quaternion.Euler(79f, 315f, 207f);

                        playerAnimator.SetBool("OnAttack", true);
                        break;
                }
                hitCounter++;
                slashTimer = 0;
                attackTimer = 0;

                //print(hitCounter);

            }
            else
            {
                //print("S4");
                playerAnimator.Play("S4");
                Invoke("TriggerSlash4", 0.3f);
                playerAnimator.SetBool("OnAttack", true);
                slashCD = 0.5f;
                slashTimer = 0;
                attackTimer = 0f;
                hitCounter = 0;
            }
        }
    }

    public void TriggerSlash4()
    {
        clothDB.enabled = false;
        Vibrate(0.5f, 0.5f, 0.05f);
		slashAudio.clip = slashClip4;
		slashAudio.Play();
        slashCollider.enabled = true;
        Invoke("InactiveCollider", 0.1f);
        flashImage.SetActive(true);
        Invoke("InactiveFlashImage", 0.01f);
        slashEffectRedObject.SetActive(true);
        slashEffectRed.Play();

        Shake(1.0f);
        Time.timeScale = 0.1f;
        Invoke("TimeScaleNormal", 0.02f);
        Invoke("DelayDetect", 0.05f);
    }

    public void TriggerSlash(int hitCounter)
    {

        Vibrate(0.1f, 0.1f, 0.05f);
		slashEffectYellowObject.SetActive(true);
		switch (hitCounter)
        {
            case 0:
                slashAudio.clip = slashClip1;
				slashAudio.Play();
				break;
			case 1:
                slashAudio.clip = slashClip2;
				slashAudio.Play();
				break;
			case 2:
				slashAudio.clip = slashClip3;
                slashAudio.Play();
				break;
			default:
                break;
        }
        slashCollider.enabled = true;
        Invoke("InactiveCollider", 0.1f);
        flashImage.SetActive(true);
        Invoke("InactiveFlashImage", 0.02f);
        
        slashEffectYellow.Play();
        Shake(0.5f);
        Time.timeScale = 0.1f;
        Invoke("TimeScaleNormal", 0.01f);
        Invoke("DelayDetect", 0.05f);
    }

    void DashToEnemy()
    {
        TriggerDashEffect();

		slashState = SlashState.Dashing;
        shieldEffect.SetActive(false);
        shieldEffectBIG.SetActive(false);
        playerRigidbody.velocity = Vector3.zero;
        if (target != null)
        {
            if (target.CompareTag("Enemy"))
            {
                slashTarget = target.transform.Find("DashPoint").position;
                Vector3 lastTargetPos = slashTarget;
                //GetComponent<AfterimageController>().StartDash();
                tweener = playerRigidbody.DOMove(slashTarget, 0.5f)
                .SetDelay(0.556f)
                .OnComplete(() =>
                {

                    slashState = SlashState.Arrived;
                    playerAnimator.SetBool("CloseEnemy",true);
                    playerRigidbody.velocity = Vector3.zero;
                    isCounting = true;
                    followZoom.m_Width = 0;
                    TriggerSlash(2);
                    slashEffectYellowR.flip = new Vector3(0, 0, 0);
                    slashEffectYellowR.transform.localRotation = Quaternion.Euler(280f, 180f, 191f);
                    IsReturnAnimation = false;
                    playerAnimator.SetBool("OnAttack", true);
                    attackTimer = 0;
                    SetAttack();
                });
            }
            if (tweener != null)
            {
                if (!tweener.IsPlaying())
                {
                    tweener.Play();
                    Invoke("DashGap", 0.9f);
                }
            }
        }
    }

    void DashToShieldEnemy()
    {
        TriggerDashEffect();

		slashState = SlashState.Dashing;
        shieldEffect.SetActive(false);
        shieldEffectBIG.SetActive(false);
        playerRigidbody.velocity = Vector3.zero;
        if (target != null)
        {
            if (target.CompareTag("Enemy"))
            {
                slashTarget = target.transform.Find("DashPoint").position;
                Vector3 lastTargetPos = slashTarget;
                //GetComponent<AfterimageController>().StartDash();
                tweener = playerRigidbody.DOMove(slashTarget, 0.5f)
                .SetDelay(0.556f)
                .OnUpdate(() =>
                {
                    if ((slashTarget - lastTargetPos).sqrMagnitude > 0.01f)
                    {
                        lastTargetPos = slashTarget;
                        tweener.ChangeEndValue(slashTarget, true);
                    }
                    if (Vector3.Distance(transform.position, slashTarget) <= 0.1f)
                    {
                        tweener.Complete();
                    }
                })
                .OnComplete(() =>
                {
                    slashState = SlashState.Arrived;
                    playerAnimator.SetBool("CloseEnemy", true);
                    playerRigidbody.velocity = Vector3.zero;
                    playerVCam.m_Lens.FieldOfView = 15;
                    followZoom.m_Width = 0;
                    IsReturnAnimation = false;
                    shieldEffect.SetActive(false);
                    shieldEffectBIG.SetActive(false);
                    TriggerSlash(2);
                    Invoke("ReturnAnimation", 0.5f);
                });
            }
            if (tweener != null)
            {
                if (!tweener.IsPlaying())
                {
                    tweener.Play();
                }
            }
        }
    }

    void SetAttack()
    {
        slashState = SlashState.Attacking;
    }
    void DashGap()
    {
        dashCounting = false;
    }

    void FixedUpdate()
    {
        if (isCounting)
        {
            slashTimer += Time.deltaTime;
            attackTimer += Time.deltaTime;
            if (attackTimer >= 0.5f)
            {
                playerAnimator.SetBool("OnAttack", false);
                clothDB.enabled = true;
            }
            if (attackTimer > maxTime)
            {
                Debug.Log("2");
                ReturnAnimation();
            }
        }
        //Debug.Log("SlashState: " + slashState);
        if (slashState == SlashState.FallingBack || slashState == SlashState.Idle && playerAnimator.GetCurrentAnimatorStateInfo(0).IsName("Base Layer.Stand__Idle"))
        {
            //playerAnimator.SetTrigger("ReFly");
            playerAnimator.Play("ReFly");
        }
		//Vibrate(0.5f, 0.5f, 0.05f);
		//      if (Input.GetKey("g"))
		//      {
		//	
		//}
	}

    void ReturnAnimation()
    {
        ResetTimer();
        slashState = SlashState.FallingBack;
        playerAnimator.SetBool("CloseEnemy", false);
        hitCounter = 0;
        clothDB.enabled = true;
        playerAnimator.SetBool("OnAttack", false);
        if (!IsReturnAnimation)
        {
            playerAnimator.Play("ReFly");
            IsReturnAnimation = true;
        }
        sword.SetActive(false);
        GetComponent<AfterimageController>().StartDash();
        playerMove.CalculateFallbackSpeed();
        followZoom.m_Width = 50;
    }

    public void FallBackFinish()
    {
        playerAim.showLockUI = true;
        aimCollider.enabled = true;
        target = playerAim.emptyAimObject;
        playerAim.aimmingImage.SetActive(true);
        shieldEffect.SetActive(true);
        shieldEffectBIG.SetActive(true);
        slashState = SlashState.Idle;
    }

    public void ForceFallBack()
    {
        if (tweener != null && (tweener.IsPlaying() || slashState == SlashState.Dashing))
        {
            tweener.Kill();
            //Debug.Log("3");
            ReturnAnimation();
            return;
        }
        if (slashState == SlashState.Arrived || slashState == SlashState.Attacking)
        {
            //Debug.Log("4");
            ReturnAnimation();
            return;
        }
    }

    void DelayDetect()
    {
        if (!playerAim._lockedEnemy.CompareTag("Enemy"))
        {
            //Debug.Log("5");
            ReturnAnimation();
        }
    }

    void InactiveFlashImage() => flashImage.SetActive(false);
    void TimeScaleNormal() => Time.timeScale = 1;
    void InactiveCollider() => slashCollider.enabled = false;
    void Shake(float intensity) => impulseSource.GenerateImpulseWithForce(intensity);
    void ResetTimer()
    {
        isCounting = false;
        attackTimer = 0f;
        slashTimer = 0f;
    }
    void Vibrate(float low, float high, float duration)
    {
        if (Gamepad.current != null)
        {
            Gamepad.current.SetMotorSpeeds(low, high);
            Invoke(nameof(StopVibration), duration);
        }
    }
    void StopVibration()
    {
        if (Gamepad.current != null)
        {
            Gamepad.current.SetMotorSpeeds(0f, 0f);
        }
    }
    void OnApplicationQuit()
    {
        //Debug.Log("搖桿震動關閉");
        Gamepad.current?.SetMotorSpeeds(0, 0);
    }
    void OnDisable()
    {
        //Debug.Log("搖桿震動關閉");
        Gamepad.current?.SetMotorSpeeds(0, 0);
    }
	/// 這個方法用於觸發屏幕波動效果
	public void TriggerDashEffect()
    {
        //Debug.Log("觸發屏幕波動效果");
		StartCoroutine(PlayDashShaderEffect());
       
    }

    /// 播放屏幕波動效果的協程
    private IEnumerator PlayDashShaderEffect()
	{
        yield return new WaitForSeconds(0.5f);
        float timeElapsed = 0f;
		float duration = dashEffectDuration;
        Dasheffect.SetActive(true);

		Dasheffect.transform.position = transform.position + new Vector3(0f,0.3f,2f);
		while (timeElapsed < duration)
		{
			float t = timeElapsed / duration;

			float fractionValue = Mathf.Lerp(0f, 1f, t);
			float sizeValue = Mathf.Lerp(0f, 0.2f, t);


			ScreenWaveShader.SetFloat("_FractionTime", fractionValue);
			ScreenWaveShader.SetFloat("_Size", sizeValue);
			ScreenWaveShader.SetFloat("_Size", sizeValue);
			timeElapsed += Time.deltaTime;
			yield return null;
		}

		// 確保最終值為結束狀態
		ScreenWaveShader.SetFloat("_FractionTime", 1f);
		ScreenWaveShader.SetFloat("_Size", 0.2f);

		// 下一幀後重置回 0（讓畫面有一格保留完整效果）
		yield return null;

		ScreenWaveShader.SetFloat("_FractionTime", 0f);
        ScreenWaveShader.SetFloat("_Size", 0f);
		Dasheffect.SetActive(false);
	}
}
