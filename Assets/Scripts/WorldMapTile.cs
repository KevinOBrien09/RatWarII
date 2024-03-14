using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldMapTile : WorldTile
{
    public LocationInfo locationInfo;
    public GameObject highlight,areaLocked;
    void Awake(){
        locationInfo.mapTile = this;
        highlight.SetActive(false);
   
    }

    public void Refresh( ){
        
        if(!MapTileManager.inst.debug){
            if(locationInfo.stage.unlocked){
            areaLocked.SetActive(false);
            }
            else{
                    areaLocked.SetActive(true);
            }
        }
        
    }

    public void UnlockArea(){
        areaLocked.SetActive(false);
    }
        
        
        
    public override void Enter()
    {
     
        AudioManager.inst.GetSoundEffect().Play(SFX);
        highlight.SetActive(true);
    }

    public override void Click()
    {
       WorldMapCamera.inst.Focus(transform);
        HubStateHandler.inst.ChangeState(HubStateHandler.HubState.INMAPTILE);
        HubStateHandler.inst.ChangeStateString("Location");
        LocationDetailHandler.inst.Show(locationInfo);
    }

    public override void Exit()
    {
        highlight.SetActive(false);
    }

}