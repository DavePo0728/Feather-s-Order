using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem;
using Cinemachine;

public class PlayerHP : MonoBehaviour
{
    ScenesManager scenesManager;
    public BulletGraze bulletGraze;
    Rigidbody playerRigidbody;
    [SerializeField]
    int maxHp;
    [SerializeField]
    int playerHp;
    [Header("UI")]
    [SerializeField]
    Image hpBarImage;
    [SerializeField]
    TMP_Text hpText;
    [SerializeField]
    GameObject gameOverUI;
    [SerializeField]
    GameObject hpDamageImage;
    [SerializeField]
    CinemachineImpulseSource impulseSource;
    [SerializeField]
    bool debug;

    AudioSource hurtAudioSource;
    AudioClip hurtClip1, hurtClip2;

    //[SerializeField]
    //GameObject body;
    //bool isRotating= false;
    public bool isMuteki = false;

    //float rotationDuration = 0.5f; // Duration of the rotation in seconds
    //private float startTime; // Time when the rotation starts
    //private Vector3 initialRotation; // Initial rotation of the object

    private void Awake()
    {
        Time.timeScale = 1;
        hurtAudioSource = GetComponent<AudioSource>();
        hurtClip1 = Resources.Load<AudioClip>("Sound/PlayerGetHit01");
        hurtClip2 = Resources.Load<AudioClip>("Sound/PlayerGetHit02");
        scenesManager = GameObject.Find("SceneManager").GetComponent<ScenesManager>();
    }

    // Start is called before the first frame update
    void Start()
    {
        if (debug)
        {
            maxHp = 9999;
            playerHp = maxHp;
        }
        else
        {
            maxHp = 100;
            playerHp = maxHp;
        }
        UpdateHpUI();
        playerRigidbody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void getHit(int damage)
    {
        int temp = Random.Range(0, 2);
        switch (temp)
        {
            case 0:
                hurtAudioSource.PlayOneShot(hurtClip1);
                break;
            case 1:
                hurtAudioSource.PlayOneShot(hurtClip2);
                break;
        }
        Vibrate(0.5f,0.5f,0.1f);
        hpDamageImage.SetActive(true);
        Invoke("InactiveFlashImage", 0.02f);
        Shake(0.5f);
        playerHp -= damage;
        UpdateHpUI();
        bulletGraze.UpdateGrazeEnergyOutside(10);
    }
    void InactiveFlashImage()
    {
        hpDamageImage.SetActive(false);
    }
    private void OnTriggerEnter(Collider other)
    {
        //Debug.Log("來自的物件：" + other.gameObject.name);
        //Debug.Log("被觸發的 Collider：" + other.name);
        if (other.tag == "EnemyBullet")
        {
            if(!isMuteki)
            getHit(10);
            //Debug.Log("hit");
            //other.gameObject.SetActive(false);
        }
        if (other.tag == "Block")
        {
            if (!isMuteki)
                getHit(5);
        }
        //if(other.tag == "Enemy")
        //{
        //    if (!isMuteki)
        //        getHit(10);
        //    enemyHp _enemyHp = other.GetComponent<enemyHp>();
        //    _enemyHp.DeathEffect();
        //}
    }
    private void UpdateHpUI()
    {
        float hpAmount = (float)playerHp / (float)maxHp;
        //Debug.Log(HpAmount);
        hpBarImage.fillAmount = hpAmount;
        hpText.text = playerHp.ToString();
        if (playerHp <= 0)
        {
            StopVibration();
            GameOver();
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
        scenesManager.isGameOver = true;
        gameOverUI.SetActive(true);
        Time.timeScale = 0;
    }
    //IEnumerator MuTeKiTime(float mutekiTime)
    //{
    //    isMuteki = true;
    //    //Physics.IgnoreLayerCollision(8, 6, true);
    //    yield return new WaitForSeconds(mutekiTime);
    //    isMuteki = false;
    //    //Physics.IgnoreLayerCollision(8, 6, false);
    //}
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
    void Shake(float intensity)
    {
        impulseSource.GenerateImpulseWithForce(intensity);
    }
}
