using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class chargeBulletMove : MonoBehaviour
{
    [SerializeField]
    float speed;
    [SerializeField]
    float lifeTime;
    GameObject lockedEnemy;
    private void Awake()
    {
       
    }
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(CountDownInactive());
    }

    // Update is called once per frame
    void FixedUpdate()
    {
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
}
