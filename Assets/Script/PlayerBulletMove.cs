using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBulletMove : MonoBehaviour
{
    GameObject hitEffect;
    [SerializeField]
    float speed;
    float lifeTime = 4.0f;
    Rigidbody bulletRigidbody;
    [Header("Gizmo")]
    [SerializeField]
    float sphereRadius;
    // Start is called before the first frame update
    private void Awake()
    {
        bulletRigidbody = GetComponent<Rigidbody>();
        
    }
    void Start()
    {
        StartCoroutine(CountDownInactive());
        hitEffect = transform.GetChild(2).gameObject;
        speed = 600f;
    }

    // Update is called once per frame
    void Update()
    {
        bulletRigidbody.velocity = new Vector3(0, 0, speed);

    }
    //private void OnDrawGizmos()
    //{
    //    Gizmos.color = Color.green;
    //    Gizmos.DrawWireSphere(transform.position, sphereRadius);
    //}
    IEnumerator CountDownInactive()
    {
        yield return new WaitForSeconds(lifeTime);
        //Debug.Log("off");
        this.gameObject.SetActive(false);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Enemy")
        {

        }
        if(other.tag == "Block")
        {

        }
    }
    IEnumerator HitEffectOn()
    {
        speed = 0;
        hitEffect.SetActive(true);
        yield return new WaitForSeconds(0.5f);

    }
}
