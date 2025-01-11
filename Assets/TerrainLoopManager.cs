using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class TerrainLoopManager : MonoBehaviour
{
    public static TerrainLoopManager terrainInstance;
    [SerializeField]
    List<GameObject> terrainList,blockList,specialList;
    Vector3 InitialPos = new Vector3(2050,-25f,6100f);
    [SerializeField]
    float loopRoundNum = 0;
    [SerializeField]
    float loopNum = 0;
    private void Start()
    {
        terrainInstance = this;
        
    }
    private void FixedUpdate()
    {
        float temp = terrainList[0].transform.position.z - terrainList[1].transform.position.z;
        //Debug.Log(terrainList[5].name +" "+ terrainList[5].transform.position.z + "-" + terrainList[6].name +" " + terrainList[6].transform.position.z + ": " + temp);
        //.Log(terrainList[5].name + terrainList[6].name + ": " + temp);
    }
    public void Refresh()
    {
        if (loopNum < 7)
        {
            loopNum++;
        }
        else if (loopNum == 7)
        {
            loopNum = 1;
            loopRoundNum++;
        }
    }
}
