using Cinemachine;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerSlashAttack : MonoBehaviour
{
    [SerializeField]
    CinemachineVirtualCamera playerVCam;
    Rigidbody playerRigidbody;
    public bool isSlashDashing;
    public bool arrived = false;
    bool isAttacking;
    [SerializeField]
    Collider slashCollider;
    [SerializeField]
    Vector3 slashtarget;
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
    int hitCounter;
    float SlashTimer;
    float slashCD = 0.3f;
    float maxTime = 3.0f; // 限制的時間 (1 秒)
    float attackTimer = 0f; // 計時器
    bool isCounting = false; // 是否計時中
    bool attackTriggered = false;
    bool tweenPlaying = false;
    bool tween1Playing = false;
    bool camTweenPlaying = false;
    bool camtween1Playing = false;
    private void Awake()
    {
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
        slashAudio = GetComponent<AudioSource>();
        slashClip = Resources.Load<AudioClip>("Sound/Slash01");
        hitCounter = 0;
    }
    public void GetSlashInput(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (isSlashDashing == false&&arrived==false)
            {
                if (!playerAim._lockedEnemy.CompareTag("Enemy"))
                {
                    return;
                }
                    
                PlayerOriginalPos = transform.position;
                PlayerOriginalPos.z = 13f;
                DashToEnemy();
                playerAim.aimmingImage.SetActive(false);
                playerAim.lockImage.SetActive(false);
            }
            else
            {
                if (!playerAim._lockedEnemy.CompareTag("Enemy"))
                {
                    return;
                }
                DashToEnemy();
            }
        }
    }
    public void GetSlashAttackInput(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (isAttacking&&SlashTimer>=slashCD)
            {
                if (hitCounter < 3)
                {
                    TriggerSlash();
                    hitCounter++;
                    SlashTimer = 0;
                    attackTimer = 0;
                }
                else if (hitCounter >= 3)
                {
                    slashAudio.PlayOneShot(slashClip);
                    slashCollider.enabled = true;
                    slashEffectRedObject.SetActive(true);
                    slashEffectRed.Play();
                    hitCounter = 0;
                    Shake(1.0f);
                    Time.timeScale = 0.1f;
                    Invoke("TimeScaleNormal", 0.02f);
                    SlashTimer = 0;
                    attackTimer = 0;
                }
            }
        }
    }
    public void TriggerSlash() 
    {
        slashAudio.PlayOneShot(slashClip);
        slashCollider.enabled = true;
        Invoke("InactiveCollider", 0.1f);
        slashEffectYellowObject.SetActive(true);
        slashEffectYellow.Play();
        Shake(0.5f);
        Time.timeScale = 0.1f;
        Invoke("TimeScaleNormal", 0.01f);
    }
    void DashToEnemy()
    {
        Tween tweener,tweenCam;

        
        slashtarget = playerAim._lockedEnemy.transform.GetChild(7).transform.position;
        tweener = playerRigidbody.DOMove(slashtarget, 0.5f).OnStart(() => tweenPlaying = true);
        tweenCam = playerVCam.transform.DOLocalRotateQuaternion(Quaternion.Euler(5, -15, 0), 0.5f).OnStart(() => camTweenPlaying = true).OnComplete(() => camTweenPlaying = false);
        if (isSlashDashing ==false&&arrived==false)
        {
            isSlashDashing = true;
            if (!tweenPlaying)
            {
                tweener.Play();
            }
            if (!camTweenPlaying)
            {
                tweenCam.Play();
            }
            tweener.OnComplete(() =>
            {
                tweenPlaying = false;
                arrived = true;
                //playerVCam.m_Lens.FieldOfView = 15;
                Invoke("SetAttack", 0.3f);
                shieldEffect.SetActive(false);
                isCounting = true;
                followZoom.m_Width = 0;
                TriggerSlash();
                //Invoke("TriggerSlash");
            });
            //transform.position = new Vector3(transform.position.x, transform.position.y, slashtarget.position.z);

        }
        else if(arrived) 
        {
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
                ReturnAnimation(); // 超時執行返回動畫
                ResetTimer();
            }
        }
    }
    void ReturnAnimation()
    {
        isSlashDashing = true;
        Tween tweener1, tweenCam;
        tweener1 = playerRigidbody.DOMoveZ(PlayerOriginalPos.z, 1f).OnStart(() => tween1Playing = true);
        tweenCam = playerVCam.transform.DOLocalRotateQuaternion(Quaternion.Euler(1.8f, 0, 0), 1f).OnStart(() => camTweenPlaying = true).OnComplete(() => camTweenPlaying = false);
        if (!tween1Playing)
        {
            tweener1.Play();
            //Debug.Log("Playing");
        }
        if (!camTweenPlaying)
        {
            tweenCam.Play();
        }
        tweener1.OnComplete(() =>
        {
            tween1Playing = false;
            arrived = false;
            //playerVCam.m_Lens.FieldOfView = 30;
            //transform.position = new Vector3(transform.position.x, transform.position.y, PlayerOriginalPos.z);
            isSlashDashing = false;
            isAttacking = false;
            playerAim.aimmingImage.SetActive(true);
            shieldEffect.SetActive(true);  
            followZoom.m_Width = 50;
        });
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
