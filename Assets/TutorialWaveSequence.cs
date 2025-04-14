using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

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

    [Header("前制教學流程設定")]
    public CanvasGroup preTutorialImage;
    public CanvasGroup confirmHintImage;
    public float fadeDuration = 1f;
    public float delayBeforeHint = 2f;
    public float holdDuration = 2f;

    private float holdTimer = 0f;
    private bool tutorialStarted = false;

    public override IEnumerator GenerateWave(WaveManager manager)
    {
        // 前制教學流程
        preTutorialImage.alpha = 0;
        confirmHintImage.alpha = 0;
        preTutorialImage.gameObject.SetActive(true);
        confirmHintImage.gameObject.SetActive(false);

        yield return FadeCanvasGroup(preTutorialImage, 0f, 1f, fadeDuration);
        yield return new WaitForSeconds(delayBeforeHint);

        confirmHintImage.gameObject.SetActive(true);
        yield return FadeCanvasGroup(confirmHintImage, 0f, 1f, fadeDuration);

        // 長按偵測
        while (!tutorialStarted)
        {
            if (Gamepad.current != null && Gamepad.current.buttonEast.isPressed)
            {
                holdTimer += Time.deltaTime;
                if (holdTimer >= holdDuration)
                {
                    tutorialStarted = true;
                    break;
                }
            }
            else
            {
                holdTimer = 0f;
            }
            yield return null;
        }

        yield return FadeCanvasGroup(confirmHintImage, 1f, 0f, fadeDuration);
        yield return FadeCanvasGroup(preTutorialImage, 1f, 0f, fadeDuration);
        preTutorialImage.gameObject.SetActive(false);
        confirmHintImage.gameObject.SetActive(false);

        // 教學主流程
        foreach (var step in tutorialSteps)
        {
            Debug.Log($"Tutorial Step: {step.waveGroupName}");
            manager.StartCoroutine(manager.FadeAndSetTutorialImage(step.tutorialImage));
            //yield return manager.WaitForContinueInput();
            yield return manager.StartCoroutine(manager.GetWave(step.waveGroupName));
            yield return new WaitUntil(() => manager.EnemyCount() == 0);
        }

        manager.BGMFadeOut();
        yield return new WaitForSeconds(2f);
        manager.GameFinish();
    }

    private IEnumerator FadeCanvasGroup(CanvasGroup canvas, float from, float to, float duration)
    {
        float timer = 0f;
        canvas.alpha = from;
        canvas.gameObject.SetActive(true);
        while (timer < duration)
        {
            timer += Time.deltaTime;
            canvas.alpha = Mathf.Lerp(from, to, timer / duration);
            yield return null;
        }

        canvas.alpha = to;
        if (to == 0f)
            canvas.gameObject.SetActive(false);
    }
}
