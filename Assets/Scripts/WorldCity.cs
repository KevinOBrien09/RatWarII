using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
public class WorldCity: Singleton<WorldCity>
{
    public enum CityState {HOVER,QUEST,RECRUIT}
    public CityState currentState;
    public GameObject desktopButton;
    
    public Transform hover;
    public UnityEvent close;
    public void ChangeState(CityState cs){
        currentState = cs;
    }
   
    void Update()
    {
        if(InputManager.inst.player.GetButtonDown("Cancel"))
        {
            if(!WorldHubCamera.inst.fuckOff)
            {
                if(WorldCity.inst.currentState == CityState.RECRUIT)
                {
                    CharacterRecruiter.inst.BackOut();
                    return;
                }
                if(WorldCity.inst.currentState != CityState.HOVER)
                {RetunToHover();}
            }
        }

        
    }

    public void QuitGame(){
        Application.Quit();
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
        currentState = CityState.HOVER;
        desktopButton.SetActive(true);
        }));
     
    }

}