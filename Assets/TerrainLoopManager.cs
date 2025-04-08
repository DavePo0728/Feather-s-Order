using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class TerrainLoopManager : MonoBehaviour
{
    [HideInInspector]
    public int usingListNum;
    public static TerrainLoopManager terrainInstance;
    [SerializeField]
    List<GameObject> terrainList,outsideLeftList,outSideRightList,blockList,specialList;
    [SerializeField]
    List<GameObject> terrainList2,terrainList3;
    [HideInInspector]
    public Vector3 InitialPos;
    [SerializeField]
    float loopRoundNum;
    [SerializeField]
    float loopNum = 1;
    public List<GameObject> _terrainList=>terrainList;
    public List<GameObject> _terrainList2 => terrainList2;
    public List<GameObject> _terrainList3 => terrainList3;
    public List<GameObject> _outSideLeftList => outsideLeftList;
    public List<GameObject> _outSideRightList => outSideRightList;
    private void Awake()
    {
        terrainInstance = this;
    }
    private void Start()
    {
        
        
    }
    private void FixedUpdate()
    {
        //float temp = terrainList[0].transform.position.z - terrainList[1].transform.position.z;
        //Debug.Log(terrainList[5].name +" "+ terrainList[5].transform.position.z + "-" + terrainList[6].name +" " + terrainList[6].transform.position.z + ": " + temp);
        //.Log(terrainList[5].name + terrainList[6].name + ": " + temp);
    }
    public void Refresh()
    {
        if (loopNum < 6)
        {
            loopNum++;
        }
        else if (loopNum == 6)
        {
            loopNum = 1;
            loopRoundNum++;
        }
    }
    public void ChangeMap(int listNum)
    {
        if (usingListNum == listNum) return;
        Debug.Log("ChangeMap");
        switch (usingListNum)
        {
            case 1:
                LandMove landMove = terrainList[terrainList.Count - 1].GetComponent<LandMove>();
                landMove.isEnding = true;
                LandMove firstLandMove = terrainList[0].GetComponent<LandMove>();
                firstLandMove.nextListNum = listNum;
                
                break;
            case 2:
                LandMove landMove2 = terrainList2[terrainList2.Count - 1].GetComponent<LandMove>();
                landMove2.isEnding = true;
                LandMove firstLandMove2 = terrainList2[0].GetComponent<LandMove>();
                firstLandMove2.nextListNum = listNum;
                
                break;
            case 3:
                LandMove landMove3 = terrainList3[terrainList3.Count - 1].GetComponent<LandMove>();
                landMove3.isEnding = true;
                LandMove firstLandMove3 = terrainList3[0].GetComponent<LandMove>();
                firstLandMove3.nextListNum = listNum;

                break;
        }

    }
}
