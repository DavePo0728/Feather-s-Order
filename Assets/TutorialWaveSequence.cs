using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
[CreateAssetMenu(menuName = "Wave/TutorialWaveSequence")]
public class TutorialWaveSequence : WaveSpawnController
{
    [System.Serializable]
    public class TutorialWaveEntry
    {
        public Sprite tutorialImage;
        public string waveGroupName; // 對應 Dictionary 的 key，例如 "Intro"
    }
    [Tooltip("定義每個波次流程的名稱、重播次數與個別延遲")]
    public List<TutorialWaveEntry> tutorialSteps;
    public override IEnumerator GenerateWave(WaveManager manager)
    {
        foreach (var step in tutorialSteps)
        {
            Debug.Log($"Tutorial Step: {step.waveGroupName}");
            manager.StartCoroutine(manager.FadeAndSetTutorialImage(step.tutorialImage));
            yield return manager.StartCoroutine(manager.GetWave(step.waveGroupName));
            yield return new WaitUntil(() => manager.EnemyCount() == 0);
        }

        manager.GameFinish();
    }
}
