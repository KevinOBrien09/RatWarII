using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldMap : Singleton<WorldMap>
{
    public enum MapState{OVERVIEW,CAPITALCITY}
    public MapState currentState;
    public AudioClip mapMusic;
    public AudioSource cityAmbi;
    public GameObject cityGO,mapGO;
    public void Start(){
        mapGO.SetActive(false);
        cityGO.SetActive(true);
        MusicManager.inst.ChangeMusic(mapMusic);
        
    }

    void Update()
    {
        if(InputManager.inst.player.GetButtonDown("Cancel"))
        {
            if(currentState == MapState.CAPITALCITY && WorldCity.inst.currentState == WorldCity.CityState.HOVER && !WorldHubCamera.inst.fuckOff)
            {ExitCity();}
        }
    }
    public void SwapState(MapState newState){
        if(currentState != newState){
            currentState = newState;
        }
    }

    public void ExitCity()
    {
        WorldHubCamera.inst.ZoomOut(()=>
        {
            cityGO.SetActive(false);
            cityAmbi.Stop();
            mapGO.SetActive(true);
            StartCoroutine(q());
            IEnumerator q(){
                yield return new WaitForSeconds(.175f);
                SwapState(MapState.OVERVIEW) ;
            }
        });
    }


    public void SwapToCity()
    {
        WorldHubCamera.inst.Zoom(()=>{
             
            cityGO.SetActive(true);
            cityAmbi.Play();
            mapGO.SetActive(false);
            StartCoroutine(q());
            IEnumerator q(){
                yield return new WaitForSeconds(.175f);
                SwapState(MapState.CAPITALCITY);
            }
            
        });
    }
    
}
