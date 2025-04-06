using Cinemachine;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerParkourMove : MonoBehaviour
{
    [SerializeField]
    CinemachineVirtualCamera playerVCam;
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
    float moveHspeed, moveVspeed, maxVelocity, moveHspeedMultiplier, moveVspeedMultiplier;
    [SerializeField]
    float zSpeed, oriZSpeed;
    [SerializeField]
    float leanAngle, lerpTime, lerpMultiplier;
    [SerializeField]
    float dutchMultiplier;
    [SerializeField]
    float fallBackTime;
    float fallbackspeed;
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
    GameObject body;
    float rotationDuration = 0.5f; // Duration of the rotation in seconds
    float rotateStartTime; // Time when the rotation starts
    Vector3 initialRotation; // Initial rotation of the object
    Vector2 movementInput;
    float leanInput;
    [Header("Effect")]
    Vector3 movement;
    private void Awake()
    {
        playerRigidbody = GetComponent<Rigidbody>();
        zSpeed = oriZSpeed;
        playerVCamFramingTransposer = playerVCam.GetCinemachineComponent<CinemachineFramingTransposer>();
    }
    // Start is called before the first frame update
    void Start()
    {
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
            }
        }
    }
    // Update is called once per frame
    void FixedUpdate()
    {
        if (!isDashing)
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0, 0, 90 * leanInput), lerpTime * Time.deltaTime * lerpMultiplier);
            if (movementInput.x > 0.2f || movementInput.y > 0.2f || movementInput.x < -0.2f || movementInput.y < -0.2f)
            {
                movement = new Vector3(movementInput.x, movementInput.y, 0);
            }
            else
            {
                movement = Vector3.zero;
            }
            if (transform.position.z > 13)
            {
                playerRigidbody.velocity = new Vector3(movement.x * moveHspeed, movement.y * moveVspeed, -fallbackspeed);
                Vector2 clampedXY = Vector2.ClampMagnitude(new Vector2(playerRigidbody.velocity.x, playerRigidbody.velocity.y), maxVelocity);
                playerRigidbody.velocity = new Vector3(clampedXY.x, clampedXY.y, playerRigidbody.velocity.z);
            }
            else
            {
                playerRigidbody.velocity = new Vector3(movement.x * moveHspeed, movement.y * moveVspeed, 0);
                playerRigidbody.velocity = Vector3.ClampMagnitude(playerRigidbody.velocity, maxVelocity);
            }
            if (playerRigidbody.velocity.x < -0.2f)
            {
                transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0, 0, leanAngle), lerpTime * Time.deltaTime * lerpMultiplier);

                playerVCam.m_Lens.Dutch = Mathf.Lerp(playerVCam.m_Lens.Dutch, -10, 0.2f * Time.deltaTime * 8);
            }
            else if (playerRigidbody.velocity.x > 0.2f)
            {
                transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0, 0, -leanAngle), lerpTime * Time.deltaTime * lerpMultiplier);
                playerVCam.m_Lens.Dutch = Mathf.Lerp(playerVCam.m_Lens.Dutch, 10, 0.2f * Time.deltaTime * 8);

            }
            else if (playerRigidbody.velocity.x == 0 && !isDashing)
            {
                playerVCam.m_Lens.Dutch = Mathf.Lerp(playerVCam.m_Lens.Dutch, 0, 0.2f * Time.deltaTime * 8);
            }
            if (isRotating)
            {
                transform.rotation = Quaternion.Euler(0, 0, 0);
                float elapsedTime = Time.time - rotateStartTime;
                float angle = Mathf.Lerp(transform.rotation.z, 360f, Mathf.SmoothStep(0f, 1f, elapsedTime / rotationDuration));
                if (playerRigidbody.velocity.x >= 0)
                {

                    body.transform.eulerAngles = initialRotation + new Vector3(0f, 0f, angle);
                }
                else
                {

                    body.transform.eulerAngles = initialRotation - new Vector3(0f, 0f, angle);
                }
                isRotating = false;

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
        canDash = false;
        isDashing = true;
        isRotating = true;
        playerVCamFramingTransposer.m_SoftZoneWidth = 0.8f;
        playerVCamFramingTransposer.m_XDamping = 1.2f;
        leanAngle = 75f;

        if (playerRigidbody.velocity.x != 0)
        {
            playerRigidbody.AddForce(playerRigidbody.velocity * dashForce, ForceMode.Impulse);
        }
        StartCoroutine(MuTeKiTime(0.2f));
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
}
