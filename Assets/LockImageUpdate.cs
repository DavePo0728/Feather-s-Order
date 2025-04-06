using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockImageUpdate : MonoBehaviour
{
    Camera playerVcam;
    GameObject lockEnemy;
    private void Awake()
    {
        playerVcam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
    }
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (lockEnemy != null)
        {
            transform.position = playerVcam.WorldToScreenPoint(lockEnemy.transform.position);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    public void SetTarget(GameObject target)
    {
        lockEnemy = target;
    }
}
