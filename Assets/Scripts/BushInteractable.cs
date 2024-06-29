
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;

public class BushInteractable : Interactable
{
    public GameObject outOfRangeOutline;
    public Zoomer zoomer;
    public ItemContainer itemContainer;
    public override void Go()
    {
        if(PartyController.inst.selected.battleUnit.character.job == Job.ARCHER){
            if(!itemContainer.itemsGone)
            {
                BushInteractZoomer biz = Instantiate(zoomer) as BushInteractZoomer;
                biz.AttachToOverworld();
                biz.InteractZoom(itemContainer,PartyController.inst.selected);

            }
        }
        else{
            Debug.Log("Not an archer");
        }
        
    }
    
    public override void OutlineToggle(bool state,bool inRange = false)
    { 
        if(!itemContainer.itemsGone){
            if(state)
            {
                if(inRange)
                {
                    outline.SetActive(true);
                    outOfRangeOutline.SetActive(false);
                }
                else
                {
                    outline.SetActive(false);
                    outOfRangeOutline.SetActive(true);
                }
            }
            else
            {
                outline.SetActive(false);
                outOfRangeOutline.SetActive(false);
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