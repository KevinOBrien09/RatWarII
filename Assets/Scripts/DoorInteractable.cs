using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
public class DoorInteractable : Interactable
{
  
    public Door door;
    public override void Go(Unit investigator)
    {
       if(investigator.slot.room == door.roomA){
 Debug.Log("Room " +door. border.roomA.roomID + "to Room " + door. border.roomB.roomID);;
       }
       else
       {
  Debug.Log("Room " +door. border.roomB.roomID + "to Room " + door. border.roomA.roomID);;
       }
       
    }
}