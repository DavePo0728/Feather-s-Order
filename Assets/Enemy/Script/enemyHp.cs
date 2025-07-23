using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
public class EnemyHp : MonoBehaviour
{
    [SerializeField]
    EnemyMove enemyMove;
    AimDetect aimDetect;
    Collider enemyCollider;
    [SerializeField]
    public float maxHp;
    [SerializeField]
    float currentHp;

    [Header("Corrupt Data")]
    //GameObject corruptEffect;
    [SerializeField]
    GameObject corruptionExplosionEffect;
    public bool corrupted;
    [SerializeField]
    bool corruption;
    public bool corruption_P => corruption;
    float corruptionDamageModifier;// 污穢傷害倍率
    [SerializeField]
    float currentCorruptionValue;// 當前污穢值
    public float maxCorruptionValue;// 污穢值上限
    public float corruptionDecreaseTime; // 污穢恢復時間
    float corruptionDecreaseTimer; // 污穢恢復計時器
    public float corruptionDecreaseSpeed; // 污穢恢復量
    [SerializeField]
    GameObject corruptionCleanseObject, chainEffectObject, UnboxExplosion, ChainEffect_broken;

    ParticleSystem[] ChainEffectGroup = new ParticleSystem[9];

	ParticleSystem corruptionCleanseParticle, chainEffectParticle;
    PlayerSlashAttack playerSlashAttack;
    [Header("Shield Data")]
    [SerializeField]
    GameObject shieldEffect;
    [SerializeField]
    GameObject shieldExplosionEffect;
    public bool haveshield;
    [SerializeField]
    float currentShieldHp;
    [SerializeField]
    public float maxShieldHp;
    float ShieldDamageModifier;
    [SerializeField]
    GameObject DeathExplosion;
    ScoreManager scoreManager;

    ParticleSystem slashHitEffectYellow, slashHitEffectRed;
    GameObject slashHitEffectYellowObject, slashHitEffectRedObject;
    int hitCounter;
    bool slashDetectBool =false;

    [Header("Audio")]
    [SerializeField]
    AudioClip shieldHitAudioClip; // 擊中盾音效
    [SerializeField]
    AudioClip shieldBreakAudioClip; // 破盾音效
    [SerializeField]
    AudioClip hitimpactAudioClip; // 擊中敵人聲
    [SerializeField]
    AudioClip hitXAudioClip; // X彈擊中敵人聲
	[SerializeField]
	AudioClip hitMisairuAudioClip; // 導彈擊中敵人聲
	public AudioSource audioSource;
    [SerializeField] AudioClip deathAudioClip; // 敵人死亡音效
    [SerializeField] AudioClip SlashHITClip; // 敵人近戰受擊音效

    [Header("UI")]
    [SerializeField]
    Canvas canvas;
    Image hpImage;
	EnterExitSchedule EES;
    [SerializeField]
    Image corruptionImageLeft, corruptionImageRight;

    [SerializeField] private AudioMixerGroup sfxGroup;

	private float hitXAudioCooldown = 0.05f; // 冷卻時間（秒）
    private float lastHitXAudioTime = -Mathf.Infinity; // 上次播放時間

