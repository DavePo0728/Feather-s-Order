using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SpawnGroupController : ScriptableObject
{
    public abstract IEnumerator GenerateGroup(WaveManager manager);
}
