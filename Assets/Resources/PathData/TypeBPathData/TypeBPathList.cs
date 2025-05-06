using PathCreation;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New TypeBPathList", menuName = "PathData/New TypeBPathList")]

public class TypeBPathList : ScriptableObject
{
    public List<PathCreator> typeBPathCreatorPathList;
}
