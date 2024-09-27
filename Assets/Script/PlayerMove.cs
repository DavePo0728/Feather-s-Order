using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerMove : MonoBehaviour
{
    [Header("UI")]
    [SerializeField]
    Image energyBarImage;
    [Header("Energy Data")]
    [SerializeField]
    float currentEnergy;
    [SerializeField]
    float maxEnergy;
    float energyRegenRate = 50;
    float timeSinceLastEnergyUse = 0f;   // 距離上次使用能量的時間
    float regenDelay = 1f;     // 回復能量的延遲時間 (1秒)
    bool isRegening = false;  // 是否正在回復能量
    [Space(height: 20)]

    [SerializeField]
    float dashForce;
    Rigidbody playerRigidbody;
    [SerializeField]
    float speed, maxVelocity;

    //Dash
    bool canDash = true;
    bool isDashing = false;
    float dashingTime = 0.2f;
    float dashCooldown = 0.5f;
    bool isRotating = false;
    [SerializeField]
    Material playerMat;
    [SerializeField]
    GameObject body,flyinglean;
    float rotationDuration = 0.5f; // Duration of the rotation in seconds
    private float startTime; // Time when the rotation starts
    private Vector3 initialRotation; // Initial rotation of the object
    private Vector2 movementInput;
    [Header("Effect")]
    [SerializeField]
    GameObject BounceExpolsion;

    bool isBoosting = false;
    bool isBraking = false;
    EnemyBulletData bulletData;

    private void Awake()
    {
        playerRigidbody = GetComponent<Rigidbody>();
        bulletData = Resources.Load<EnemyBulletData>("BulletData/NormalBullet");
    }
    // Start is called before the first frame update
    void Start()
    {
        currentEnergy = maxEnergy;
        UpdateUI();
    }
    public void GetMove(InputAction.CallbackContext context)
    {
        movementInput = context.ReadValue<Vector2>(); 
    }
    public void GetDash(InputAction.CallbackContext context)
    {

        if (context.performed)
        {
            if (canDash&&currentEnergy>=20)
            {
                StartCoroutine(OnDash());
                startTime = Time.time;
                playerMat.color = Color.green;
                StartCoroutine(MuTeKiTime(0.2f));
                currentEnergy -= 20;
                UpdateUI();
                timeSinceLastEnergyUse = 0f;   // 重置時間計數器
                isRegening = false;
            }
        }
    }
    public void GetBoost(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            isBoosting = true;
        }
        if (context.canceled)
        {
            isBoosting = false;
        }
    }
    public void GetBrake(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            isBraking = true;
        }
        if (context.canceled)
        {
            isBraking = false;
        }
    }
    // Update is called once per frame
    void FixedUpdate()
    {
        // 計時器每幀更新
        timeSinceLastEnergyUse += Time.deltaTime;
        // 如果能量不是滿的並且已經超過回復延遲時間，開始回復能量
        if (!isRegening && currentEnergy < maxEnergy && timeSinceLastEnergyUse >= regenDelay)
        {
            StartEnergyRegen();
        }

        // 如果正在回復能量，逐漸增加能量
        if (isRegening)
        {
            RegenerateEnergy();
        }
        if (isRotating)
        {
            transform.rotation = Quaternion.Euler(0, 0, 0);
            float elapsedTime = Time.time - startTime;
            float angle = Mathf.Lerp(transform.rotation.z, 360f, Mathf.SmoothStep(0f, 1f, elapsedTime / rotationDuration));
            if (playerRigidbody.velocity.x <= 0)
            {
                body.transform.eulerAngles = initialRotation + new Vector3(0f, 0f, angle);
            }
            else
            {
                body.transform.eulerAngles = initialRotation - new Vector3(0f, 0f, angle);
            }


            // Stop rotation after completing one full rotation
            if (elapsedTime >= rotationDuration)
            {
                isRotating = false;
                playerMat.color = Color.white;
                //body.transform.rotation = transform.rotation;
            }
        }
        if (!isDashing)
        {
            Vector3 movement = new Vector3(movementInput.x, movementInput.y, 0);
            playerRigidbody.velocity = movement * speed;
            playerRigidbody.velocity = Vector3.ClampMagnitude(playerRigidbody.velocity, maxVelocity);
            // flying lean
            if (playerRigidbody.velocity.x < -0.2f)
            {
                transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0, 0, 30), 0.5f);
            }
            else if (playerRigidbody.velocity.x > 0.2f)
            {
                transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0, 0, -30), 0.5f);
            }
            else if (playerRigidbody.velocity.x == 0 && !isDashing)
            {
                transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0, 0, 0), 0.5f);
            }
        }
        if (isBoosting&&!isBraking)
        {
            
            bulletData.speed = 2f;
            Debug.Log("Boost"+bulletData.speed);
        }
        if (isBraking&&!isBoosting)
        {
                bulletData.speed = 0.3f;
            Debug.Log("Break" + bulletData.speed);
        }
        if (!isBoosting && !isBraking)
        {
            bulletData.speed = 0.5f;
        }

    }
    IEnumerator OnDash()
    {
        Debug.Log("dash");
        canDash = false;
        isDashing = true;
        isRotating = true;
        if (playerRigidbody.velocity.x != 0)
        {
            playerRigidbody.AddForce(playerRigidbody.velocity * dashForce, ForceMode.Impulse);
        }
        
        yield return new WaitForSeconds(dashingTime);
        isDashing = false;
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
            //DashUI.fillAmount = currentValue;
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
        playerMat.color = Color.white;
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "EnemyGoldBullet"&&isDashing)
        {
            GoldBulletMove gold = other.GetComponent<GoldBulletMove>();
            gold.speed *= -1f;
            gold.bounceBack = true;
            Vector3 spawnEffectPosition = new Vector3(transform.position.x, transform.position.y, transform.position.z - 1f);
            GameObject explosionInstance = Instantiate(BounceExpolsion, spawnEffectPosition, Quaternion.identity);
            Destroy(explosionInstance, 2f);
        }
    }
    private void UpdateUI()
    {
        float EnergyAmount = (float)currentEnergy / (float)maxEnergy;
        //Debug.Log(HpAmount);
        energyBarImage.fillAmount = EnergyAmount;
    }
    private void StartEnergyRegen()
    {
        isRegening = true;
    }
    private void RegenerateEnergy()
    {
        currentEnergy += energyRegenRate * Time.deltaTime;
        UpdateUI();
        currentEnergy = Mathf.Min(currentEnergy, maxEnergy);  // 確保能量不超過最大值

        // 如果能量已經回滿，停止回復
        if (currentEnergy >= maxEnergy)
        {
            isRegening = false;
        }
    }
}
