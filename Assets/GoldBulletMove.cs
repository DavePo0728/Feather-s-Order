using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoldBulletMove : EnemyBulletMove
{
    [SerializeField]
    GameObject GoldBulletExplosion;
    float lifeTime = 10f;
    MeshRenderer meshRenderer;
    [HideInInspector]
    public bool bounceBack = false;
    // Start is called before the first frame update
    void Start()
    {
        moveToPlayer = false;
        StartCoroutine(CountDownInactive(lifeTime));
        moveToPlayer = true;
        meshRenderer = gameObject.GetComponent<MeshRenderer>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (moveToPlayer)
        {
            transform.Translate(-Vector3.forward * speed);
        }
        if (bounceBack)
        {
            if(gameObject.transform.position.z >= 100)
            {
                GoldBulletEffect();
            }
        }
    }
    public void GoldBulletEffect()
    {
        speed = 0;
        meshRenderer.enabled = false;
        Collider[] destroyList = Physics.OverlapSphere(transform.position,sphereRadius);
        for(int i=0; i < destroyList.Length; i++)
        {
            switch (destroyList[i].tag)
            {
                case "EnemyBullet":
                    destroyList[i].gameObject.SetActive(false);
                    break;
                case "Enemy":
                    Destroy(destroyList[i].gameObject);
                    break;
            }
        }
        GameObject temp = Instantiate(GoldBulletExplosion, transform.position, Quaternion.identity);
        Destroy(temp, 2f);
        Destroy(gameObject, 3f);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (bounceBack)
        {
            if(other.tag == "Enemy")
            {
                GoldBulletEffect();
            }
        }
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, sphereRadius);
    }
}
