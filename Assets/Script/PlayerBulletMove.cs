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
    ParticleSystem body, glow,hitEffect;
    // Start is called before the first frame update
    private void Awake()
    {
        bulletRigidbody = GetComponent<Rigidbody>();
        bulletCollider = GetComponent<Collider>();
        body = GetComponent<ParticleSystem>();
        glow = transform.GetChild(1).GetComponent<ParticleSystem>();
        
    }
    void Start()
    {
        StartCoroutine(CountDownInactive());
        hitEffectObject = transform.GetChild(1).gameObject;
        hitEffect = hitEffectObject.GetComponent<ParticleSystem>();
    }
    private void OnEnable()
    {
        bulletCollider.enabled = true;
        speed = 300;
        //body.Play();
       // glow.Play();
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //bulletRigidbody.velocity = new Vector3(0, 0, speed);
        if (lockedEnemy != null)
        {
            transform.LookAt(lockedEnemy.transform);
            //Debug.Log(transform.rotation);
            transform.Translate(Vector3.forward * speed * Time.deltaTime);
        }
        else
        {
            transform.Translate(Vector3.forward * speed * Time.deltaTime);
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
        this.gameObject.SetActive(false);
    }
    IEnumerator HitEffectOn()
    {
        speed = 0;
        hitEffectObject.SetActive(true);
        hitEffect.Play();
        if (hitEffect.isPlaying)
        {
            Debug.Log("hitEffectPlaying");
        }
        bulletCollider.enabled = false;
        body.Stop();
        glow.Stop();
        yield return new WaitForSeconds(1f);
        gameObject.SetActive(false);
        hitEffectObject.SetActive(false);
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