	public GameObject MisairuHit; // 導彈擊中敵人特效
	private void Awake()
    {
        enemyCollider = GetComponent<Collider>();
        aimDetect = GameObject.Find("AimDetectCollider").GetComponent<AimDetect>();
        corruptionDamageModifier = 0.5f; // ��©�ˮ`���v
        corruptionDamageModifier = 0.5f; // 污穢傷害倍率
        playerSlashAttack = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerSlashAttack>();
        enemyMove = gameObject.GetComponent<EnemyMove>();
        canvas = transform.Find("StatusCanvas").GetComponent<Canvas>();
        audioSource = GetComponent<AudioSource>();
		shieldEffect = transform.Find("MagicShieldBlue").gameObject;
        shieldExplosionEffect = transform.Find("TargetHitExplosion").gameObject;
        //corruptEffect = transform.Find("CorruptionEffect").gameObject;
        slashHitEffectYellowObject = transform.Find("SwordHitMagicYellow").gameObject;
        slashHitEffectRedObject = transform.Find("SwordHitRedCritical").gameObject;
        slashHitEffectYellow = slashHitEffectYellowObject.GetComponent<ParticleSystem>();
        slashHitEffectRed = slashHitEffectRedObject.GetComponent<ParticleSystem>();
        canvas.worldCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        hpImage = canvas.transform.GetChild(0).GetChild(0).Find("HPBar").GetComponent<Image>();
        DeathExplosion = Resources.Load<GameObject>("ShadowExplosion2");
        scoreManager = GameObject.Find("ScoreManager").GetComponent<ScoreManager>();
        shieldHitAudioClip = Resources.Load<AudioClip>("Sound/ShieldHitSound");
        shieldBreakAudioClip = Resources.Load<AudioClip>("Sound/ShieldBreakSound");
        hitimpactAudioClip = Resources.Load<AudioClip>("Sound/HitImpactSound");
		hitXAudioClip = Resources.Load<AudioClip>("Sound/X_slash_hit");
		hitMisairuAudioClip = Resources.Load<AudioClip>("Sound/missile_hit");
        SlashHITClip = Resources.Load<AudioClip>("Sound/slashHit");
        corruptionImageLeft = canvas.transform.Find("StatusUI").Find("CorruptionBG").Find("CorruptionBarLeft").GetComponent<Image>();
        corruptionImageRight = canvas.transform.Find("StatusUI").Find("CorruptionBG").Find("CorruptionBarRight").GetComponent<Image>();
        corruptionCleanseObject = transform.Find("CorruptionCleanse").gameObject;
        corruptionCleanseParticle =corruptionCleanseObject.GetComponent<ParticleSystem>();
        chainEffectObject = transform.Find("ChainEffect").gameObject;
        ChainEffect_broken = transform.Find("ChainEffect_brokenNew").gameObject;
        ChainEffectGroup[0] = chainEffectObject.transform.Find("Ring1").GetComponent<ParticleSystem>();
        ChainEffectGroup[1] = chainEffectObject.transform.Find("Ring2").GetComponent<ParticleSystem>();
		ChainEffectGroup[2] = chainEffectObject.transform.Find("Ring3").GetComponent<ParticleSystem>();
		ChainEffectGroup[3] = ChainEffect_broken.transform.Find("brokenRing1").GetComponent<ParticleSystem>();
		ChainEffectGroup[4] = ChainEffect_broken.transform.Find("brokenRing2").GetComponent<ParticleSystem>();
		ChainEffectGroup[5] = ChainEffect_broken.transform.Find("brokenRing3").GetComponent<ParticleSystem>();
		ChainEffectGroup[6] = ChainEffect_broken.transform.Find("Sparks_broken").GetComponent<ParticleSystem>();
		ChainEffectGroup[7] = ChainEffect_broken.transform.Find("CenterSpark_broken").GetComponent<ParticleSystem>();
		ChainEffectGroup[8] = ChainEffect_broken.transform.Find("Glow_broken").GetComponent<ParticleSystem>();
		chainEffectParticle = transform.Find("ChainEffect").gameObject.GetComponent<ParticleSystem>();
        UnboxExplosion = transform.Find("UnboxExplosion").gameObject;
        deathAudioClip = Resources.Load<AudioClip>("Sound/EnemyDeathSound");
        audioSource.outputAudioMixerGroup = sfxGroup;

	}
    // Start is called before the first frame update
    void Start()
    {
		EES = GetComponent<EnterExitSchedule>();
		if (corrupted)
        {
            corruption = true;
            //corruptEffect.SetActive(true);
            currentCorruptionValue = 0;
        }
        else 
        {
            corruption = false;
            //corruptEffect.SetActive(false);
            currentCorruptionValue = 0;
        }
        //corruptionDamageModifier = 0.5f;
        currentHp = maxHp;
        UpdateUI();
        if (corrupted)
        {
            UpdateCorruptionUI();
        }
        else
        {
            InitializeCorruptionUI();
        }

        if (haveshield)
        {
            
            currentShieldHp = maxShieldHp;
            shieldEffect.SetActive(true);
        }
        else
        {
            shieldEffect.SetActive(false);
            currentShieldHp = 0;
        }
        canvas.gameObject.SetActive(false); // 先隱藏UI
    }

