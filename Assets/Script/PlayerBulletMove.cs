using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBulletMove : MonoBehaviour
{
    [SerializeField]
    GameObject hitEffectObject;
    [SerializeField]
    float speed;
    float lifeTime = 4.0f;
    Rigidbody bulletRigidbody;
    [Header("Gizmo")]
    [SerializeField]
    float sphereRadius;
    GameObject lockedEnemy;
    Collider bulletCollider;
    ParticleSystem hitEffect;
    EnemyBulletData bulletData;

    // Start is called before the first frame update
    private void Awake()
    {
        bulletRigidbody = GetComponent<Rigidbody>();
        bulletCollider = GetComponent<Collider>();
        hitEffectObject = transform.GetChild(1).gameObject;
        hitEffect = hitEffectObject.GetComponent<ParticleSystem>();
        bulletData = Resources.Load<EnemyBulletData>("BulletData/PlayerBullet");
    }
    void Start()
    {
        
    }
    private void OnEnable()
    {
        StartCoroutine(CountDownInactive());
        if (lockedEnemy != null)
            transform.LookAt(lockedEnemy.transform);
        bulletCollider.enabled = true;
        speed = bulletData.speed;

    }
    private void OnDisable()
    {
        lockedEnemy = null;
    }
    // Update is called once per frame
    void FixedUpdate()
    {
        //bulletRigidbody.velocity = new Vector3(0, 0, speed);
        if (lockedEnemy != null)
        {
            //transform.LookAt(lockedEnemy.transform);
            //Debug.Log(transform.rotation);
            transform.Translate(Vector3.forward * speed);
        }
        else
        {
            transform.Translate(Vector3.forward * speed);
        }

    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag =="Enemy")
        {
            StartCoroutine(HitEffectOn());
        }
    }
    IEnumerator CountDownInactive()
    {
        yield return new WaitForSeconds(lifeTime);
        //Debug.Log("off");
        //if(gameObject.tag == "ChargeBullet")
        //{
        //    this.gameObject.SetActive(false);
        //}
        this.gameObject.SetActive(false);
    }
    IEnumerator HitEffectOn()
    {
        if (gameObject.tag =="PlayerBullet")
        {
            speed = 0;
            bulletCollider.enabled = false;
        }
        hitEffectObject.SetActive(true);
        hitEffect.Play();
        if (hitEffect.isPlaying)
        {
            //Debug.Log("hitEffectPlaying");
        }


        yield return new WaitForSeconds(1f);
        if (gameObject.tag == "PlayerBullet")
        {
            gameObject.SetActive(false);
            hitEffectObject.SetActive(false);
        }
    }
    public void SetLockedEnemy(GameObject enemy)
    {
        lockedEnemy = enemy;
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, sphereRadius);
    }
}
