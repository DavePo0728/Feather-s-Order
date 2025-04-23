using AfterimageFX;
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
    public enum SlashState { Idle, Dashing, Arrived, Attacking, FallingBack }
    public SlashState slashState = SlashState.Idle;

    PlayerMove playerMove;
    [SerializeField] CinemachineVirtualCamera playerVCam;
    Rigidbody playerRigidbody;
    [SerializeField] Collider slashCollider;
    [SerializeField] Vector3 slashTarget;
    PlayerAim playerAim;
    Vector3 PlayerOriginalPos;
    SlashDetect slashDetect;
    GameObject shieldEffect;
    GameObject shieldEffectBIG;
    [SerializeField] GameObject slashEffectYellowObject, slashEffectRedObject;
    ParticleSystem slashEffectYellow, slashEffectRed;
    AudioSource slashAudio;
    AudioClip slashClip;
    [SerializeField] CinemachineImpulseSource impulseSource;
    [SerializeField] CinemachineFollowZoom followZoom;
    [SerializeField] GameObject flashImage;

    bool dashCounting=false;
    int hitCounter;
    float SlashTimer;
    float slashCD = 0.2f;
    float maxTime = 1.0f;
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

    private void Awake()
    {
        playerMove = GetComponent<PlayerMove>();
        playerAim = GetComponent<PlayerAim>();
        slashDetect = GameObject.Find("SlashCollider").GetComponent<SlashDetect>();
        playerRigidbody = GetComponent<Rigidbody>();
        shieldEffect = GameObject.Find("MagicShieldYellow");
        shieldEffectBIG = GameObject.Find("MagicShieldYellow_1");
        slashEffectYellow = slashEffectYellowObject.GetComponent<ParticleSystem>();
        slashEffectRed = slashEffectRedObject.GetComponent<ParticleSystem>();
        slashAudio = slashEffectYellowObject.GetComponent<AudioSource>();
        slashClip = Resources.Load<AudioClip>("Sound/Slash01");
        hitCounter = 0;
    }

    public void GetSlashInput(InputAction.CallbackContext context)
    {
        if (context.performed && slashState == SlashState.Idle&&playerAim.isLocked)
        {
            if (playerAim.CheckLockedEnemy())
            {
                
                if(playerAim._lockedEnemy.CompareTag("Enemy"))
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
                    playerAim.aimmingImage.SetActive(false);
                    playerAim.FarLockImage.SetActive(false);
                    return;
                }
                if (enemyHp.corrupted && !enemyHp.corruption_P)
                {
                    dashCounting = true;
                    playerAnimator.SetTrigger("Dash");
                    sword.SetActive(true);
                    DashToEnemy();
                    playerAim.aimmingImage.SetActive(false);
                    playerAim.FarLockImage.SetActive(false);
                }
            }
        }
        if(context.canceled)
        {
            if (slashState == SlashState.Attacking && dashCounting == false)
            {
                ReturnAnimation();
            }
        }
    }

    public void GetSlashAttackInput(InputAction.CallbackContext context)
    {
        if (context.performed && slashState == SlashState.Attacking && SlashTimer >= slashCD)
        {
            if (hitCounter < 3)
            {
                slashCD = 0.2f;
                Invoke("TriggerSlash", 0.1f);
                hitCounter++;
                SlashTimer = 0;
                attackTimer = 0;

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
                }
            }
            else
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

    public void TriggerSlash4()
    {
        clothDB.enabled = false;
        Vibrate(0.5f, 0.5f, 0.05f);
        slashAudio.Play();
        slashCollider.enabled = true;
        Invoke("InactiveCollider", 0.1f);
        flashImage.SetActive(true);
        Invoke("InactiveFlashImage", 0.01f);
        switch (hitCounter)
        {
            case 0:
                slashEffectYellowObject.transform.localScale = new Vector3(2.72f, -2.72f, 2.72f);
                break;
            case 1:
                slashEffectYellowObject.transform.localScale = new Vector3(2.72f, 2.72f, 2.72f);
                break;
            case 2:
                slashEffectYellowObject.transform.localScale = new Vector3(2.72f, -2.72f, 2.72f);
                break;
            case 3:
                slashEffectYellowObject.transform.localScale = new Vector3(2.72f, 2.72f, 2.72f);
                break;
        }
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
        slashEffectYellowObject.SetActive(true);
        slashEffectYellow.Play();
        Shake(0.5f);
        Time.timeScale = 0.1f;
        Invoke("TimeScaleNormal", 0.01f);
        Invoke("DelayDetect", 0.05f);
        SetAttack();
    }

    void DashToEnemy()
    {
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
                GetComponent<AfterimageController>().StartDash();
                tweener = playerRigidbody.DOMove(slashTarget, 0.5f).OnComplete(() =>
                {
                    slashState = SlashState.Arrived;
                    playerRigidbody.velocity = Vector3.zero;
                    isCounting = true;
                    followZoom.m_Width = 0;
                    TriggerSlash();
                    
                    IsReturnAnimation = false;
                    playerAnimator.SetBool("OnAttack", true);
                    attackTimer = 0;
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
        slashState = SlashState.Dashing;
        playerRigidbody.velocity = Vector3.zero;
        if (target != null)
        {
            if (target.CompareTag("Enemy"))
            {
                slashTarget = target.transform.Find("DashPoint").position;
                Vector3 lastTargetPos = slashTarget;
                GetComponent<AfterimageController>().StartDash();
                tweener = playerRigidbody.DOMove(slashTarget, 0.5f).OnUpdate(() =>
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
                }).OnComplete(() =>
                {
                    slashState = SlashState.Arrived;
                    playerRigidbody.velocity = Vector3.zero;
                    playerVCam.m_Lens.FieldOfView = 15;
                    followZoom.m_Width = 0;
                    IsReturnAnimation = false;
                    shieldEffect.SetActive(false);
                    shieldEffectBIG.SetActive(false);

                    TriggerSlash();
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

    void SetAttack() { 
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
            SlashTimer += Time.deltaTime;
            attackTimer += Time.deltaTime;
            if (attackTimer >= 0.5f)
            {
                playerAnimator.SetBool("OnAttack", false);
                clothDB.enabled = true;
            }
            if (attackTimer > maxTime)
            {
                ReturnAnimation();
                ResetTimer();
            }
        }
        Debug.Log("SlashState: " + slashState);
        if (slashState == SlashState.FallingBack|| slashState == SlashState.Idle && playerAnimator.GetCurrentAnimatorStateInfo(0).IsName("Base Layer.Stand__Idle"))
        {
            //playerAnimator.SetTrigger("ReFly");
            playerAnimator.Play("ReFly");
        }
    }

    void ReturnAnimation()
    {
        ResetTimer();
        slashState = SlashState.FallingBack;
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
            ReturnAnimation();
            return;
        }
        if (slashState == SlashState.Arrived|| slashState == SlashState.Attacking)
        {
            ReturnAnimation();
            return;
        }
    }

    void DelayDetect()
    {
        if (!playerAim._lockedEnemy.CompareTag("Enemy"))
        {
            ReturnAnimation();
        }
    }

    void InactiveFlashImage() => flashImage.SetActive(false);
    void TimeScaleNormal() => Time.timeScale = 1;
    void InactiveCollider() => slashCollider.enabled = false;
    void Shake(float intensity) => impulseSource.GenerateImpulseWithForce(intensity);
    void ResetTimer() { isCounting = false; attackTimer = 0f; }
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
}