    // Update is called once per frame
    void Update()
    {
		
		corruptionDecreaseTimer += Time.deltaTime; // 增加計時器
        if (corruption&&corruptionDecreaseTimer > corruptionDecreaseTime)
        {
            if(currentCorruptionValue > 0)
            {
                currentCorruptionValue -= corruptionDecreaseSpeed;
                UpdateCorruptionUI();
            }
        }
        if (corruption == false)
        {
            UpdateParalazeUI(); // 更新UI顯示的麻痺時間
        }
    }
    public void UpdateParalazeUI()
    {
        float ratio = Mathf.Clamp01( enemyMove.currentParalyzeTime/ enemyMove.initialParalyzeTime);
        corruptionImageLeft.fillAmount = ratio;
        corruptionImageRight.fillAmount = ratio;
    }
    public void ShootHurt(float damage ,int BulletType)
    {
        if (haveshield)     //打到盾無效
        {
            PlayShieldHitSound();
            //Debug.Log("Source :"+gameObject.name+" "+"ShieldBlock");
            //播抵消特效
            return;
        }
        else
        {
			switch (BulletType)
			{
				case 0: // 普通子彈
					PlayhitNormalAudio();
					break;
				case 1: // X子彈
					//PlayhitXAudio();
					break;
				case 2: // 追蹤子彈
					//PlayhitMisairuAudio();
					break;
				default:
					break;
			}
		}
        if (corrupted)      //如果有污穢
        {
            if (canvas.gameObject.activeSelf == false)
                canvas.gameObject.SetActive(true); // 顯示UI
            if (corruption == true)      //如果污穢未被解除
            {
				currentHp -= damage * corruptionDamageModifier;
                UpdateUI();
                //Debug.Log("Source :" + gameObject.name + " " + "CorruptionDamage:"+ damage * corruptionDamageModifier);
                if (currentHp <= 0)
                {
                    DeathEffect();
                    scoreManager.AddScore();
                }
            }
            else       //如果污穢被解除
            {

				currentHp -= damage;
                UpdateUI();
                if (currentHp <= 0)
                {
                    DeathEffect();
                    scoreManager.AddScore();
                }
            }
            
        }
        else       //沒有污穢
        {
            if (canvas.gameObject.activeSelf == false)
                canvas.gameObject.SetActive(true); // 顯示UI


            currentHp -= damage;
            UpdateUI();
            if (currentHp <= 0)
            {
                DeathEffect();
                scoreManager.AddScore();
            }
        }
    }
    public void ShieldHurt(float damage) //只有近攻會觸發這個
    {
        if (haveshield)
        {
            if (currentShieldHp > 0)
            {
                PlayShieldHitSound();
                currentShieldHp -= damage;
                UpdateUI();
                if (shieldEffect != null && currentShieldHp <= 0)
                {
                    shieldEffect.SetActive(false);
                    shieldExplosionEffect.SetActive(true);
                    PlayShieldBreakSound();
                    haveshield = false;
                }
            }
        }
        else
        {
            if (corrupted)
            {
                if (canvas.gameObject.activeSelf == false)
                    canvas.gameObject.SetActive(true); // 顯示UI
                if (corruption == false)
                {
                    PlayhitNormalAudio();
                    currentHp -= damage;
                    UpdateUI();
                    //Debug.Log("Source :" + gameObject.name + " " + "CorruptionClean");
                    if (currentHp <= 0)
                    {
                        DeathEffect();
                        scoreManager.AddScore();
                        PlayerHP playerHP = GameObject.FindGameObjectWithTag("HPCollider").GetComponent<PlayerHP>();
                        playerHP.Heal(10);
                    }
                }
            }
            else
            {
                if (canvas.gameObject.activeSelf == false)
                    canvas.gameObject.SetActive(true); // 顯示UI
                PlayhitNormalAudio();
                currentHp -= damage;
                UpdateUI();
                //Debug.Log("Source :" + gameObject.name + " " + "NoCSorruption");
                if (currentHp <= 0)
                {
                    DeathEffect();
                    scoreManager.AddScore();
                    PlayerHP playerHP = GameObject.FindGameObjectWithTag("HPCollider").GetComponent<PlayerHP>();
                    playerHP.Heal(10);
                }
            }
        }   
    }
    public void DeathEffect()/// 死亡特效
    {
        if (aimDetect != null)
        {
            aimDetect.ManualOnTriggerExit(enemyCollider); 
        }
        EES.PlayDeathAnimation(); 
        transform.Find("StatusCanvas").gameObject.SetActive(false);
        enemyMove.DoStop(); // 停止移動
        enemyMove.DoStopA();
        enemyMove.gun.SetActive(false); // 停止射擊

		enemyCollider.enabled = false; 
		EES.PlayDeathAnimation(); // 播放死亡動畫
		transform.Find("StatusCanvas").gameObject.SetActive(false); // 隱藏UI
		GameObject sfxPlayer = new GameObject("DeathSFX");
        sfxPlayer.transform.position = transform.position;
        AudioSource sfxAudio = sfxPlayer.AddComponent<AudioSource>();
        sfxAudio.clip = deathAudioClip;
        //  音量控制（你可以這裡調整音量大小）
        sfxAudio.volume = 1f;
        //  空間感：讓聲音根據距離遠近衰減
        sfxAudio.spatialBlend = 1f;       // 3D 音效
        sfxAudio.minDistance = 50f;        // 在這距離內聲音不變
        sfxAudio.maxDistance = 300f;       // 超過這距離聲音最小
        //  混音群組（可選，如果你用 Audio Mixer）
        // sfxAudio.outputAudioMixerGroup = yourEnemySFXGroup;
        sfxAudio.Play();
        

        Destroy(sfxPlayer, deathAudioClip.length);
        GameObject effect = Instantiate(DeathExplosion, transform.position, Quaternion.identity);
        Destroy(effect, 1.5f);
    }

