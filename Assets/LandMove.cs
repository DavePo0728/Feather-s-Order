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
    Vector3 endPoint,endPoinLeft,endPointRight;
    [SerializeField]
    bool left, right, main;
    private void Awake()
    {
        
    }
    // Start is called before the first frame update
    void Start()
    {
        endPoint = TerrainLoopManager.terrainInstance._terrainList[0].transform.position;
        endPoint.z -= 100f;
        endPoinLeft = TerrainLoopManager.terrainInstance._outSideLeftList[0].transform.position;
        endPoinLeft.z -= 500f;
        endPointRight = TerrainLoopManager.terrainInstance._outSideRightList[0].transform.position;
        endPointRight.z -= 500f;
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        transform.Translate(-transform.forward * speed);
        if (main)
        {
            if (transform.position.z < endPoint.z)
            {
                transform.position = originPoint;
                TerrainLoopManager.terrainInstance.Refresh();
            }
        }
        if (left)
        {
            if (transform.position.z < endPoinLeft.z)
            {
                transform.position = originPoint;
                //TerrainLoopManager.terrainInstance.Refresh();
            }
        }
        if (right)
        {
            if (transform.position.z < endPointRight.z)
            {
                transform.position = originPoint;
                //TerrainLoopManager.terrainInstance.Refresh();
            }
        }
    }
}
