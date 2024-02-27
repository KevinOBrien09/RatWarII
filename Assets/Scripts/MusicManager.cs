using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class MusicManager : Singleton<MusicManager>
{
    public AudioClip battle,peace;
    public AudioSource source;
    
    public void ChangeMusic(AudioClip c)
    {
        source.Stop();
        source.clip = c;
        source.Play();
    }

}