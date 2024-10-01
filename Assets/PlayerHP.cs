using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerHP : MonoBehaviour
{
    Rigidbody playerRigidbody;
    [SerializeField]
    int maxHp;
    [SerializeField]
    int playerHp;
    [Header("UI")]
    [SerializeField]
    Image HpBar;
    [SerializeField]
    List<Material> playerMat;
    //[SerializeField]
    //GameObject body;
    //bool isRotating= false;
    bool isMuteki = false;

    //float rotationDuration = 0.5f; // Duration of the rotation in seconds
    //private float startTime; // Time when the rotation starts
    //private Vector3 initialRotation; // Initial rotation of the object



    // Start is called before the first frame update
    void Start()
    {
        playerHp = maxHp;
        UpdateHpUI();
        playerRigidbody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        //if (Input.GetKeyDown(KeyCode.Space) && !isRotating)
        //{
        //    isRotating = true;
        //    startTime = Time.time;
        //    playerMat.color = Color.green;
        //    StartCoroutine(MuTeKiTime(0.2f));
        //}
        
        //if (playerRigidbody.velocity.x < -0.2f)
        //{
        //    transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0, 0, 30), 0.5f);
        //}
        //else if (playerRigidbody.velocity.x > 0.2f)
        //{
        //    transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0, 0, -30), 0.5f);
        //}
        //else if (playerRigidbody.velocity.x == 0 && !isRotating)
        //{
        //    transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0, 0, 0), 0.5f);
        //}
        //    // If rotation is active, calculate the rotation angle based on the easeOutSine curve
        //    if (isRotating)
        //{
        //    float elapsedTime = Time.time - startTime;
        //    float angle = Mathf.Lerp(transform.rotation.z, 360f, Mathf.SmoothStep(0f, 1f, elapsedTime / rotationDuration));
        //    if (playerRigidbody.velocity.x <= 0)
        //    {
        //        body.transform.eulerAngles = initialRotation + new Vector3(0f, 0f, angle);
        //    }
        //    else
        //    {
        //        body.transform.eulerAngles = initialRotation - new Vector3(0f, 0f, angle);
        //    }
            

        //    // Stop rotation after completing one full rotation
        //    if (elapsedTime >= rotationDuration)
        //    {
        //        isRotating = false;
        //        playerMat.color = Color.white;
        //    }
        //}
    }
    public void getHit(int damage)
    {
        playerHp -= damage;
        UpdateHpUI();
        StartCoroutine(MuTeKiTime(0.1f));
        playerMat[0].color = Color.red;
        playerMat[1].color = Color.red;
        playerMat[2].color = Color.red;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "EnemyBullet")
        {
            if(!isMuteki)
            getHit(5);
            other.gameObject.SetActive(false);
        }
        if (other.tag == "Block")
        {
            if (!isMuteki)
                getHit(10);
        }
        if(other.tag == "Enemy")
        {
            if (!isMuteki)
                getHit(10);
            Destroy(other.gameObject);
        }
    }
    private void UpdateHpUI()
    {
        float HpAmount = (float)playerHp / (float)maxHp;
        //Debug.Log(HpAmount);
        HpBar.fillAmount = HpAmount;
    }
    IEnumerator MuTeKiTime(float mutekiTime)
    {
        isMuteki = true;
        //Physics.IgnoreLayerCollision(8, 6, true);
        yield return new WaitForSeconds(mutekiTime);
        isMuteki = false;
        //Physics.IgnoreLayerCollision(8, 6, false);
        playerMat[0].color = Color.white;
        playerMat[1].color = Color.white;
        playerMat[2].color = Color.white;
    }
}
