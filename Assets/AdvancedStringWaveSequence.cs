using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using UnityEngine;

[CreateAssetMenu(menuName = "Wave/Advanced String Wave Sequence")]
public class AdvancedStringWaveSequence : WaveSpawnController
{
    [System.Serializable]
    public class GroupEntry
    {
        public string groupName;
        [Min(1)] public int repeatCount = 1;
        [Min(0)] public float delayAfterGroup = 1f;
        public bool killAllEnemies = false;
    }
    [Tooltip("定義每個波次流程的名稱、重播次數與個別延遲")]
    public List<GroupEntry> waveSequence = new List<GroupEntry>();

    public override IEnumerator GenerateWave(WaveManager manager)
    {
        foreach (var entry in waveSequence)
        {
            for (int i = 0; i < entry.repeatCount; i++)
            {
                Debug.Log($"Wave Group: {entry.groupName}");
                yield return manager.StartCoroutine(manager.GetWave(entry.groupName));
            }
            if(entry.killAllEnemies)
            {
                yield return new WaitUntil(() => manager.EnemyCount() == 0);
            }
            else
            {
                yield return new WaitForSeconds(entry.delayAfterGroup);
            }
        }
        manager.BGMFadeOut();
        yield return new WaitForSeconds(2f);
        manager.GameFinish();
    }
}
