using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class LoadindAnim : MonoBehaviour
{
	public static LoadindAnim instance;
	void Awake()
	{
		if (instance == null)
		{
			instance = this;
		}
	}
	public ScenesManager scenesManager;

	public ScoreManager scoreManager;
	public WaveManager waveManager;
	public bool Gamestarted = false;
	public AudioSource EVaudioSource;
	public AudioSource BGMaudioSource;
	public ParticleSystem B1;
	public ParticleSystem B2;
	public Image bar1;
	public Image bar2;
	public float BarDuration = 1.0f;
	public AudioSource MeunBGM;
	public Material FS_Speed;
	public bool LoadOver =false;
	void Start()
	{
		if (scenesManager.scenesNum == ScenesNum.BattleScene)
		{
			Color c1 = bar1.color;
			c1.a = 255f;
			bar1.color = c1;
			Color c2 = bar2.color;
			c2.a = 255f;
			bar2.color = c2;
			StartCoroutine(DelayedStart());
		}
		if (scenesManager.scenesNum == ScenesNum.StartScene)
		{
			MeunBGM.Play();
			B1.Play();
			B2.Play();
		}
	}

	public void floatIn()
	{
		StartCoroutine(Fade(bar1, 0f, 1f, BarDuration));
		StartCoroutine(Fade(bar2, 0f, 1f, BarDuration));
	}

	public void floatOut()
	{
		StartCoroutine(Fade(bar1, 1f, 0f, BarDuration));
		StartCoroutine(Fade(bar2, 1f, 0f, BarDuration));
	}

	IEnumerator Fade(Image img, float fromAlpha, float toAlpha, float time)
	{
		Color c = img.color;
		float t = 0f;

		while (t < time)
		{
			c.a = Mathf.Lerp(fromAlpha, toAlpha, t / time);
			img.color = c;
			t += Time.deltaTime;
			yield return null;
		}

		// 確保最終透明度正確
		c.a = toAlpha;
		img.color = c;
	}
	IEnumerator DelayedStart()
	{
		yield return new WaitForSeconds(1f); // 延遲1秒
		scenesManager.EndLoading();
		
	}

	public void StartGame()
	{
		Gamestarted = true;
		gameObject.SetActive(false);
		waveManager.StartGenerateWave();
		FS_Speed.SetFloat("_enabled", 1f);  
		EVaudioSource.Play();
		BGMaudioSource.Play();
	}
	public void StartFadeOutMusic()
	{

		StartCoroutine(FadeOutMusic(MeunBGM, 1f));

	}
	IEnumerator FadeOutMusic(AudioSource audio, float duration)
	{
		float startVolume = audio.volume;

		for (float t = 0; t < duration; t += Time.deltaTime)
		{
			audio.volume = Mathf.Lerp(startVolume, 0f, t / duration);
			yield return null;
		}

		audio.Stop();
	}
	private void Update()
	{
		//if (Input.anyKeyDown && Gamestarted ==false && LoadOver)
		//{
		//	Gamestarted = true;
		//	StartGame();
		//}
	}
}
