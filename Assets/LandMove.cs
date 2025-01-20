using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LandMove : MonoBehaviour
{
    [SerializeField]
    float speed;
    [SerializeField]
    Vector3 originPoint;
    [SerializeField]
    float zOffset;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        transform.Translate(-Vector3.forward * speed);
        if(transform.position.z < -11000)
        {
            originPoint.z += zOffset;
            transform.position = originPoint;
            TerrainLoopManager.terrainInstance.Refresh();
        }
    }
}
