using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class InteractHandler : Singleton<InteractHandler>
{
    bool inCoro;
  public  List<Slot> slots = new List<Slot>();
    public void Open()
    {
        GameManager.inst.ChangeGameState(GameState.INTERACT);
      
        BattleTicker.inst.Type("Interact");
        
        slots =BattleManager.inst.currentUnit.slot.func.GetRadiusSlots(1,null,true);
        Cursor.lockState = CursorLockMode.Confined;
        foreach (var item in slots)
        {
          item .ChangeColour(item.interactColour);
        }
    }

    void LateUpdate()
    {
        if(GameManager.inst.checkGameState(GameState.INTERACT) && !inCoro && !ActionMenu.inst.FUCKOFF)
        {
            if(InputManager.inst.player. GetButtonDown("Cancel"))
            {
                ExitSelectionMode();
                ActionMenu.inst.Show(BattleManager.inst.currentUnit.slot);
                //CamFollow.inst.ChangeCameraState(CameraState.FREE);
            }
        }
    }

    public void ExitSelectionMode()
    {
        if(CamFollow.inst.CheckCameraState(CameraState.FOCUS))
        {return; }

        if( GameManager.inst.checkGameState(GameState.INTERACT) && !inCoro)
        {
            StartCoroutine(q());
        }
        IEnumerator q()
        {      
            
            foreach (var item in slots)
            {
                item.ChangeColour(item.normalColour);
            }
            slots.Clear();
            inCoro = true;
            yield return new WaitForSeconds(.15f);
            inCoro = false;
      
        }
    }



    public void Close(){

    }
}