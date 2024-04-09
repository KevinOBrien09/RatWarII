using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;
using UnityEngine.EventSystems;

public class HubStateHandler: Singleton<HubStateHandler>
{
    public enum HubState {HOVER,QUEST,RECRUIT,ORGANIZER,LEAVE,MAP,INMAPTILE,PARTYEDIT,STATSHEET}
    public HubState currentState;

    public TextMeshProUGUI location,date,state;
    public Button leaveButton;
    public Transform hover;
    public UnityEvent close;
    public AudioClip mapMusic;
   
    IEnumerator Start(){
        ChangeState(HubState.HOVER);
        Cursor.lockState = CursorLockMode.Confined;
        MusicManager.inst.ChangeMusic(mapMusic);
        yield return new WaitForEndOfFrame();
        ChangeLocationName(MapTileManager.inst.ld[LocationManager.inst.currentLocation].locationInfo.locationName);
        MapTileManager.inst.RefreshCurrentLoc();
     
    }


    public void ChangeLocationName(string s){
        location.text = s;
    }
    public void ChangeState(HubState cs){
        currentState = cs;
        state.gameObject.SetActive(true);
        leaveButton.gameObject.SetActive(true);
    
        if(currentState == HubState.HOVER){
            state.gameObject.SetActive(false);
            leaveButton.gameObject.SetActive(false);
        }
    }

    public void ChangeStateString(string str){
    state.text = str;
    }
    void Update(){
        if(Input.GetKeyDown(KeyCode.X)){
            Leave();
        }
    }
    public void Leave(){
            EventSystem.current.SetSelectedGameObject(null);
        if(!WorldHubCamera.inst.fuckOff)
        {
            if(currentState == HubState.RECRUIT | currentState == HubState.QUEST)
            {
                CharacterRecruiter.inst.BackOut();
                return;
            }
            if(currentState == HubState.PARTYEDIT){
                if(BackbenchHandler.inst.CheckIfReady()){
                BackbenchHandler.inst.LeaveEdit();
                }
                return;
            }
            if(currentState == HubState.ORGANIZER)
            {
               
                if(!Draggable.isDragging)
                {
                    RetunToHover();
                }
                
               
            
                return;
            }
            if(currentState == HubState.MAP){
              
                WorldHubCamera.inst.Reset(hover);
                HubManager.inst.MapToHub();
                return;
            }
            if(currentState == HubState.INMAPTILE){

                HubStateHandler.inst.ChangeState(HubStateHandler.HubState.MAP);
                HubStateHandler.inst.ChangeStateString("Map");
                WorldMapCamera.inst.Reset(true);
                return;
            }
            if(currentState == HubState.STATSHEET){
                CharacterStatSheet.inst.Close();
                return;
            }
           
            if(currentState != HubState.HOVER)
            {RetunToHover();}
        }
        
    }

      public void RetunToHover()
    {
        if(close != null){
            close.Invoke();
            close = null;
        }
        WorldHubCamera.inst.fuckOff = true;
        WorldHubCamera.inst.Move(hover,(()=>{
        WorldHubCamera.inst.fuckOff = false;
        ChangeState(HubState.HOVER);
 
        }));
     
    }

}