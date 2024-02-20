using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class InteractHandler : Singleton<InteractHandler>
{
    bool inCoro;
  public  List<Slot> slots = new List<Slot>();
  public bool bleh;
    public void Open()
    {
        GameManager.inst.ChangeGameState(GameState.INTERACT);
      
        BattleTicker.inst.Type("Interact");
        
        slots =BattleManager.inst.currentUnit.slot.func.GetRadiusSlots(1,CharacterBuilder.inst.mandatorySkills[0],false);//dumb
        Cursor.lockState = CursorLockMode.Confined;
        foreach (var item in slots)
        {
          item .ChangeColour(item.interactColour);
        }
    }

    public void TakeInteraction(Slot slot)
    {
        foreach (var item in slots)
        {item.ChangeColour(item.normalColour);}
        slots.Clear();
        BattleManager.inst.currentUnit .Flip(slot. transform.position);
                  bleh = true;
        SlotInfoDisplay.inst.Disable();
        Cursor.lockState = CursorLockMode.Locked;
        if(slot.cont.unit != null)
        {
            if(slot.cont.unit.side == Side.PLAYER){
            BattleTicker.inst.Type("It's just " + slot.cont.unit.character.characterName.firstName);
            }
            else if(slot.cont.unit.side == Side.ENEMY)
            {
             BattleTicker.inst.Type("An evil " + slot.cont.unit.character.characterName.firstName + "!");
            }
            else if(slot.cont.unit.isHostage)
            {
                slot.cont.specialSlot.interactable.Go(BattleManager.inst.currentUnit);
            }
        }
        else if(slot.cont.specialSlot != null)
        {
            if(slot.cont.specialSlot.interactable == null){
                BattleTicker.inst.Type(slot.cont.specialSlot.slotContents.name);
            }
            else{
                slot.cont.specialSlot.interactable.Go(BattleManager.inst.currentUnit);
     
                return;
            }
        
        }
        else if(slot.cont.slotContents.Count > 0){
            BattleTicker.inst.Type(slot.cont.slotContents[Random.Range( 0,slot.cont.slotContents.Count)].contentDesc);
        }
        else{
            BattleTicker.inst.Type("Nothing of note here.");
        }
        
        StartCoroutine(q());
        IEnumerator q()
        {
            yield   return new WaitForSeconds(1);
            slot.DisableHover();
            bleh =false;
            ExitSelectionMode();
            ActionMenu.inst.Show(BattleManager.inst.currentUnit.slot);
        }

    }

    void LateUpdate()
    {
        if(!bleh){
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



    
}