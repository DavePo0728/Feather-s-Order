using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMoveTypeB : EnemyMove
{
    GameObject player;
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        //StartCoroutine(TrackPlayer());
    }

    // Update is called once per frame
    void FixedUpdate()
    {
            transform.Translate(-Vector3.forward * speed * Time.deltaTime);
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
}
