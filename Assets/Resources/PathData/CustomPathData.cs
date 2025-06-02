using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "New PathData", menuName = "PathData/New PathData")]
public class CustomPathData : ScriptableObject
{
    public List<int> pathX;
    public List<int> pathY;
    public List<int> pathZ;
    public int loopTime;
    public int loopStartIndex;
    public int loopEndIndex;
    public List<moveTimeValue> moveTimeValues;
}
[System.Serializable]
public class moveTimeValue
{
    public int pointIndex;
    public float moveTime;
    public float pointWaitTime;
    public float endpointWaitTime;
}
