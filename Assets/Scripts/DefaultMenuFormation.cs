using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;

public class DefaultMenuFormation : ActionMenuFormation
{
    public override void Activate(){
        gameObject.SetActive(true);
    }

    public override void Deactivate(){
        gameObject.SetActive(false);
    }

    public override void MoveLeft()
    {
        
        switch(ActionMenu.inst.currentState)
        {
            case ActionMenuState.SKILL:
            ChangeState(ActionMenuState.INTERACT);
            break;
            
            case ActionMenuState.MOVE:
            ChangeState(ActionMenuState.SKILL);
            break;
            
            case ActionMenuState.INTERACT:
            ChangeState(ActionMenuState.ROAM);
            break;
            
            case ActionMenuState.ROAM:
            ChangeState(ActionMenuState.MOVE);
            break;

        }
         ChangeCenterText();
    }

    public override void MoveRight(){
        switch( ActionMenu.inst.currentState)
        {
            case ActionMenuState.SKILL:
            ChangeState(ActionMenuState.MOVE);
            break;

            case ActionMenuState.MOVE:
            ChangeState(ActionMenuState.ROAM);
            break;

            case ActionMenuState.ROAM:
            ChangeState(ActionMenuState.INTERACT);
            break;

            case ActionMenuState.INTERACT:
            ChangeState(ActionMenuState.SKILL);
            break;
        }
         ChangeCenterText();
    }

    public override void ChangeState(ActionMenuState newState)
    {
        ActionMenu.inst.ChangeActionMenuState(newState);
        switch(newState)
        {
            case ActionMenuState.SKILL:
            ActionMenu.inst.border.DORotate(Vector3.zero,.25f);
            icons[ActionMenuState.SKILL].DOLocalRotate(new Vector3(0,0,0),.25f);
            icons[ActionMenuState.ROAM].DOLocalRotate(new Vector3(0,0,180),.25f);
            icons[ActionMenuState.INTERACT].DOLocalRotate(new Vector3(0,0,-90),.25f);
            icons[ActionMenuState.MOVE].DOLocalRotate(new Vector3(0,0,90),.25f);
            break;

            case ActionMenuState.MOVE:
            ActionMenu.inst.border.DORotate(new Vector3(0,0,90),.25f);
            icons[ActionMenuState.SKILL].DOLocalRotate(new Vector3(0,0,-90),.25f);
            icons[ActionMenuState.ROAM].DOLocalRotate(new Vector3(0,0,90),.25f);
            icons[ActionMenuState.INTERACT].DOLocalRotate(new Vector3(0,0,180),.25f);
            icons[ActionMenuState.MOVE].DOLocalRotate(new Vector3(0,0,0),.25f);
            break;

            case ActionMenuState.ROAM:
            ActionMenu.inst.border.DORotate(new Vector3(0,0,180),.25f);
            icons[ActionMenuState.SKILL].DOLocalRotate(new Vector3(0,0,180),.25f);
            icons[ActionMenuState.ROAM].DOLocalRotate(new Vector3(0,0,0),.25f);
            icons[ActionMenuState.INTERACT].DOLocalRotate(new Vector3(0,0,90),.25f);
            icons[ActionMenuState.MOVE].DOLocalRotate(new Vector3(0,0,-90),.25f);
            break;

            case ActionMenuState.INTERACT:
            ActionMenu.inst.border.DORotate(new Vector3(0,0, -90),.25f);
            icons[ActionMenuState.SKILL].DOLocalRotate(new Vector3(0,0,90),.25f);
            icons[ActionMenuState.ROAM].DOLocalRotate(new Vector3(0,0,-90),.25f);
            icons[ActionMenuState.INTERACT].DOLocalRotate(new Vector3(0,0,0),.25f);
            icons[ActionMenuState.MOVE].DOLocalRotate(new Vector3(0,0,180),.25f);
            break;
        }
         ChangeCenterText();
    }

    public override void Reset(){
        icons[ActionMenuState.SKILL].DOLocalRotate(new Vector3(0,0,0),.25f);
        icons[ActionMenuState.ROAM].DOLocalRotate(new Vector3(0,0,180),.25f);
        icons[ActionMenuState.INTERACT].DOLocalRotate(new Vector3(0,0,-90),.25f);
        icons[ActionMenuState.MOVE].DOLocalRotate(new Vector3(0,0,90),.25f);
        ChangeCenterText();
    }

    public void ChangeCenterText(){
     ActionMenu.inst.center.text = ActionMenu.inst.currentState.ToString();
     if( ActionMenu.inst.currentState == ActionMenuState.ROAM){
           ActionMenu.inst.center.text = "ANALYSIS";
     }
    }



}