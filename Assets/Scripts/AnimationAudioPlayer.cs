using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using DG.Tweening;
using System.Collections;

public class AnimationAudioPlayer : MonoBehaviour
{
    public SoundData sd;
    public void Play(){
        Debug.Log("STEP");
       // AudioManager.inst.GetSoundEffect().Play(sd);
    }
}