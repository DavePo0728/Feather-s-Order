using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMove : MonoBehaviour
{
    Rigidbody playerRigidbody;
    [SerializeField]
    float speed, maxVelocity;
    //Dash
    [SerializeField]
    float dashForce;
    bool canDash = true;
    bool isDashing = false;
    float dashingTime = 0.2f;
    float dashCooldown = 2f;
    //[SerializeField]
    //Animator bodyAnimator;
    bool isRotating = false;
    [SerializeField]
    Material playerMat;
    [SerializeField]
    GameObject body,flyinglean;
    float rotationDuration = 0.5f; // Duration of the rotation in seconds
    private float startTime; // Time when the rotation starts
    private Vector3 initialRotation; // Initial rotation of the object
    private Vector2 movementInput;
    private void Awake()
    {
        playerRigidbody = GetComponent<Rigidbody>();
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
            if (canDash)
            {
                StartCoroutine(OnDash());
                startTime = Time.time;
                playerMat.color = Color.green;
                StartCoroutine(MuTeKiTime(0.2f));
            }
        }
    }
    // Update is called once per frame
    void FixedUpdate()
    {
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
}
