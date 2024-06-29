
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;

public class KeyPickup : BushInteractable
{
    public Outline ol;
    public Color32 inRangeColour,outOfRangeColour;
    public override void Go()
    {
        PartyController.inst.run = false;
        var itemData = itemContainer .RetrieveItems();
        InspectionResult.inst.LoadItems(itemData.Item2, ()=>{
            foreach (var item in itemData.Item2)
            {
               InventoryManager.inst.inventory.AddItem(item);
               PartyController.inst.run = true;
            }
       
        });
        Destroy(gameObject);
    }

    public override void OutlineToggle(bool state,bool inRange = false)
    { 
        if(!itemContainer.itemsGone){
            if(state)
            {
                if(inRange)
                {
                    ol.enabled = true;
                    ol.OutlineColor = inRangeColour;
                    
                }
                else
                {
                    ol.enabled = true;
                    ol.OutlineColor = outOfRangeColour;
                }
            }
            else
            {
                ol.enabled = false;
               
            }
        }
    }

    public override void OnPointerEnter(PointerEventData eventData)
    {
        if(!itemContainer.itemsGone){
        base.OnPointerEnter(eventData);
        }
   
    }

    public override void OnPointerExit(PointerEventData eventData)
    {  
        if(!itemContainer.itemsGone){
       base.OnPointerExit(eventData);
        }
    }
}