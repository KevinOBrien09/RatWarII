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
    float ogVol;
    void Start(){
        ogVol = source.volume;
    }    
    public void ChangeMusic(AudioClip c)
    {
        source.Stop();
        source.clip = c;
        source.Play();
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

}