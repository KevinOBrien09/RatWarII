using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class OverworldClickable: MonoBehaviour,IPointerClickHandler
{
   public OverworldUnit owner;
   public void OnPointerClick(PointerEventData eventData)
   {
      if(eventData.button ==PointerEventData.InputButton.Left){
 if(PartyController.inst.selected != owner && PartyController.inst.run){
         PartyController.inst.ChangeSelected(owner);
      }
      }
     
     
   }
}