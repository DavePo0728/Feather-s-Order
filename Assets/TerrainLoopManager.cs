using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class TerrainLoopManager : MonoBehaviour
{
    public static TerrainLoopManager terrainInstance;
    [SerializeField]
    List<GameObject> terrainList,blockList,specialList;
    [SerializeField]
    List<Vector3> InitialPos;
    [SerializeField]
    float OriginSpeed;
    [SerializeField]
    float moveSpeed;
    public float _moveSpeed => moveSpeed;
    private void Start()
    {
        
    }
    private void Update()
    {

    }
}
