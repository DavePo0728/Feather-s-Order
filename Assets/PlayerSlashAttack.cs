using Cinemachine;
using DG.Tweening;
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

    public GameObject sword;
    private void Awake()
    {
        playerMove = GetComponent<PlayerMove>();
        playerAim = GetComponent<PlayerAim>();
        isSlashDashing = false;
        isAttacking = false;
        slashDetect = transform.GetChild(4).GetComponent<SlashDetect>();
        playerRigidbody = GetComponent<Rigidbody>();
        shieldEffect = transform.GetChild(2).gameObject;
        slashEffectYellowObject = transform.GetChild(6).gameObject;
        slashEffectRedObject = transform.GetChild(7).gameObject;
        slashEffectYellow = slashEffectYellowObject.GetComponent<ParticleSystem>();
        slashEffectRed = slashEffectRedObject.GetComponent<ParticleSystem>();
        slashAudio = transform.GetChild(6).GetComponent<AudioSource>();
        slashClip = Resources.Load<AudioClip>("Sound/Slash01");
        hitCounter = 0;
    }
    public void GetSlashInput(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (isSlashDashing == false&&arrived==false&&isFallBack==false)
            {
                if (!playerAim._lockedEnemy.CompareTag("Enemy"))
                {
                    Debug.Log("1");
                    ReturnAnimation();
                    return;
                }

                //PlayerOriginalPos = transform.position;
                //PlayerOriginalPos.z = 13f;
                enemyHp = playerAim._lockedEnemy.GetComponent<EnemyHp>();
                if (enemyHp.corruption_P ==false)
				{
                    //Asuisui
					playerAnimator.SetTrigger("dash");
					//--
					DashToEnemy();
                    playerAim.aimmingImage.SetActive(false);
                    playerAim.FarLockImage.SetActive(false);
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

                    Invoke("TriggerSlash", 0.4f);

                    hitCounter++;
                    SlashTimer = 0;
                    attackTimer = 0;


                    //Asuisui
                    switch (hitCounter)
                    {
                        case 1:
                            playerAnimator.SetTrigger("S1");
                            break;
                        case 2:
                            playerAnimator.SetTrigger("S2");
                            break;
                        case 3:
                            playerAnimator.SetTrigger("S3");
                            break;
                        default:
                            break;
                    }
                    //--


                }
                else if (hitCounter >= 3)
                {
                    playerAnimator.SetTrigger("S4");
					Invoke("TriggerSlash4", 0.6f);
					slashCD = 0.3f;
					SlashTimer = 0;
					attackTimer = 0;
				}
            }
        }
    }
	public void TriggerSlash4()
    {
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
    void InactiveFlashImageLeft(string name)
    {
        flashImage.SetActive(false);
        switch (name)
        {
            case "left":
                break;
                flashImage.transform.localScale = new Vector3(2.72f, -2.72f, 2.72f);
            case "right":
                flashImage.transform.localScale = new Vector3(2.72f, 2.72f, 2.72f);
                break;

            default:
                break;
        }


    }

	void InactiveFlashImageRight(string name)
    {

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
		sword.SetActive(true);
		playerRigidbody.velocity = Vector3.zero;
        target = playerAim._lockedEnemy;
        enemyHp = target.GetComponent<EnemyHp>();
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
            Debug.Log("arrived: "+arrived);
            shieldEffect.SetActive(false);
            isCounting = true;
            followZoom.m_Width = 0;
            TriggerSlash();
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
        else if(arrived) 
        {
            Debug.Log("4");
            ReturnAnimation();
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
    void Update()
    {
        if (isCounting)
        {
            SlashTimer += Time.deltaTime;
            attackTimer += Time.deltaTime;
            //Debug.Log(attackTimer);
            if (attackTimer > maxTime)
            {
                Debug.Log("5");
                ReturnAnimation(); // 超時執行返回動畫
                ResetTimer();
            }
        }
        if(target != null)
        {
            enemyHp= target.GetComponent<EnemyHp>();
            if (arrived&&enemyHp.corruption_P)
            {
                Debug.Log("6");
                ReturnAnimation();
                target = null;
            }
        }

    }
    void ReturnAnimation()
    {
        //Asuisui
        playerAnimator.SetTrigger("ReFly");
		sword.SetActive(false);
		print("ReFly");
        //--
        ResetTimer();
        isSlashDashing = false;
        isAttacking = false;
        playerMove.CalculateFallbackSpeed();
        isFallBack = true;
        followZoom.m_Width = 50;
        //Tween /*tweener1,*/ tweenCam;
        //tweener1 = playerRigidbody.DOMove(PlayerOriginalPos, 0.5f).OnStart(() => { tween1Playing = true;  });
        //tweenCam = playerVCam.transform.DOLocalRotateQuaternion(Quaternion.Euler(1.8f, 0, 0), 1f).OnStart(() => camTweenPlaying = true).OnComplete(() => camTweenPlaying = false); 
        //if (!camTweenPlaying)
        //{
        //    tweenCam.Play();
        //}
    }
    public void FallBackFinish()
    {
        isFallBack = false;
        arrived = false;
        isSlashDashing = false;
        playerAim.aimmingImage.SetActive(true);
        shieldEffect.SetActive(true);
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
