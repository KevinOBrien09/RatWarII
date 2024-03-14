using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class HubManager: Singleton<HubManager>
{

    public GameObject map,hub;
    public WorldLeaveArea worldLeaveArea;
    public AudioClip mapMusic,cityMusic;

    void Start(){
        map.gameObject.SetActive(false);
            hub.gameObject.SetActive(true);  
    }

    public void HubToMap(){
        MusicManager.inst.FadeAndChange(mapMusic);
        HubStateHandler.inst.ChangeState(HubStateHandler.HubState.MAP);
        HubStateHandler.inst.ChangeStateString("Map");
        map.gameObject.SetActive(true);
        hub.gameObject.SetActive(false);
        BlackFade.inst.FadeOut(.2f);
    }

    public void MapToHub(){
          WorldMapCamera.inst.Reset(true);
        MusicManager.inst.FadeAndChange(cityMusic);
        BlackFade.inst.FadeInEvent(()=>{
            worldLeaveArea.Reset();
            HubStateHandler.inst.ChangeState(HubStateHandler.HubState.HOVER);
        
            map.gameObject.SetActive(false);
            hub.gameObject.SetActive(true);
            BlackFade.inst.FadeOut(.2f);

        });
       
    }

}