using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMoveB : EnemyMove
{
    float Speed;
    GameObject player;
    private void Awake()
    {
        //enemyData = Resources.Load<EnemyData>("EnemyData/EnemyData_keepDistance");
    }
    // Start is called before the first frame update
    void Start()
    {
        //player = GameObject.FindGameObjectWithTag("Player");
        //StartCoroutine(TrackPlayer());
        StartCoroutine(TimeToDestroy());
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //if (moveSpeed!= enemyData.originSpeed)
        //{
        //    moveSpeed = enemyData.originSpeed;
        //}
        transform.Translate(-Vector3.forward * moveSpeed);
    }
    IEnumerator TrackPlayer()
    {
        while (true)
        {
            transform.LookAt(player.transform);
            Debug.Log("yes");
            yield return new WaitForSeconds(1.0f);
        }
    }
    IEnumerator TimeToDestroy()
    {
        yield return new WaitForSeconds(lifeTime);
        Destroy(gameObject);
    }
}
