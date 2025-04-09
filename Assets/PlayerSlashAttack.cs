using Cinemachine;
using DG.Tweening;
using FUnit.GameObjectExtensions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerSlashAttack : MonoBehaviour
{
    PlayerMove playerMove;
    [SerializeField]
    CinemachineVirtualCamera playerVCam;
    Rigidbody playerRigidbody;
    public bool isSlashDashing;
    public bool arrived = false;
    bool isAttacking;
    public bool isFallBack;
    [SerializeField]
    Collider slashCollider;
    [SerializeField]
    Vector3 slashTarget;
    PlayerAim playerAim;
    Vector3 PlayerOriginalPos;
    SlashDetect slashDetect;
    GameObject shieldEffect;
    [SerializeField]
    GameObject slashEffectYellowObject, slashEffectRedObject;
    ParticleSystem slashEffectYellow, slashEffectRed;
    AudioSource slashAudio;
    AudioClip slashClip;
    [SerializeField]
    CinemachineImpulseSource impulseSource;
    [SerializeField]
    CinemachineFollowZoom followZoom;
    [SerializeField]
    GameObject flashImage;
    int hitCounter;
    float SlashTimer;
    float slashCD = 0.2f;
    float maxTime = 3.0f; // 限制的時間 (1 秒)
    float attackTimer = 0f; // 計時器
    bool isCounting = false; // 是否計時中
    bool attackTriggered = false;
    bool tweenPlaying = false;
    bool tween1Playing = false;
    bool camTweenPlaying = false;
    bool camtween1Playing = false;
    bool isTracking = false;
    Tweener tweener, tweenCam;
    GameObject target;
    EnemyHp enemyHp;
    
	/*
     * Asuisui
        動畫控制
    */
	[SerializeField]
    Animator playerAnimator;
	public DynamicBone clothDB;
    public GameObject sword;
    bool IsReturnAnimation = false;

	private void Awake()
    {
        playerMove = GetComponent<PlayerMove>();
        playerAim = GetComponent<PlayerAim>();
        isSlashDashing = false;
        isAttacking = false;
        slashDetect = GameObject.Find("SlashCollider").GetComponent<SlashDetect>();
        playerRigidbody = GetComponent<Rigidbody>();
        shieldEffect = GameObject.Find("MagicShieldYellow");
        slashEffectYellow = slashEffectYellowObject.GetComponent<ParticleSystem>();
        slashEffectRed = slashEffectRedObject.GetComponent<ParticleSystem>();
        slashAudio = slashEffectYellowObject.GetComponent<AudioSource>();
        slashClip = Resources.Load<AudioClip>("Sound/Slash01");
        hitCounter = 0;
    }
    public void GetSlashInput(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (isSlashDashing == false&&arrived==false&&isFallBack==false)
            {
                if (playerAim.CheckLockedEnemy())
                {
                    target = playerAim._lockedEnemy;
                    enemyHp = target.GetComponent<EnemyHp>();
                    //Debug.Log("SlashInputTrigger");
                }
                if (enemyHp != null)
                {
                    if (enemyHp.haveshield == true)
                    {
                        playerAnimator.SetTrigger("Dash");
						sword.SetActive(true);
						DashToShieldEnemy();
						playerAim.aimmingImage.SetActive(false);
						playerAim.FarLockImage.SetActive(false);
						return;
                    }
                    if (enemyHp.corrupted&&enemyHp.corruption_P == false)
                    {
                        //Asuisui
                        playerAnimator.SetTrigger("Dash");
						sword.SetActive(true);
						//--
						DashToEnemy();
                        playerAim.aimmingImage.SetActive(false);
                        playerAim.FarLockImage.SetActive(false);
                    }
                }

            }
            else if(isSlashDashing && arrived)
            {
                    Debug.Log("2");
                    ReturnAnimation();
                    return;
                //DashToEnemy();
            }
        }
    }
    public void GetSlashAttackInput(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (isAttacking && SlashTimer >= slashCD)
            {
                if (hitCounter < 3)
                {
                    slashCD = 0.2f;

                    Invoke("TriggerSlash", 0.1f);

                    hitCounter++;
                    SlashTimer = 0;
                    attackTimer = 0;


                    //Asuisui
                    switch (hitCounter)
                    {
                        case 1:
                            playerAnimator.Play("S1");
                            playerAnimator.SetBool("OnAttack", true);
                            break;
                        case 2:
                            playerAnimator.Play("S2");
							playerAnimator.SetBool("OnAttack", true);
							break;
                        case 3:
                            playerAnimator.Play("S3");
							playerAnimator.SetBool("OnAttack", true);
							break;
                        default:
                            break;
                    }
                    //--
                }
                else if (hitCounter >= 3)
                {
                    playerAnimator.Play("S4");
                    Invoke("TriggerSlash4", 0.3f);
					playerAnimator.SetBool("OnAttack", false);
					slashCD = 0.5f;
					SlashTimer = 0;
					attackTimer = 0;
				}
            }
        }
    }
	public void TriggerSlash4()
    {
		clothDB.enabled = false;
		Vibrate(0.5f, 0.5f, 0.05f);
		slashAudio.Play();
		slashCollider.enabled = true;
		Invoke("InactiveCollider", 0.1f);
		flashImage.SetActive(true);
		Invoke("InactiveFlashImage", 0.01f);
		slashEffectRedObject.SetActive(true);
		slashEffectRed.Play();
		hitCounter = 0;
		Shake(1.0f);
		Time.timeScale = 0.1f;
		Invoke("TimeScaleNormal", 0.02f);

		Invoke("DelayDetect", 0.05f);
	}

	public void TriggerSlash() 
    {
        Vibrate(0.1f, 0.1f, 0.05f);
        slashAudio.Play();
        slashCollider.enabled = true;
        Invoke("InactiveCollider", 0.1f);
        flashImage.SetActive(true);
        Invoke("InactiveFlashImage", 0.02f);
        switch (hitCounter)
        {
			case 0:
				break;
				slashEffectYellowObject.transform.localScale = new Vector3(2.72f, -2.72f, 2.72f);
			case 1:
				break;
				slashEffectYellowObject.transform.localScale = new Vector3(2.72f, 2.72f, 2.72f);
			case 2:
				slashEffectYellowObject.transform.localScale = new Vector3(2.72f, -2.72f, 2.72f);
				break;
			case 3:
				slashEffectYellowObject.transform.localScale = new Vector3(2.72f, 2.72f, 2.72f);
				break;
		}
        slashEffectYellowObject.SetActive(true);
        slashEffectYellow.Play();
        Shake(0.5f);
        Time.timeScale = 0.1f;
        Invoke("TimeScaleNormal", 0.01f);
        Invoke("DelayDetect",0.05f);
    }
    void InactiveFlashImage()
    {
        flashImage.SetActive(false);
    }
	void DelayDetect()
    {
        if (!playerAim._lockedEnemy.CompareTag("Enemy"))
        {
            Debug.Log("3");
            ReturnAnimation();
            
            return;
        }
    }

    void DashToEnemy()
	{
		shieldEffect.SetActive(false);
        playerRigidbody.velocity = Vector3.zero;
        if (target != null) 
        slashTarget = target.transform.GetChild(7).transform.position;
        Vector3 lastTargetPos = slashTarget;
        tweener = playerRigidbody.DOMove(slashTarget, 0.5f).OnStart(() => { tweenPlaying = true;}).OnUpdate(() =>
        {
            // 持續更新目標位置
            if ((slashTarget - lastTargetPos).sqrMagnitude > 0.01f) // 有改變才更新
            {
                lastTargetPos = slashTarget;
                tweener.ChangeEndValue(slashTarget, true);
            }
            // 計算當前位置與目標位置的距離
            if (Vector3.Distance(transform.position, slashTarget) <= 0.1f)
            {
                tweener.Complete();// 直接完成動畫
            }
        }).OnComplete(() =>
        {
			
			tweenPlaying = false;
            arrived = true;
            playerRigidbody.velocity = Vector3.zero;

            //playerVCam.m_Lens.FieldOfView = 15;
            //Debug.Log("arrived: "+arrived);
            
            isCounting = true;
            followZoom.m_Width = 0;
            TriggerSlash();
            IsReturnAnimation = false;
			playerAnimator.SetBool("OnAttack", true);
			attackTimer = 0;
			Invoke("SetAttack", 0.2f);
        });
        //tweenCam = playerVCam.transform.DOLocalRotateQuaternion(Quaternion.Euler(5, -15, 0), 0.3f).OnStart(() => camTweenPlaying = true).OnComplete(() => camTweenPlaying = false);
        if (isSlashDashing ==false&&arrived==false)
        {
            isSlashDashing = true;
            //if (!camTweenPlaying)
            //{
            //    tweenCam.Play();
            //}
            if (!tweenPlaying)
            {
                tweener.Play();
            }
            //transform.position = new Vector3(transform.position.x, transform.position.y, slashtarget.position.z);
        }
    }
    void DashToShieldEnemy()
    {
        playerRigidbody.velocity = Vector3.zero;
        //target = playerAim._lockedEnemy;
        slashTarget = target.transform.GetChild(7).transform.position;
        Debug.Log("slashTarget: " + slashTarget);
        Vector3 lastTargetPos = slashTarget;
        tweener = playerRigidbody.DOMove(slashTarget, 0.5f).OnStart(() => { tweenPlaying = true;}).OnUpdate(() =>
        {
            // 持續更新目標位置
            if ((slashTarget - lastTargetPos).sqrMagnitude > 0.01f) // 有改變才更新
            {
                lastTargetPos = slashTarget;
                tweener.ChangeEndValue(slashTarget, true);
            }
            // 計算當前位置與目標位置的距離
            if (Vector3.Distance(transform.position, slashTarget) <= 0.1f)
            {
                tweener.Complete();// 直接完成動畫
            }
        }).OnComplete(() =>
        {
            tweenPlaying = false;
            arrived = true;
            playerRigidbody.velocity = Vector3.zero;

            playerVCam.m_Lens.FieldOfView = 15;
            followZoom.m_Width = 0;
			//Debug.Log("arrived: "+arrived);
			IsReturnAnimation = false;
			shieldEffect.SetActive(false);
            TriggerSlash();
            Invoke("ReturnAnimation", 0.5f);
        });
            isSlashDashing = true;
            if (!tweenPlaying)
            {
                tweener.Play();
            }
    }
    void SetAttack() 
    {
        isAttacking = true;
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (isCounting)
        {
            SlashTimer += Time.deltaTime;
            attackTimer += Time.deltaTime;
            if (attackTimer >= 1f)
            {
                playerAnimator.SetBool("OnAttack", false);
				clothDB.enabled = true;
			}
            //Debug.Log(attackTimer);
            if (attackTimer > maxTime)
            {
                Debug.Log("5");
                ReturnAnimation(); // 超時執行返回動畫
                ResetTimer();
            }
        }


    }
    public void ForceFallBack()
    {
        
        if (target != null)
        {
           // Debug.Log("Target: " + target.name);
            enemyHp = target.GetComponent<EnemyHp>();
            //Debug.Log("Target: " + target.name + "Corruption: " + enemyHp.corruption_P);
            if (enemyHp.corruption_P)
            {
                if (tweener != null)
                {
                    if (tweener.IsPlaying() || isSlashDashing)
                    {
                        Debug.Log("6");
                        tweener.Kill();
                        ReturnAnimation();
                        return;
                    }
                }
                if (arrived)
                {
                    Debug.Log("8");
					
					ReturnAnimation();
                    return;
                }
            }
                //if (isCounting)
                //{
                //    Debug.Log("7");
                //    ReturnAnimation();
                //    isCounting = false;
                //    attackTimer = 0f;
                //    return;
                //}  
        }
    }
    void ReturnAnimation()
    {
        //Asuisui
        hitCounter = 0;
		clothDB.enabled = true;
		playerAnimator.SetBool("OnAttack", false);
		if (!IsReturnAnimation)
        {
            playerAnimator.Play("ReFly");
			IsReturnAnimation = true;
			print("ReFly");
		}
        sword.SetActive(false);

        //--
        ResetTimer();
        isSlashDashing = false;
        isAttacking = false;
        playerMove.CalculateFallbackSpeed();
        isFallBack = true;
        followZoom.m_Width = 50;
    }
    public void FallBackFinish()
    {
        isFallBack = false;
        arrived = false;
        isSlashDashing = false;
        playerAim.aimmingImage.SetActive(true);
        shieldEffect.SetActive(true);
        target = null;
    }
    void Vibrate(float lowFrequency, float highFrequency, float duration)
    {
        if (Gamepad.current != null) // 確保手把已連接
        {
            Gamepad.current.SetMotorSpeeds(lowFrequency, highFrequency);
            Invoke(nameof(StopVibration), duration); // 設定定時停止震動
        }
    }
    void StopVibration()
    {
        if (Gamepad.current != null)
        {
            Gamepad.current.SetMotorSpeeds(0f, 0f); // 停止震動
        }
    }

    void TimeScaleNormal()
    {
        Time.timeScale = 1;
    }
    void InactiveCollider()
    {
        slashCollider.enabled = false;
    }
    void Shake(float intensity)
    {
        impulseSource.GenerateImpulseWithForce(intensity);
    }
    void ResetTimer()
    {
        isCounting = false;
        attackTimer = 0f;
    }
}