    private void OnTriggerEnter(Collider other)
    {
        if ((other.tag=="PlayerMissile"))
        {
            if (haveshield==false && currentShieldHp <= 0)
            {
                if (corrupted)
                {
                    PlayerMissileMove playerMissileMove = other.GetComponent<PlayerMissileMove>();
                    CleanseCorruption(playerMissileMove.corruptionDamage);
                    ShootHurt(5, 2);
                }
                else
                {
                    ShootHurt(5, 2);
                }
				Transform MisairuHitPos = other.transform;
				Instantiate(MisairuHit, MisairuHitPos.position, transform.rotation);
			}
        }
        if (other.tag == "PlayerBullet")
        {
            ShootHurt(1, 0);
            if (haveshield == false && currentShieldHp <= 0)
            {
                if (corrupted)
                {
                    CleanseCorruption(0.25f);
                }
            } 
        }
        if (other.tag == "ChargeBullet")
        {
            if(haveshield == false && currentShieldHp <= 0)
            {
                if (corrupted)
                {
					
					CleanseCorruption(10);

                    ShootHurt(20, 1);

				}
                else
                {
					
					ShootHurt(20, 1);

				}
            }

            //Debug.Log("hit");
        }
        if(other.tag == "SlashCollider"&&slashDetectBool==false)
        {
            SlashHurt(40);
        }
    }
    void SlashHurt(float damage)
    {
        slashDetectBool = true;
        if (hitCounter < 3)
        {
            slashHitEffectYellowObject.SetActive(true);
            slashHitEffectYellow.Play();
            hitCounter++;
            ShieldHurt(damage);
            Invoke("SetSlashDetectBool", 0.2f);

            //PlaySlashHitAudio(); // <<<<< 新增！播放近戰打擊音效
        }
        else if (hitCounter >= 3)
        {
            Debug.Log("Source :" + gameObject.name + " " + "SlashHit");
			slashHitEffectRedObject.SetActive(true);
            slashHitEffectRed.Play();
            hitCounter = 0;
            ShieldHurt(damage);
            Invoke("SetSlashDetectBool", 0.3f);
            
            //PlaySlashHitAudio(); // <<<<< 新增！播放近戰打擊音效
        }
    }
    void SetSlashDetectBool()
    {
        slashDetectBool = false;
    }
    void CleanseCorruption(float corruptionDamage)
    {
        corruptionDecreaseTimer = 0; // 重置污穢恢復計時器
        if (currentCorruptionValue < maxCorruptionValue)
        {
            currentCorruptionValue += corruptionDamage;
            UpdateCorruptionUI();
        }
        if (corruption)
        {
            if (currentCorruptionValue >= maxCorruptionValue)
            {
                corruption = false;
                corruptionCleanseObject.SetActive(true);
                chainEffectObject.SetActive(true);
				UnboxExplosion.SetActive(true);

				chainEffectParticle.Play();
                corruptionCleanseParticle.Play();
                //corruptEffect.SetActive(false);
                enemyMove.Paralyze();
            }
        }
    }
    public void CorruptionRecover()
    {
        corruption = true;
        currentCorruptionValue = 0;
        UpdateCorruptionUI();
        playerSlashAttack.ForceFallBack();
        corruptionCleanseObject.SetActive(false);
        chainEffectObject.SetActive(false);
		UnboxExplosion.SetActive(false);
		ChainEffect_broken.SetActive(true);
		//corruptEffect.SetActive(true);
    }
    void UpdateCorruptionUI()
    {
        float CorruptionAmount = currentCorruptionValue / maxCorruptionValue;
        corruptionImageLeft.fillAmount = CorruptionAmount;
        corruptionImageRight.fillAmount = CorruptionAmount;
    }
    void InitializeCorruptionUI()
    {
        corruptionImageLeft.fillAmount = 0f;
        corruptionImageRight.fillAmount = 0f;
    }
    void UpdateUI()
    {
        float HpAmount = currentHp / maxHp;
        hpImage.fillAmount = HpAmount;
    }
    private void PlayShieldHitSound()
    {
        if (audioSource != null && shieldHitAudioClip != null)
        {
            audioSource.PlayOneShot(shieldHitAudioClip);
        }
    }

