using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParkourLandMove : MonoBehaviour
{
    ParkourTerrainLoopManager terrainLoopManager;
    [HideInInspector]
    public int nextListNum = 2;
    [SerializeField]
    float speed;
    [SerializeField]
    Vector3 originPoint;
    [SerializeField]
    Vector3 endPoint;
    [SerializeField]
    bool left, right, main, firstMap, lastMap, initial, end;
    public bool isEnding;
    // Start is called before the first frame update
    void Start()
    {
        terrainLoopManager = ParkourTerrainLoopManager.terrainInstance;
        if (initial)
        {
            endPoint = terrainLoopManager._terrainList1[0].transform.position;
            endPoint.z -= 100f;
            terrainLoopManager.InitialPos = endPoint;
            initial = false;
            terrainLoopManager.usingListNum = 1;
        }
        Invoke("OtherInitial", 0.1f);
    }
    void OtherInitial()
    {
        if (!initial)
        {
            endPoint = terrainLoopManager.InitialPos;
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
                if (end)
                {
                    if (firstMap)
                    {
                        switch (nextListNum)
                        {
                            case 1:
                                for (int i = 0; i < terrainLoopManager._terrainList1.Count; i++)
                                {
                                    terrainLoopManager._terrainList1[i].SetActive(true);
                                    terrainLoopManager._terrainList1[i].transform.position = new Vector3(originPoint.x, originPoint.y, originPoint.z + (i * 100f));
                                    Debug.Log("Change2Complete");
                                    terrainLoopManager.usingListNum = 1;
                                }
                                break;
                            case 2:
                                for (int i = 0; i < terrainLoopManager._terrainList2.Count; i++)
                                {
                                    terrainLoopManager._terrainList2[i].SetActive(true);
                                    terrainLoopManager._terrainList2[i].transform.position = new Vector3(originPoint.x, originPoint.y, originPoint.z + (i * 100f));
                                    Debug.Log("Change3Complete");
                                    terrainLoopManager.usingListNum = 2;
                                }
                                break;
                            case 3:
                                for (int i = 0; i < terrainLoopManager._terrainList3.Count; i++)
                                {
                                    terrainLoopManager._terrainList3[i].SetActive(true);
                                    terrainLoopManager._terrainList3[i].transform.position = new Vector3(originPoint.x, originPoint.y, originPoint.z + (i * 100f));
                                    Debug.Log("Change1Complete");
                                    terrainLoopManager.usingListNum = 3;
                                }
                                break;
                            case 4:
                                for (int i = 0; i < terrainLoopManager._terrainList4.Count; i++)
                                {
                                    terrainLoopManager._terrainList4[i].SetActive(true);
                                    terrainLoopManager._terrainList4[i].transform.position = new Vector3(originPoint.x, originPoint.y, originPoint.z + (i * 100f));
                                    Debug.Log("Change1Complete");
                                    terrainLoopManager.usingListNum = 4;
                                }
                                break;
                            case 5:
                                for (int i = 0; i < terrainLoopManager._terrainList5.Count; i++)
                                {
                                    terrainLoopManager._terrainList5[i].SetActive(true);
                                    terrainLoopManager._terrainList5[i].transform.position = new Vector3(originPoint.x, originPoint.y, originPoint.z + (i * 100f));
                                    Debug.Log("Change1Complete");
                                    terrainLoopManager.usingListNum = 5;
                                }
                                break;
                            case 6:
                                for (int i = 0; i < terrainLoopManager._terrainList6.Count; i++)
                                {
                                    terrainLoopManager._terrainList6[i].SetActive(true);
                                    terrainLoopManager._terrainList6[i].transform.position = new Vector3(originPoint.x, originPoint.y, originPoint.z + (i * 100f));
                                    Debug.Log("Change1Complete");
                                    terrainLoopManager.usingListNum = 6;
                                }
                                break;
                        }
                    }

                    gameObject.SetActive(false);
                }
                else
                {
                    transform.position = originPoint;
                    terrainLoopManager.Refresh();
                }
                if (lastMap && isEnding)
                {
                    switch (terrainLoopManager.usingListNum)
                    {
                        case 1:
                            foreach (GameObject obj in terrainLoopManager._terrainList1)
                            {
                                ParkourLandMove landMove = obj.GetComponent<ParkourLandMove>();
                                landMove.end = true;
                            }
                            break;
                        case 2:
                            foreach (GameObject obj in terrainLoopManager._terrainList2)
                            {
                                ParkourLandMove landMove = obj.GetComponent<ParkourLandMove>();
                                landMove.end = true;
                            }
                            break;
                        case 3:
                            foreach (GameObject obj in terrainLoopManager._terrainList3)
                            {
                                ParkourLandMove landMove = obj.GetComponent<ParkourLandMove>();
                                landMove.end = true;
                            }
                            break;
                        case 4:
                            foreach (GameObject obj in terrainLoopManager._terrainList4)
                            {
                                ParkourLandMove landMove = obj.GetComponent<ParkourLandMove>();
                                landMove.end = true;
                            }
                            break;
                        case 5:
                            foreach (GameObject obj in terrainLoopManager._terrainList5)
                            {
                                ParkourLandMove landMove = obj.GetComponent<ParkourLandMove>();
                                landMove.end = true;
                            }
                            break;
                        case 6:
                            foreach (GameObject obj in terrainLoopManager._terrainList6)
                            {
                                ParkourLandMove landMove = obj.GetComponent<ParkourLandMove>();
                                landMove.end = true;
                            }
                            break;
                    }
                    isEnding = false;
                }
            }

        }
    }
}
