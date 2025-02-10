using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WingFlapSound : StateMachineBehaviour
{
    [SerializeField]
    GameObject soundManager;
    private AudioSource wingFlapAudio;

    // 角色進入這個動畫狀態時執行
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (wingFlapAudio == null)
        {
            soundManager = GameObject.FindGameObjectWithTag("SoundManager");
            wingFlapAudio = soundManager.transform.GetChild(6).GetComponent<AudioSource>();
        }

        if (wingFlapAudio != null)
        {
            wingFlapAudio.Play();
        }
    }
}
