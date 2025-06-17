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
        public bool specialTutorial;
    }
    [Tooltip("定義每個波次流程的名稱、重播次數與個別延遲")]
    public List<TutorialWaveEntry> tutorialSteps;
    public override IEnumerator GenerateWave(WaveManager manager)
    {
        manager.PreteachImageFadeIn();
        //yield return manager.WaitForPressBInput();
        manager.PreteachImageFadeOut();
        foreach (var step in tutorialSteps)
        {
            Debug.Log($"Tutorial Step: {step.waveGroupName}");
            manager.FadeAndSetTutorialImage(step.tutorialImage);
            manager.StartCoroutine(manager.scenesManager.Fade(manager.tutorialImage, 0f, 1f, 1f));
            if (step.specialTutorial)
            {
                //yield return manager.WaitForContinueInput();
                manager.StartCoroutine(manager.scenesManager.Fade(manager.tutorialImage, 1f, 0f,1f));
            }
            
            yield return manager.StartCoroutine(manager.GetWave(step.waveGroupName));
            yield return new WaitUntil(() => manager.EnemyCount() == 0);
            manager.StartCoroutine(manager.scenesManager.Fade(manager.tutorialImage, 1f, 0f, 1f));
        }
        manager.BGMFadeOut();
        yield return new WaitForSeconds(2.5f);
        manager.GameFinish();
    }
}
