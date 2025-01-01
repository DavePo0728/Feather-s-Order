using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LandMove : MonoBehaviour
{
    [SerializeField]
    float speed;
    [SerializeField]
    Vector3 originPoint;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        transform.Translate(-Vector3.forward * speed*Time.deltaTime);
        if(transform.position.z <= -900f)
        {
            transform.position = originPoint;
        }
    }
}
