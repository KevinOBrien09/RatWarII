using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

using DG.Tweening;

public class WorldInteractable : WorldTile
{
    public GameObject hoverObject;
    public Transform camPos;
    public Outline outline;
    bool hasOutline,hasHoverObject;
    public UnityEvent a;
    void Start()
    {
        hasOutline = outline != null;
        hasHoverObject = hoverObject!= null;
        if(hasHoverObject)
        {hoverObject.gameObject.SetActive(false);}
        
        if(hasOutline)
        {outline.enabled = false;}
    }
    public override void Click()
    {
        if(WorldCity.inst.currentState == WorldCity.CityState.HOVER&& !WorldHubCamera.inst.fuckOff)
        { WorldCity.inst. desktopButton.SetActive(false);
            WorldHubCamera.inst.fuckOff = true;
            WorldHubCamera.inst.Move(camPos,()=>
            { a.Invoke(); });
            DisableHover();
        }
    }
    
    public override void Enter()
    {
        if(WorldCity.inst.currentState == WorldCity.CityState.HOVER && !WorldHubCamera.inst.fuckOff)
        {
            base.Enter();
            if(hasHoverObject)
            {hoverObject.gameObject.SetActive(true);}
            if(hasOutline)
            {outline.enabled = true;}
        }
    }

    public override void Exit()
    {
        base.Exit();    
        DisableHover();
    }

    void DisableHover()
    {
        if(hasHoverObject)
        { hoverObject.gameObject.SetActive(false);}
        if(hasOutline)
        {outline.enabled = false;}
    }

}