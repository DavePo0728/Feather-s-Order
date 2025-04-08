using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class WaveSpawnController : ScriptableObject
{
    public abstract IEnumerator GenerateWave(WaveManager manager);
}

