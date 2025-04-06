using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParkourTerrainLoopManager : MonoBehaviour
{
    [HideInInspector]
    public int usingListNum;
    public static ParkourTerrainLoopManager terrainInstance;
    [SerializeField]
    List<GameObject> terrainList1, terrainList2, terrainList3, terrainList4, terrainList5, terrainList6;
    [HideInInspector]
    public List<GameObject> _terrainList1 => terrainList1;
    public List<GameObject> _terrainList2 => terrainList2;
    public List<GameObject> _terrainList3 => terrainList3;
    public List<GameObject> _terrainList4 => terrainList4;
    public List<GameObject> _terrainList5 => terrainList5;
    public List<GameObject> _terrainList6 => terrainList6;
    [HideInInspector]
    public Vector3 InitialPos;
    [SerializeField]
    float loopRoundNum;
    [SerializeField]
    float loopNum = 1;
    private void Awake()
    {
        terrainInstance = this;
    }
    private void Start()
    {


    }
    private void FixedUpdate()
    {
        //float temp = terrainList1[0].transform.position.z - terrainList1[1].transform.position.z;
        //Debug.Log(terrainList[5].name +" "+ terrainList[5].transform.position.z + "-" + terrainList[6].name +" " + terrainList[6].transform.position.z + ": " + temp);
        //.Log(terrainList[5].name + terrainList[6].name + ": " + temp);
    }
    public void Refresh()
    {
        if (loopNum < 8)
        {
            loopNum++;
        }
        else if (loopNum == 8)
        {
            loopNum = 1;
            loopRoundNum++;
        }
    }
    public void ChangeMap(int listNum)
    {
        if (usingListNum == listNum) return;

        switch (usingListNum)
        {
            case 1:
                ParkourLandMove landMove = terrainList1[terrainList1.Count - 1].GetComponent<ParkourLandMove>();
                landMove.isEnding = true;
                ParkourLandMove firstLandMove = terrainList1[0].GetComponent<ParkourLandMove>();
                firstLandMove.nextListNum = listNum;

                break;
            case 2:
                ParkourLandMove landMove2 = terrainList2[terrainList2.Count - 1].GetComponent<ParkourLandMove>();
                landMove2.isEnding = true;
                ParkourLandMove firstLandMove2 = terrainList2[0].GetComponent<ParkourLandMove>();
                firstLandMove2.nextListNum = listNum;

                break;
            case 3:
                ParkourLandMove landMove3 = terrainList3[terrainList3.Count - 1].GetComponent<ParkourLandMove>();
                landMove3.isEnding = true;
                ParkourLandMove firstLandMove3 = terrainList3[0].GetComponent<ParkourLandMove>();
                firstLandMove3.nextListNum = listNum;

                break;
            case 4:
                ParkourLandMove landMove4 = terrainList4[terrainList4.Count - 1].GetComponent<ParkourLandMove>();
                landMove4.isEnding = true;
                ParkourLandMove firstLandMove4 = terrainList4[0].GetComponent<ParkourLandMove>();
                firstLandMove4.nextListNum = listNum;
                break;
            case 5:
                ParkourLandMove landMove5 = terrainList5[terrainList5.Count - 1].GetComponent<ParkourLandMove>();
                landMove5.isEnding = true;
                ParkourLandMove firstLandMove5 = terrainList5[0].GetComponent<ParkourLandMove>();
                firstLandMove5.nextListNum = listNum;
                break;
            case 6:
                ParkourLandMove landMove6 = terrainList6[terrainList6.Count - 1].GetComponent<ParkourLandMove>();
                landMove6.isEnding = true;
                ParkourLandMove firstLandMove6 = terrainList6[0].GetComponent<ParkourLandMove>();
                firstLandMove6.nextListNum = listNum;
                break;
        }

    }
}
