using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
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
        WorldHubCamera.inst.transform.DORotate(new Vector3(90,0,0) ,.15f);
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
              WorldHubCamera.inst.transform.DORotate(new Vector3(0,0,0) ,.15f);
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
