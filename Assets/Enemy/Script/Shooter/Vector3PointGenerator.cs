using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
//[ExecuteInEditMode]
public class Vector3PointGenerator : MonoBehaviour
{
    public static Vector3PointGenerator instance;
    [SerializeField]
    public Vector3[][][] _3dArray;
    public List<Vector3> GeneratePointList = new List<Vector3>();
    public GameObject OriginPoint;
    public GameObject prefab;
    public float xSpacing, ySpacing, zSpacing;
    public int xCount, yCount, ZCount;
    void Awake()
    {
        instance = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
        GeneratePointList.Clear();
        _3dArray = new Vector3[xCount][][];
        for (int i = 0; i < xCount; i++)
        {
            _3dArray[i] = new Vector3[yCount][];
            for (int j = 0; j < yCount; j++)
            {
                _3dArray[i][j] = new Vector3[ZCount];
            }
        }
        GeneratePoint();
        SpawnPrefab();
    }
    // Update is called once per frame
    void Update()
    {

    }
    public void ReDraw(InputAction.CallbackContext context)
    {
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
        GeneratePointList.Clear();
        GeneratePoint();
        SpawnPrefab();
    }
    public void GeneratePoint()
    {
        if (OriginPoint == null)
        {
            Debug.LogError("OriginPoint is not set.");
            return;
        }

        for (int i = 0; i < xCount; i++)
        {
            for (int j = 0; j < yCount; j++)
            {
                for (int k = 0; k < ZCount; k++)
                {
                    GeneratePointList.Add(new Vector3(
                        OriginPoint.transform.position.x + i * xSpacing,
                        OriginPoint.transform.position.y - j * ySpacing,
                        OriginPoint.transform.position.z - k * zSpacing));
                    _3dArray[i][j][k] = new Vector3(
                        OriginPoint.transform.position.x + i * xSpacing,
                        OriginPoint.transform.position.y - j * ySpacing,
                        OriginPoint.transform.position.z - k * zSpacing);
                }
            }
        }
        
    }
    void SpawnPrefab()
    {
        foreach (Vector3 point in GeneratePointList)
        {
            GameObject temp = Instantiate(prefab, point, Quaternion.identity);
            temp.transform.parent = transform;
        }
    }
    public Vector3 GetPoint(int x, int y, int z)
    {
        return _3dArray[x][y][z];
    }
    public Vector3[] GetMoveCPathList(int Length) 
    {
        Vector3[] pathList = new Vector3[Length];
        for (int i = 0; i < Length; i++)
        {
            int RandomX = Random.Range(0, xCount);
            int RandomY = Random.Range(0, yCount);
            int RandomZ = Random.Range(0, ZCount);
            pathList[i] = GetPoint(RandomX, RandomY, RandomZ);
        }
        return pathList;
    }
    //private void OnDrawGizmos()
    //{
    //    if(GeneratePointList.Count != 0)
    //    {
    //        Gizmos.color = Color.red;
    //        Gizmos.DrawWireSphere(GeneratePointList[500], 10f);
    //        for (int i = 0; i < GeneratePointList.Count; i++)
    //        {
    //            Gizmos.DrawWireSphere(GeneratePointList[i], 0.1f);
    //        }
    //    }


    //}
}

