using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBulletMove : MonoBehaviour
{
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
}
