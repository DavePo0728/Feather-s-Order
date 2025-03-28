using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Cinemachine;
using DG.Tweening;


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
    [SerializeField]
    float leanAngle, lerpTime, lerpMultiplier;
    [SerializeField]
    float dutchMultiplier;
    
    //Dash
    bool canDash = true;
    bool isDashing = false;
    float dashingTime = 0.2f;
    float dashCooldown = 0.2f;
    bool isRotating = false;
    AudioSource dashSound;
    [SerializeField]
    List<Material> playerMat;
    [SerializeField]
    GameObject body,flyinglean;
    float rotationDuration = 0.5f; // Duration of the rotation in seconds
    float rotateStartTime; // Time when the rotation starts
    //float leanStartTime;
    Vector3 initialRotation; // Initial rotation of the object
    Vector2 movementInput;
    float leanInput;
    //bool manualLean = true;
    [Header("Effect")]
    [SerializeField]
    GameObject BounceExpolsion;
    [SerializeField]
    ParticleSystem speedLine;
    Vector3 movement;
    bool isBouncing = false;
    [SerializeField]
    float goldspeedMulti;
    PlayerSlashAttack playerSlashAttack;




    private void Awake()
    {
        playerRigidbody = GetComponent<Rigidbody>();
        zSpeed = oriZSpeed;
        playerSlashAttack = GetComponent<PlayerSlashAttack>();
        playerVCamFramingTransposer = playerVCam.GetCinemachineComponent<CinemachineFramingTransposer>();
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
        if (context.performed)
        {
            if (canDash/*&&currentEnergy>=20&&!isOutBurst*/)
            {
                rotateStartTime = Time.time;
                StartCoroutine(OnDash());
                DashAudioSource.Play();
                //currentEnergy -= 20;
                //UpdateUI();
               // timeSinceLastEnergyUse = 0f;   // 重置時間計數器
                //isRegening = false;
            }
        }
    }
    public void GetBounce(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            rotateStartTime = Time.time;
            isBouncing = true;
            isRotating = true;
            StartCoroutine(MuTeKiTime(0.2f));
            //currentEnergy -= 20;
            //UpdateUI();
            //timeSinceLastEnergyUse = 0f;   // 重置時間計數器
            //isRegening = false;
        }
    }

    
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
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0, 0, 90 * leanInput), lerpTime * Time.deltaTime * lerpMultiplier);
            if (movementInput.x>0.2f||movementInput.y>0.2f|| movementInput.x < -0.2f || movementInput.y < -0.2f)
            {
                movement = new Vector3(movementInput.x, movementInput.y, 0);
            }
            else
            {
                movement = Vector3.zero;
            }
            if(!playerSlashAttack.isSlashDashing)
            {
                playerRigidbody.velocity = new Vector3(movement.x * moveHspeed, movement.y * moveVspeed, 0);
                playerRigidbody.velocity = Vector3.ClampMagnitude(playerRigidbody.velocity, maxVelocity);
                //Debug.Log(playerRigidbody.velocity);
                // flying lean
                if (playerRigidbody.velocity.x < -0.2f)
                {
                    //leanStartTime = Time.time;
                    transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0, 0, leanAngle), lerpTime * Time.deltaTime * lerpMultiplier);
                    //float t = (Time.time - rotateStartTime) / 2.0f;
                    //vCam.m_Lens.Dutch = Mathf.SmoothStep(vCam.m_Lens.Dutch, -10, t);
                    playerVCam.m_Lens.Dutch = Mathf.Lerp(playerVCam.m_Lens.Dutch, -10, 0.2f * Time.deltaTime * 8);
                    SceneVCam.m_Lens.Dutch = Mathf.Lerp(playerVCam.m_Lens.Dutch, -10, 0.2f * Time.deltaTime * 8);
                }
                else if (playerRigidbody.velocity.x > 0.2f)
                {
                    //leanStartTime = Time.time;
                    transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0, 0, -leanAngle), lerpTime * Time.deltaTime * lerpMultiplier);
                    //float t = (Time.time - rotateStartTime) / 2.0f;
                    //vCam.m_Lens.Dutch = Mathf.SmoothStep(vCam.m_Lens.Dutch, 10, t);
                    playerVCam.m_Lens.Dutch = Mathf.Lerp(playerVCam.m_Lens.Dutch, 10, 0.2f * Time.deltaTime * 8);
                    SceneVCam.m_Lens.Dutch = Mathf.Lerp(playerVCam.m_Lens.Dutch, 10, 0.2f * Time.deltaTime * 8);

                }
                else if (playerRigidbody.velocity.x == 0 && !isDashing)
                {
                    //leanStartTime = Time.time;
                    //transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0, 0, 0), lerpTime * Time.deltaTime * lerpMultiplier);
                    //float t = (Time.time - rotateStartTime) / 2.0f;
                    //vCam.m_Lens.Dutch = Mathf.SmoothStep(vCam.m_Lens.Dutch, 0, t);
                    playerVCam.m_Lens.Dutch = Mathf.Lerp(playerVCam.m_Lens.Dutch, 0, 0.2f * Time.deltaTime * 8);
                    SceneVCam.m_Lens.Dutch = Mathf.Lerp(playerVCam.m_Lens.Dutch, 0, 0.2f * Time.deltaTime * 8);

                }
            }
            if (isRotating)
            {
                //Debug.Log(leanInput);
                //initialRotation = transform.rotation.EulerAngles();
                transform.rotation = Quaternion.Euler(0, 0, 0);
                float elapsedTime = Time.time - rotateStartTime;
                float angle = Mathf.Lerp(transform.rotation.z, 360f, Mathf.SmoothStep(0f, 1f, elapsedTime / rotationDuration));
                if (playerRigidbody.velocity.x >= 0)
                {

                    body.transform.eulerAngles = initialRotation + new Vector3(0f, 0f, angle);
                    //playerAnimator.SetTrigger("RightRoll");
                }
                else
                {

                    //playerAnimator.SetTrigger("LeftRoll");
                    body.transform.eulerAngles = initialRotation - new Vector3(0f, 0f, angle);
                }
                isRotating = false;

            }

        }
    }
    
    IEnumerator OnDash()
    {
        canDash = false;
        isDashing = true;
        isRotating = true;
        playerVCamFramingTransposer.m_SoftZoneWidth = 0.8f;
        playerVCamFramingTransposer.m_XDamping = 1.2f;
        leanAngle = 75f;
        //if (playerRigidbody.velocity.x < -0.2f)
        //{
        //    transform.DORotate(new Vector3(0, 0, 40f), 0.05f);
        //}
        //else if (playerRigidbody.velocity.x > 0.2f)
        //{
        //    transform.DORotate(new Vector3(0, 0, -40f), 0.05f);
        //}
        
        if (playerRigidbody.velocity.x != 0)
        {
            playerRigidbody.AddForce(playerRigidbody.velocity * dashForce, ForceMode.Impulse);
        }
        
        yield return new WaitForSeconds(dashingTime);
        isDashing = false;
        DOTween.To(() => leanAngle, x => leanAngle = x, 30f, 1.5f).Play();
        DOTween.To(() => playerVCamFramingTransposer.m_XDamping, x => playerVCamFramingTransposer.m_XDamping = x, 0.4f, 0.4f).Play();
        DOTween.To(() => playerVCamFramingTransposer.m_SoftZoneWidth, x => playerVCamFramingTransposer.m_SoftZoneWidth = x, 0.4f, 0.4f).Play();
        StartCoroutine(StartCountdown());
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
        playerMat[0].color = Color.green;
        playerMat[1].color = Color.green;
        playerMat[2].color = Color.green;
        yield return new WaitForSeconds(mutekiTime);
        //isMuteki = false;
        Physics.IgnoreLayerCollision(8, 6, false);
        playerMat[0].color = Color.white;
        playerMat[1].color = Color.white;
        playerMat[2].color = Color.white;
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "EnemyGoldBullet"&&isBouncing)
        {
            Debug.Log("Gold");
            GoldBulletMove gold = other.GetComponent<GoldBulletMove>();
            gold.speed *= goldspeedMulti;
            gold.bounceBack = true;
            Vector3 spawnEffectPosition = new Vector3(transform.position.x, transform.position.y, transform.position.z - 1f);
            GameObject explosionInstance = Instantiate(BounceExpolsion, transform);
            Destroy(explosionInstance, 1f);
        }
    }
    //private void UpdateUI()
    //{
    //    float EnergyAmount = (float)currentEnergy / (float)maxEnergy;
    //    //Debug.Log(HpAmount);
    //    energyBarImage.fillAmount = EnergyAmount;
    //}
    //private void StartEnergyRegen()
    //{
    //    isRegening = true;
    //}
    //private void RegenerateEnergy()
    //{
    //    currentEnergy += energyRegenRate * Time.deltaTime;
    //    UpdateUI();
    //    currentEnergy = Mathf.Min(currentEnergy, maxEnergy);  // 確保能量不超過最大值

    //    // 如果能量已經回滿，停止回復
    //    if (currentEnergy >= maxEnergy)
    //    {
    //        isRegening = false;
    //        isOutBurst = false;
    //        regenDelay = 1f;
    //    }
    //}
    //public void GetBoost(InputAction.CallbackContext context)
    //{
    //    var emission = speedLine.emission;
    //    if (context.performed&&currentEnergy >=15&&!isOutBurst)
    //    {
    //        currentEnergy -= 5;
    //        UpdateUI();
    //        isRegening = false;
    //        isBoosting = true;

    //        emission.rateOverTime = 500f;
    //    }
    //    if (context.canceled)
    //    {
    //        isBoosting = false;
    //        emission.rateOverTime = 50f;
    //    }
    //}
    //public void GetBrake(InputAction.CallbackContext context)
    //{
    //    var emission = speedLine.emission;
    //    if (context.performed&&currentEnergy >= 15&&!isOutBurst)
    //    {
    //        currentEnergy -= 15;
    //        UpdateUI();
    //        isRegening = false;
    //        isBraking = true;
    //        emission.rateOverTime = 0f;
    //    }
    //    if (context.canceled)
    //    {
    //        isBraking = false;
    //        emission.rateOverTime = 100f;
    //    }
    //}
}
