using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LandMove : MonoBehaviour
{
    [HideInInspector]
    public int nextListNum=2;
    [SerializeField]
    float speed;
    [SerializeField]
    Vector3 originPoint;
    [SerializeField]
    Vector3 endPoint,endPoinLeft,endPointRight;
    [SerializeField]
    bool left, right, main,firstMap,lastMap,initial,end;
    public bool isEnding;
    private void Awake()
    {
        if (initial)
        {
            endPoint = TerrainLoopManager.terrainInstance._terrainList[0].transform.position;
            endPoint.z -= 100f;
            TerrainLoopManager.terrainInstance.InitialPos = endPoint;
            initial = false;
            TerrainLoopManager.terrainInstance.usingListNum = 1;
        }
        if (TerrainLoopManager.terrainInstance._outSideLeftList.Count>0)
        {
            endPoinLeft = TerrainLoopManager.terrainInstance._outSideLeftList[0].transform.position;
            endPoinLeft.z -= 500f;
            endPointRight = TerrainLoopManager.terrainInstance._outSideRightList[0].transform.position;
            endPointRight.z -= 500f;
        }

    }
    // Start is called before the first frame update
    void Start()
    {
        if (!initial)
        {
            endPoint = TerrainLoopManager.terrainInstance.InitialPos;
        }
    }
    private void OnDisable()
    {
        end = false;
    }
    // Update is called once per frame
    void FixedUpdate()
    {
        transform.Translate(-transform.forward * speed);
        if (main)
        {
            if (transform.position.z < endPoint.z)
            {
                if(end)
                {
                    if (firstMap) 
                    {
                        switch (nextListNum)
                        {
                            case 1:
                                for (int i = 0; i < TerrainLoopManager.terrainInstance._terrainList.Count; i++)
                                {
                                    TerrainLoopManager.terrainInstance._terrainList[i].SetActive(true);
                                    TerrainLoopManager.terrainInstance._terrainList[i].transform.position = new Vector3(originPoint.x, originPoint.y, originPoint.z + (i * 100f));
                                    Debug.Log("Change2Complete");
                                    TerrainLoopManager.terrainInstance.usingListNum = 1;
                                }
                                break;
                            case 2:
                                for (int i = 0; i < TerrainLoopManager.terrainInstance._terrainList2.Count; i++)
                                {
                                    TerrainLoopManager.terrainInstance._terrainList2[i].SetActive(true);
                                    TerrainLoopManager.terrainInstance._terrainList2[i].transform.position = new Vector3(originPoint.x, originPoint.y, originPoint.z + (i * 100f));
                                    Debug.Log("Change3Complete");
                                    TerrainLoopManager.terrainInstance.usingListNum = 2;
                                }
                                break;
                            case 3:
                                for (int i = 0; i < TerrainLoopManager.terrainInstance._terrainList3.Count; i++)
                                {
                                    TerrainLoopManager.terrainInstance._terrainList3[i].SetActive(true);
                                    TerrainLoopManager.terrainInstance._terrainList3[i].transform.position = new Vector3(originPoint.x, originPoint.y, originPoint.z + (i * 100f));
                                    Debug.Log("Change1Complete");
                                    TerrainLoopManager.terrainInstance.usingListNum = 3;
                                }
                                break;
                        }
                    }
                    
                    gameObject.SetActive(false);
                }
                else
                {
                    transform.position = originPoint;
                    TerrainLoopManager.terrainInstance.Refresh();
                }
                if (lastMap&&isEnding)
                {
                    switch (TerrainLoopManager.terrainInstance.usingListNum)
                    {
                        case 1:
                            foreach (GameObject obj in TerrainLoopManager.terrainInstance._terrainList)
                            {
                                LandMove landMove = obj.GetComponent<LandMove>();
                                landMove.end = true;
                            }
                            break;
                        case 2:
                            foreach (GameObject obj in TerrainLoopManager.terrainInstance._terrainList2)
                            {
                                LandMove landMove = obj.GetComponent<LandMove>();
                                landMove.end = true;
                            }
                            break;
                        case 3:
                            foreach (GameObject obj in TerrainLoopManager.terrainInstance._terrainList3)
                            {
                                LandMove landMove = obj.GetComponent<LandMove>();
                                landMove.end = true;
                            }
                            break;
                    }
                    isEnding = false;
                }
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
