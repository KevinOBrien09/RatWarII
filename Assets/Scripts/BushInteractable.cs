
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;

public class BushInteractable : Interactable
{
    public GameObject outOfRangeOutline;
    public override void Go(){
        InteractZoomer.inst.Zoom(transform.parent.gameObject,PartyController.inst.leader);
    }
    public override void OutlineToggle(bool state,bool inRange = false)
    {
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