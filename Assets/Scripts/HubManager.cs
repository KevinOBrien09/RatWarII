using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class HubManager: Singleton<HubManager>
{

    public GameObject map,hub;
   
    public AudioClip mapMusic,cityMusic;
    public WorldLocationDeco currentDeco;
    public Transform decoHolder;

    protected override void Awake()
    {
        base.Awake();
        if(GameManager.inst. loadFromFile){
            GameManager.inst.Load();
        }

    }
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
    
            HubStateHandler.inst.ChangeState(HubStateHandler.HubState.HOVER);
        
            map.gameObject.SetActive(false);
            hub.gameObject.SetActive(true);
            BlackFade.inst.FadeOut(.2f);

        });
       
    }

    
    public void SpawnNewDeco(LocationInfo locInfo){
        if(currentDeco != null){
            Destroy(currentDeco.gameObject);
            currentDeco = null;
        }
           LocationManager.inst.locName = locInfo.locationName;
        if(locInfo.locationMusic != null)
        {
          cityMusic = locInfo.locationMusic;
      
            MusicManager.inst.FadeAndChange(cityMusic);
          
         
        }
        if(locInfo.decoPrefab != null)
        {
            currentDeco = Instantiate(locInfo.decoPrefab,decoHolder);
            

        }
        else{
            Debug.LogWarning("Deco Prefab is Null!!");
        }
      
    }

}