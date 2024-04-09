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
    public float camFOV = 40;
   IEnumerator Start()
    {
        hasOutline = outline != null;
        hasHoverObject = hoverObject!= null;
        if(hasHoverObject)
        {hoverObject.gameObject.SetActive(false);}
        yield return new WaitForEndOfFrame();
        if(hasOutline)
        {outline.enabled = false;}
    }
    public override void Click()
    {
        if(  HubStateHandler.inst.currentState ==  HubStateHandler.HubState.HOVER&& !WorldHubCamera.inst.fuckOff)
        {  
            WorldHubCamera.inst.fuckOff = true;
            WorldHubCamera.inst.cam.DOFieldOfView(camFOV,.2f);
            WorldHubCamera.inst.Move(camPos,()=>
            { a.Invoke(); });
            DisableHover();
        }
    }
    
    public override void Enter()
    {
        if(  HubStateHandler.inst.currentState ==   HubStateHandler.HubState.HOVER && !WorldHubCamera.inst.fuckOff)
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

  public  void DisableHover()
    {
        if(hasHoverObject)
        { hoverObject.gameObject.SetActive(false);}
        if(hasOutline)
        {outline.enabled = false;}
    }

}