    private void PlayShieldBreakSound()
    {
        if (audioSource != null && shieldBreakAudioClip != null)
        {
            audioSource.PlayOneShot(shieldBreakAudioClip);
        }
    }

    private void PlayhitNormalAudio()
    {
        if (audioSource != null && hitimpactAudioClip != null)
        {
            audioSource.PlayOneShot(hitimpactAudioClip);
            // 播放普通擊中敵人聲
        }
    }
	private void PlayhitDeathlAudio()
	{
		if (audioSource != null && hitimpactAudioClip != null)
		{
			audioSource.PlayOneShot(deathAudioClip);
			// 播放普通擊中敵人聲
		}
	}
	public void PlayhitXAudio()
	{
		if (Time.time - lastHitXAudioTime < hitXAudioCooldown)
		{
			//Debug.Log("hit X audio on cooldown");
			return;
		}

		if (audioSource != null && hitXAudioClip != null)
		{
			audioSource.PlayOneShot(hitXAudioClip);
			//Debug.Log("Play hit X audio");
			lastHitXAudioTime = Time.time;
		}
	}
	public void PlayhitMisairuAudio(float AudioVolume)
	{
		if (audioSource != null && hitimpactAudioClip != null)
        {
            audioSource.PlayOneShot(hitMisairuAudioClip, AudioVolume);
            // 播放導彈擊中敵人聲
            Debug.Log("Play hit Misairu audio" +"+音量:" + AudioVolume);
            
        }
    }
	private void PlaySlashHitAudio()
    {
        if (audioSource != null && SlashHITClip != null)
        {
            audioSource.pitch = Random.Range(0.95f, 1.05f); // 在 AudioSource 上調音高
            audioSource.PlayOneShot(SlashHITClip); // 播放 Clip
            audioSource.pitch = 1f; // 播放後重置回正常，避免後面別的聲音也受影響
        }
    }
    public void ChainEffectContrl(int mod)
    {
        switch (mod)
        {
			case 1: // 關閉連鎖特效
                ChainEffectGroup[0].Stop();
				ChainEffectGroup[3].Play();
				break;
			case 2: // 關閉連鎖特效並播放破碎特效
				ChainEffectGroup[1].Stop();
				ChainEffectGroup[4].Play();
				break;
            case 3:
				ChainEffectGroup[2].Stop();
                ChainEffectGroup[5].Play();
                ChainEffectGroup[6].Play();
                ChainEffectGroup[7].Play();
				ChainEffectGroup[8].Play();

				break;
			default:
                break;
        }
    }
}