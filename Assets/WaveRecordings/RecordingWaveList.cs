using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "New RecordingWaveList", menuName = "Wave/New RecordingWaveList")]
public class RecordingWaveList : ScriptableObject
{
    public List<RecordedWaveData> recordingWaves;
}
