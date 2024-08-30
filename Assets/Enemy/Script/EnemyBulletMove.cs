using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBulletMove : MonoBehaviour
{
    [SerializeField]
    float speed;
    float lifeTime = 3.5f;
    Rigidbody bulletRigidbody;
    [SerializeField]
    bulletType bullet_Type;
    [Header("Gizmo")]
    [SerializeField]
    float sphereRadius;
    bool initialMove = false;
    bool moveToPlayer = false;
    Vector3 destination;
    private enum bulletType
    {
        normal, breakable, golden
    }
    private void Awake()
    {
        bulletRigidbody = GetComponent<Rigidbody>();
    }
    public void Initial()
    {
        initialMove = false;
        moveToPlayer = false;
        StartCoroutine(CountDownInactive());
        moveToPlayer = true;
    }
    public void NoMoveInitial()
    {
        initialMove = false;
        moveToPlayer = false;
        StartCoroutine(CountDownInactive());
    }
    public void Update()
    {
        if (initialMove)
        {
            var step = speed * Time.deltaTime; // calculate distance to move
            transform.position = Vector3.MoveTowards(transform.position, destination, step);
            if (Vector3.Distance(transform.position, destination) < 0.001f)
            {
                initialMove = false;
                moveToPlayer = true;
            }
        }
        if (moveToPlayer)
        {
            transform.Translate(-Vector3.forward * 0.1f);
        }
    }
    IEnumerator CountDownInactive()
    {
        yield return new WaitForSeconds(lifeTime);
        gameObject.SetActive(false);
        //Debug.Log("Inactive");
    }
    public void Speard()
    {
        destination = transform.position + new Vector3(Random.Range(-2f,2f), Random.Range(-2f,2f), Random.Range(-2f,2f));
        
        initialMove = true;
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "PlayerBullet"&&bullet_Type ==bulletType.breakable)
        {
            gameObject.SetActive(false);
        }
        if(other.tag == "PlayerBullet"&&bullet_Type ==bulletType.golden)
        {
            //gameObject.SetActive(false);
        }
        if(other.tag == "PlayerBullet"&&bullet_Type ==bulletType.normal)
        {
            //gameObject.SetActive(false);
        }
    }
}
