using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChargeAudioPlayer : MonoBehaviour
{
	public AudioSource audioSource1;
	public AudioSource audioSource2;

	private int callCount = 0;
	private AudioSource currentPlaying;

	// 呼叫這個來播放聲音
	public void Play()
	{
		// 如果有正在播放的聲音，先停止
		Stop();

		callCount++;

		if (callCount == 1)
		{
			currentPlaying = audioSource1;
		}
		else
		{
			currentPlaying = audioSource2;
		}

		if (currentPlaying != null)
		{
			currentPlaying.Play();
		}
	}

	// 呼叫這個來中斷當前聲音
	public void Stop()
	{
		if (currentPlaying != null && currentPlaying.isPlaying)
		{
			currentPlaying.Stop();
		}
		currentPlaying = null;
	}

	// 如果你想重置計數器，可以呼叫這個
	public void ResetPlayState()
	{
		callCount = 0;
		Stop();
	}
}
