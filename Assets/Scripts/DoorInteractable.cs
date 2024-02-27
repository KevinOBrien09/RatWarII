using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
public class DoorInteractable : Interactable
{
  
    public Door door;
    public SoundData open,close;
    public override void Go(Unit investigator)
    {
        if(investigator.slot.room == door.roomA)
        {
            if(BattleManager.inst.roomLockDown)
            {StartCoroutine(doorIsLocked());}
            else
            {MoveRoom(door.roomB);}
            
            //Debug.Log("Room " +door. border.roomA.roomID + "to Room " + door. border.roomB.roomID);;
        }
        else
        {  
            if(BattleManager.inst.roomLockDown){
                 StartCoroutine(doorIsLocked());
            }
              else
            {MoveRoom(door.roomA);}
          
            //Debug.Log("Room " +door. border.roomB.roomID + "to Room " + door. border.roomA.roomID);;
        }

        IEnumerator doorIsLocked(){
            slot.DisableHover();
            BattleTicker.inst.Type("The door is locked!");
            yield return new WaitForSeconds(1);
           
            InteractHandler.inst. bleh = false;
            InteractHandler.inst. ExitSelectionMode();
            ActionMenu.inst.Show(BattleManager.inst.currentUnit.slot);

        }

        void MoveRoom(Room room)
        {
            BattleTicker.inst.Type("Changing Room");
            CamFollow.inst.enabled = false;
            for (int i = 0; i < BattleManager.inst.playerUnits.Count; i++)
            {BattleManager.inst.playerUnits[i].graphic.gameObject.SetActive(false);}
            AudioManager.inst.GetSoundEffect().Play(open);
            MapManager.inst.ChangeRoom(room,door);
            slot.DisableHover();
        }
    }
}