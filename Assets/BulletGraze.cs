using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class BulletGraze : MonoBehaviour
{
    [Header("GrazeBulletData")]
    public bool canGraze;
    public float grazeCD;
    public float maxGrazeEnergy;
    public float currentGrazeEnergy;
    public float grazeEnergyGain;

    public Image grazeEnergyBar;
    AudioSource grazeSound;
    AudioClip grazeClip;

    private void Awake()
    {
        grazeSound = GetComponent<AudioSource>();
        grazeClip = Resources.Load<AudioClip>("Sound/BulletGrazing");
    }
    // Start is called before the first frame update
    void Start()
    {
        canGraze = true;
        currentGrazeEnergy = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            currentGrazeEnergy += maxGrazeEnergy;
            UpdateGrazeUI();
        }
        //if(Input.GetKeyDown(KeyCode.Keypad1))
        //{
        //    Vibrate(0.1f, 0.1f, 0.1f);
        //}
        //if (Input.GetKeyDown(KeyCode.Keypad2))
        //{
        //    Vibrate(0.2f, 0.2f, 0.1f);
        //}
        //if (Input.GetKeyDown(KeyCode.Keypad3))
        //{
        //    Vibrate(0.3f, 0.3f, 0.1f);
        //}
        //if (Input.GetKeyDown(KeyCode.Keypad4))
        //{
        //    Vibrate(0.4f, 0.4f, 0.1f);
        //}
        //if (Input.GetKeyDown(KeyCode.Keypad5))
        //{
        //    Vibrate(0.5f, 0.5f, 0.1f);
        //}
        //if (Input.GetKeyDown(KeyCode.Keypad6))
        //{
        //    Vibrate(0.6f, 0.6f, 0.1f);
        //}
        //if (Input.GetKeyDown(KeyCode.Keypad7))
        //{
        //    Vibrate(0.7f, 0.7f, 0.1f);
        //}        
        //if (Input.GetKeyDown(KeyCode.Keypad8))
        //{
        //    Vibrate(0.8f, 0.9f, 0.1f);
        //}        
        //if (Input.GetKeyDown(KeyCode.Keypad9))
        //{
        //    Vibrate(0.9f, 0.9f, 0.1f);
        //}
        //if (Input.GetKeyDown(KeyCode.Keypad0))
        //{
        //    Vibrate(1.0f, 1.0f, 0.1f);
        //}
        //if (Input.GetKeyDown(KeyCode.KeypadPeriod))
        //{
        //    float lowFrequency = Random.Range(0.0f, 1.0f);
        //    float highFrequency = Random.Range(0.0f, 1.0f);
        //    float duration = Random.Range(0.1f, 0.5f);
        //    Vibrate(lowFrequency, highFrequency, duration);
        //    Debug.Log(lowFrequency + " , " + highFrequency);
        //}
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "BulletGrazeCollider")
        {
            if (canGraze)
            {
                Vibrate(0.1f, 0.1f, 0.05f);
                grazeSound.PlayOneShot(grazeClip);
                Debug.Log(other.transform.parent.name);
                StartCoroutine(GrazeCD());
                currentGrazeEnergy += grazeEnergyGain;
                if (currentGrazeEnergy > maxGrazeEnergy)
                {
                    currentGrazeEnergy = maxGrazeEnergy;
                }
                UpdateGrazeUI();
            }
        }
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
    public void UpdateGrazeEnergyOutside(float grazeEnergy)
    {
        currentGrazeEnergy -= grazeEnergy;
        if (currentGrazeEnergy < 0)
        {
            currentGrazeEnergy = 0;
        }
        UpdateGrazeUI();
    }
    public bool CheckGrazeEnergy(float costEnergy)
    {
        if (currentGrazeEnergy >= costEnergy)
        {
            return true;
        }
        else
        {
            Debug.Log("Graze Energy Not Enough");
            return false;
        }
    }
    public void UpdateGrazeUI()
    {
        grazeEnergyBar.fillAmount = currentGrazeEnergy / maxGrazeEnergy;
    }
    IEnumerator GrazeCD()
    {
        canGraze = false;
        yield return new WaitForSeconds(grazeCD);
        canGraze = true;
    }
}

