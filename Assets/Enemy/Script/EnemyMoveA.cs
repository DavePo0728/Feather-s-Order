using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMoveA : EnemyMove
{
    public GameObject endPoint;
    [SerializeField]
    GameObject gun;
    [SerializeField]
    float hSpeed;
    [SerializeField]
    bool enterScene=false;
    [SerializeField]
    bool isLeaving = false;
    [SerializeField]
    float leaveTime;
    [SerializeField]
    float timer;

    void Start()
    {
        enemyData = Resources.Load<EnemyData>("EnemyData/EnemyData_keepDistance");
        enterScene = true;
        timer = 0;
        lifeTime = 15f;
        StartCoroutine(TimeToDestroy());
        gun = transform.GetChild(1).gameObject;
        gun.SetActive(false);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!enterScene && !isLeaving)
        {
            timer += Time.deltaTime; 
        }

        if (timer>=leaveTime)
        {
            enterScene = false;
            isLeaving = true;
            timer = 0;
        }

        if (speed != enemyData.originSpeed)
        {
            speed = enemyData.originSpeed;
        }

        if (enterScene&&!isLeaving)
        {
            if (endPoint != null)
            {
                //Debug.Log(gameObject.name+" "+(Mathf.Abs(endPoint.transform.position.x - transform.position.x)));
                if (Mathf.Abs(endPoint.transform.position.x - transform.position.x) > 0.5f)
                {
                    //left to right
                    if (endPoint.transform.position.x > transform.position.x)
                    {
                        transform.Translate(new Vector3(1 * hSpeed * Time.deltaTime, 0, 1 * speed * Time.deltaTime));
                        //Debug.Log("LeftIN");
                    }
                    else //right to left
                    {
                        transform.Translate(new Vector3(-1 * hSpeed * Time.deltaTime, 0, 1 * speed * Time.deltaTime));
                        //Debug.Log("RightIN");
                    }
                }
                else
                {
                    enterScene = false;
                }
            }
        }

        if(!enterScene && !isLeaving)
        {
            transform.Translate(Vector3.forward * speed * Time.deltaTime);
            gun.SetActive(true);
            //Debug.Log("Go Forward");
        }

        if (!enterScene && isLeaving)
        {
            //left to right
            if (endPoint.transform.position.x > transform.position.x)
            {
                transform.Translate(new Vector3(-1 * hSpeed * Time.deltaTime, 0, 1 * speed * Time.deltaTime));
                //Debug.Log("Leftout");
            }
            else //right to left
            {
                transform.Translate(new Vector3(1 * hSpeed * Time.deltaTime, 0, 1 * speed * Time.deltaTime));
                //Debug.Log("Rightout");
            }
        }
    }
    IEnumerator TimeToDestroy()
    {
        yield return new WaitForSeconds(lifeTime);
        Destroy(gameObject);
    }
}
