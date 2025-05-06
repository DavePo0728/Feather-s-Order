using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New TypeCPathList", menuName = "PathData/New TypeCPathList")]

public class CustomPathDataList : ScriptableObject
{
    public List<CustomPathData> customPathDataList;
}
