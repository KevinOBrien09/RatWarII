using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldMap : Singleton<WorldMap>
{
    public AudioClip mapMusic;

    public void Start(){
        MusicManager.inst.ChangeMusic(mapMusic);
    }
    
}
