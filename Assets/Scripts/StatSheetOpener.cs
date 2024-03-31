using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections.Generic;
public class StatSheetOpener : MonoBehaviour ,IPointerClickHandler
{

   public Character character;
    public virtual void Init(Character c){
    character = c;
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        if(character != null){
if(!Draggable.isDragging){
        if(eventData.button == PointerEventData.InputButton.Right){
        CharacterStatSheet.inst.Open(character);
        Debug.Log("RightClick");
       }
        }
        }
        
     
    }
}