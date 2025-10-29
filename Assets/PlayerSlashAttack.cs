using AfterimageFX;
using Cinemachine;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

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
	[SerializeField] List<ParticleSystem> slashEffectList = new List<ParticleSystem>();
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
    bool isDashTriggered = false;
    [SerializeField]
    float maxTime;
    [SerializeField]
    float attackTimer = 0f;
    bool isCounting = false;
    Tweener brakeTweener, dashTweener;
    GameObject target;
    EnemyHp enemyHp;

    [SerializeField] Animator playerAnimator;
    public DynamicBone clothDB;
    public GameObject sword;
    public MeshRenderer[] Sword_Shader = new MeshRenderer[3];
    public ParticleSystem grow;
    bool IsReturnAnimation = false;
    [Header("Shader Effect")]
    public Material ScreenWaveShader;
    [Header("Dash Effect Time")]
    public float dashEffectDuration = 0.5f; // 動畫總時間（秒）
    [Header("Dasheffect")]
    public GameObject Dasheffect;
    public GameObject airflow;
    public AnimSpeedByCurve asbc;

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
        InitializeScreenWaveEffect();
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
                    airflow.gameObject.SetActive(false);

                    playerAnimator.SetTrigger("Dash");
                    //sword.SetActive(true);
                    SwordAnimIn();
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
                    airflow.gameObject.SetActive(false);
                    playerAnimator.SetTrigger("Dash");
                    //sword.SetActive(true);
                    SwordAnimIn();
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
						//SlashEffectFlip(180);
						//slashEffectYellowR.transform.localRotation = Quaternion.Euler(292f, 195f, 104f);

                        playerAnimator.SetBool("OnAttack", true);
                        break;
                    case 1:
                        //print("S2");
                        playerAnimator.Play("S2");
      //                  SlashEffectFlip(0);
						//slashEffectYellowR.transform.localRotation = Quaternion.Euler(290f, 206f, 18f);

                        playerAnimator.SetBool("OnAttack", true);
                        break;
                    case 2:
                        //print("S3");
                        playerAnimator.Play("S3");
						//SlashEffectFlip(180);
						//slashEffectYellowR.transform.localRotation = Quaternion.Euler(294f, 215f, 68f);

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
                slashCD = 0.7f;
                slashTimer = 0;
                attackTimer = 0f;
                hitCounter = 0;
            }
        }
    }
    bool checkAnimationFinish()
    {
        // Get the current state info of the Animator
        AnimatorStateInfo stateInfo = playerAnimator.GetCurrentAnimatorStateInfo(0);

        // Check if the animation is playing and has finished
        if (stateInfo.IsName("S4") && stateInfo.normalizedTime >= 1.0f)
        {
            return true; // Animation has finished
        }
        else
        {
            return false; // Animation is still playing
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
        //slashEffectRedObject.SetActive(true);
		PlaySlashEffect4();

        Shake(1.0f);
        Time.timeScale = 0.1f;
        Invoke("TimeScaleNormal", 0.02f);
        Invoke("DelayDetect", 0.05f);
    }
    public void DashSlash()
    {
        isDashTriggered = true;
        Vibrate(0.1f, 0.1f, 0.05f);
		SlashEffectFlip(180);
		slashEffectYellowObject.SetActive(true);
        slashAudio.clip = slashClip3;
        slashAudio.Play();
        Invoke("PlaySlashEffect4", 0.2f);
        Invoke("ActiveCollider", 0.5f);
        flashImage.SetActive(true);
        Invoke("InactiveFlashImage", 0.02f);
        Shake(0.5f);
        Time.timeScale = 0.1f;
        Invoke("TimeScaleNormal", 0.01f);
        Invoke("DelayDetect", 0.05f);
    }
    public void DashShieldSlash()
    {
        Vibrate(0.1f, 0.1f, 0.05f);
        slashEffectYellowObject.SetActive(true);
        slashAudio.clip = slashClip3;
        slashAudio.Play();
        Invoke("PlaySlashEffectYellow", 0.2f);
        Invoke("ActiveCollider", 0.5f);
        flashImage.SetActive(true);
        Invoke("InactiveFlashImage", 0.02f);
        Shake(0.5f);
        Time.timeScale = 0.1f;
        Invoke("TimeScaleNormal", 0.01f);
        Invoke("DelayDetect", 0.05f);
    }
    public void TriggerSlash(int hitCounter)
    {


        switch (hitCounter)
        {
            case 0:
				Invoke("PlaySlashEffect1", 0.1f);
				slashAudio.clip = slashClip1;
                slashAudio.Play();
                break;
            case 1:
				Invoke("PlaySlashEffect2", 0.1f);
				slashAudio.clip = slashClip2;
                slashAudio.Play();
                break;
            case 2:
				Invoke("PlaySlashEffect3", 0.1f);
				slashAudio.clip = slashClip3;
                slashAudio.Play();
                break;
            default:
                break;
        }
        
        ActiveCollider();
        flashImage.SetActive(true);
        Invoke("InactiveFlashImage", 0.02f);
        Shake(0.5f);
        Time.timeScale = 0.1f;
        Invoke("TimeScaleNormal", 0.01f);
        Invoke("DelayDetect", 0.05f);
    }
    void PlaySlashEffectYellow()
    {
        slashEffectYellow.Play();
    }
	void PlaySlashEffect1()
	{
        slashEffectList[0].Play();
	}
	void PlaySlashEffect2()
	{
		slashEffectList[1].Play();
	}
	void PlaySlashEffect3()
	{
		slashEffectList[2].Play();
	}
	void PlaySlashEffect4()
	{
		slashEffectList[3].Play();
	}
	void DashToEnemy()
    {
        TriggerDashEffect();
        asbc.PlayWithCurve();
        slashState = SlashState.Dashing;
        shieldEffect.SetActive(false);
        shieldEffectBIG.SetActive(false);
        playerRigidbody.velocity = Vector3.zero;
        if (target != null)
        {
            if (target.CompareTag("Enemy"))
            {
                slashTarget = target.transform.Find("DashPoint").position;
                Vector3 brakePos = slashTarget + new Vector3(0, 0, -200f);
                Vector3 lastTargetPos = brakePos;
                brakeTweener = playerRigidbody.DOMove(slashTarget, 0.3f)
                .SetEase(Ease.Linear)
                .OnComplete(() =>
                {
                    slashState = SlashState.Arrived;
                    playerRigidbody.velocity = Vector3.zero;
                    isCounting = true;
                    followZoom.m_Width = 0;
                    DashSlash();
                    slashEffectYellowR.flip = new Vector3(0, 0, 0);
                    slashEffectYellowR.transform.localRotation = Quaternion.Euler(293f, 149f, 114f);

                    IsReturnAnimation = false;
                    playerAnimator.SetBool("OnAttack", true);
                    attackTimer = 0;
                });
                //GetComponent<AfterimageController>().StartDash();
                dashTweener = playerRigidbody.DOMove(brakePos, 0.2f)
                .SetDelay(0.556f)
                .SetEase(Ease.Linear)
                .OnComplete(() =>
                {
                    if (brakeTweener != null)
                    {
                        if (!brakeTweener.IsPlaying())
                        {
                            //playerAnimator.Play("Dash 0");
                            playerAnimator.SetBool("CloseEnemy", true);

                            brakeTweener.Play();
                        }
                    }
                });
            }
            if (dashTweener != null)
            {
                if (!dashTweener.IsPlaying())
                {
                    dashTweener.Play();
                    Invoke("DashGap", 0.9f);
                }
            }
        }
    }

    void DashToShieldEnemy()
    {
        TriggerDashEffect();
        asbc.PlayWithCurve();
        slashState = SlashState.Dashing;
        shieldEffect.SetActive(false);
        shieldEffectBIG.SetActive(false);
        playerRigidbody.velocity = Vector3.zero;
        if (target != null)
        {
            if (target.CompareTag("Enemy"))
            {
                slashTarget = target.transform.Find("DashPoint").position;
                Vector3 brakePos = slashTarget + new Vector3(0, 0, -5f);
                Vector3 lastTargetPos = brakePos;
                //GetComponent<AfterimageController>().StartDash();
                brakeTweener = playerRigidbody.DOMove(slashTarget, 0.1f)
                .SetEase(Ease.Linear)
                .OnComplete(() =>
                {
                    slashState = SlashState.Arrived;
                    playerRigidbody.velocity = Vector3.zero;
                    playerVCam.m_Lens.FieldOfView = 15;
                    followZoom.m_Width = 0;
                    IsReturnAnimation = false;
                    shieldEffect.SetActive(false);
                    shieldEffectBIG.SetActive(false);
                    DashShieldSlash();
                    Invoke("ReturnAnimation", 0.5f);
                });
                dashTweener = playerRigidbody.DOMove(brakePos, 0.4f)
                .SetEase(Ease.Linear)
                .SetDelay(0.556f)
                .OnUpdate(() =>
                {
                    if ((brakePos - lastTargetPos).sqrMagnitude > 0.01f)
                    {
                        lastTargetPos = brakePos;
                        dashTweener.ChangeEndValue(brakePos, true);
                    }
                    if (Vector3.Distance(transform.position, brakePos) <= 0.1f)
                    {
                        dashTweener.Complete();
                    }
                })
                .OnComplete(() =>
                {
                    if (brakeTweener != null)
                    {
                        if (!brakeTweener.IsPlaying())
                        {
                            playerAnimator.SetBool("CloseEnemy", true);
                            brakeTweener.Play();
                        }
                    }
                });
            }
            if (dashTweener != null)
            {
                if (!dashTweener.IsPlaying())
                {
                    dashTweener.Play();
                }
            }
        }
    }

    public void SetAttack()
    {
        if (isDashTriggered)
        {
            slashState = SlashState.Attacking;
            isDashTriggered = false;
        }
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
        if (slashState == SlashState.Arrived && checkAnimationFinish())
        {
            SetAttack();
        }
    }

    void ReturnAnimation()
    {
        airflow.gameObject.SetActive(true);

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
        //sword.SetActive(false);
        SwordAnimOut();
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
        playerAim.isLocked = false;
        playerAim.lockedEnemy = playerAim.emptyAimObject;
    }

    public void ForceFallBack()
    {
        if (dashTweener != null && (dashTweener.IsPlaying() || slashState == SlashState.Dashing))
        {
            dashTweener.Kill();
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
    public IEnumerator NormalizePlayer()     //復活重置位置
    {
        Debug.Log($"SlashState: {slashState}");
        if (slashState != SlashState.Idle)
        {
            slashState = SlashState.Idle;
            if (brakeTweener != null)
            {
                if (brakeTweener.active && brakeTweener.IsPlaying())
                {
                    brakeTweener.Kill();
                }
            }
            ForceFallBack();
            
            yield return null;
        }
    }
    void InactiveFlashImage() => flashImage.SetActive(false);
    void TimeScaleNormal() => Time.timeScale = 1;
    void ActiveCollider()
    {
        slashCollider.enabled = true;
        Invoke("InactiveCollider", 0.1f);
    }
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
    void InitializeScreenWaveEffect()
    {
        ScreenWaveShader.SetFloat("_FractionTime", 0f);
        ScreenWaveShader.SetFloat("_Size", 0f);
    }
    /// 播放屏幕波動效果的協程
    private IEnumerator PlayDashShaderEffect()
    {
        yield return new WaitForSeconds(0.5f);
        float timeElapsed = 0f;
        float duration = dashEffectDuration;
        Dasheffect.SetActive(true);

        Dasheffect.transform.position = transform.position + new Vector3(0f, 0.3f, 2f);
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
    public void SwordAnimIn()
    {

        StartCoroutine(SwordAnim(1f, 0f, 0.3f));
    }

    public void SwordAnimOut()
    {

        StartCoroutine(SwordAnim(0f, 1f, 0.3f));
    }

    private IEnumerator SwordAnim(float startValue, float endValue, float animDuration)
    {
        float t = 0f;

        List<Material> newmat = new List<Material>();

        foreach (var item in Sword_Shader)
        {
            newmat.Add(item.material);

        }
        if (startValue == 1f)
        {

            yield return new WaitForSeconds(1.04f);
            Debug.Log("SwordAnimIn");
            grow.Play();
        }

        while (t < animDuration)
        {
            t += Time.deltaTime;
            float progress = t / animDuration;
            float value = Mathf.Lerp(startValue, endValue, progress);

            foreach (var item in newmat)
            {
                item.SetFloat("_Dtime", value);

            }


            yield return null;
        }
        foreach (var item in newmat)
        {
            item.SetFloat("_Dtime", endValue);
        }

    }
	// 最後確保數值完全到達目標

	private void SlashEffectFlip(float tagerRota)
    {

        slashEffectYellowR.transform.GetChild(0).localRotation = Quaternion.Euler(tagerRota, 0, 0);
		slashEffectYellowR.transform.GetChild(1).localRotation = Quaternion.Euler(tagerRota, 0, 0);
		slashEffectYellowR.transform.GetChild(2).localRotation = Quaternion.Euler(tagerRota, 0, 0);

	}
}
