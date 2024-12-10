using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraForward : MonoBehaviour
{
    [SerializeField]
    float speed;
    [SerializeField]
    GameObject PlayerCam;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        transform.position = new Vector3(transform.position.x,transform.position.y, transform.position.z);
        transform.Translate(Vector3.forward * speed *Time.deltaTime);
    }
}
