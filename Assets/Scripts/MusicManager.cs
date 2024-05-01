using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using DG.Tweening;

public class MusicManager : Singleton<MusicManager>
{
    public AudioClip battle,peace;
    public AudioSource source;
    public AudioSource ambience;
   public float ogVol;
    void Start(){
        ogVol = source.volume;
    }    
    public void ChangeMusic(AudioClip c)
    {
        source.Stop();
        source.clip = c;
        source.Play();
    }

    public void ChangeAmbience(AudioClip c){
        ambience.Stop();
        ambience.clip = c;
        ambience.volume = ogVol;
        ambience.Play();
    }

    public void FadeAndChange(AudioClip c,float t = .5f){

        source.DOFade(0,t).OnComplete(()=>
        {
            if(c != null){
                source.Stop();
                source.clip = c;
                source.Play();
                source.DOFade(ogVol,t);
            }
          

        });
    }

    public void ResetVol(){
        source.volume = ogVol;
    }

    public void FadeToSilence(float t = .5f){
        source.DOFade(0,t);
        ambience.DOFade(0,t);
    }

}