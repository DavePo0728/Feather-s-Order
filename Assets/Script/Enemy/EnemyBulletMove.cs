using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBulletMove : MonoBehaviour
{
    [SerializeField]
    float speed;
    float lifeTime=2.5f;
    Rigidbody bulletRigidbody;
    [SerializeField]
    bool breakable, gold;
    [Header("Gizmo")]
    [SerializeField]
    float sphereRadius;
    // Start is called before the first frame update
    private void Awake()
    {
        bulletRigidbody = GetComponent<Rigidbody>();

    }
    public void Initial()
    {
        StartCoroutine(CountDownInactive());
        bulletRigidbody.velocity = new Vector3(0, 0, -speed);
    }
    IEnumerator CountDownInactive()
    {
        yield return new WaitForSeconds(lifeTime);
        gameObject.SetActive(false);
        Debug.Log("Inactive");
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "PlayerBullet"&&breakable)
        {
            gameObject.SetActive(false);
        }
    }
}
