using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class WorldMapTile : WorldTile
{
    public LocationInfo locationInfo;
    public GameObject highlight,areaLocked;
    public Transform locationIndicator;
     float og ;
    void Awake(){
    
        highlight.SetActive(false);
        og = locationIndicator.localPosition.y;
    }

    public void Refresh( ){
        
        if(!MapTileManager.inst.debug){
          
        }
        
    }

    public void UnlockArea(){
        areaLocked.SetActive(false);
    }

    public void ToggleCurrentLocIndic(bool b)
    {
        locationIndicator.gameObject.SetActive(b);
        if(b){
q();
        }
        
        void q()
        {
       
            float c = locationIndicator.localPosition.y+.15f;
            locationIndicator.DOLocalMoveY(c,1f).OnComplete(()=>{
            locationIndicator.DOLocalMoveY(og,1f).OnComplete(()=>{
                q();
            });

            });

        }
       
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