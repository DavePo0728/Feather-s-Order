using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem;

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
    TMP_Text HPText;
    [SerializeField]
    GameObject GameOverUI,gameOverText,GameClearText;
    public TMP_Text mpText;
    public GameObject heal, healBack;
    public float maxMp;
    public float currentMp;
    public float mpCost;
    //[SerializeField]
    //GameObject body;
    //bool isRotating= false;
    bool isMuteki = false;

    //float rotationDuration = 0.5f; // Duration of the rotation in seconds
    //private float startTime; // Time when the rotation starts
    //private Vector3 initialRotation; // Initial rotation of the object

    private void Awake()
    {
        Time.timeScale = 1;
        currentMp = 0;
    }

    // Start is called before the first frame update
    void Start()
    {
        playerHp = maxHp;
        UpdateHpUI();
        playerRigidbody = GetComponent<Rigidbody>();
        UpdateMpUI();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void getHit(int damage)
    {
        playerHp -= damage;
        UpdateHpUI();
        StartCoroutine(MuTeKiTime(0.1f));
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "EnemyBullet")
        {
            if(!isMuteki)
            getHit(5);
            //other.gameObject.SetActive(false);
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
            enemyHp _enemyHp = other.GetComponent<enemyHp>();
            _enemyHp.DeathEffect();
        }
    }
    private void UpdateHpUI()
    {
        float HpAmount = (float)playerHp / (float)maxHp;
        //Debug.Log(HpAmount);
        HpBar.fillAmount = HpAmount;
        HPText.text = playerHp.ToString();
        if (playerHp <= 0)
        {
            GameOver();
        }
    }
    public void UpdateMpUI()
    {
        
        //Debug.Log(HpAmount);
        if (currentMp >= maxHp)
        {
            currentMp = maxHp;
        }
        mpText.text = currentMp.ToString();
        if(currentMp >= mpCost)
        {
            heal.SetActive(true);
            healBack.SetActive(false);
        }
        else
        {
            heal.SetActive(false);
            healBack.SetActive(true);
        }
    }
    public void GetMp(float amount)
    {
        currentMp += amount;
        UpdateMpUI();
    }
    public void GetHealInput(InputAction.CallbackContext context)
    {

        if (context.performed&&currentMp>=mpCost)
        {
            Heal(30);
            currentMp -= mpCost;
            UpdateMpUI();
        }
    }
    public void Heal(int healAmount)
    {
        playerHp += healAmount;
        if (playerHp > maxHp)
        {
            playerHp = maxHp;
        }
        UpdateHpUI();
    }
    void GameOver()
    {
        GameOverUI.SetActive(true);
        gameOverText.SetActive(true);
        GameClearText.SetActive(false);
        Time.timeScale = 0;
    }
    IEnumerator MuTeKiTime(float mutekiTime)
    {
        isMuteki = true;
        //Physics.IgnoreLayerCollision(8, 6, true);
        yield return new WaitForSeconds(mutekiTime);
        isMuteki = false;
        //Physics.IgnoreLayerCollision(8, 6, false);
    }
}
