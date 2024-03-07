using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;

public class NoMoveFormation : ActionMenuFormation
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
            
            case ActionMenuState.INTERACT:
            ChangeState(ActionMenuState.ROAM);
            break;
            
            case ActionMenuState.ROAM:
            ChangeState(ActionMenuState.SKILL);
            break;

        }
        ActionMenu.inst.center.text = ActionMenu.inst.currentState.ToString();
    }

    public override void MoveRight(){
        switch( ActionMenu.inst.currentState)
        {
            case ActionMenuState.SKILL:
            ChangeState(ActionMenuState.ROAM);
            break;
            
            case ActionMenuState.ROAM:
            ChangeState(ActionMenuState.INTERACT);
            break;

            case ActionMenuState.INTERACT:
            ChangeState(ActionMenuState.SKILL);
            break;
        }
        ActionMenu.inst.center.text = ActionMenu.inst.currentState.ToString();
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
            
            break;

     

            case ActionMenuState.ROAM:
            ActionMenu.inst.border.DORotate(new Vector3(0,0,180),.25f);
            icons[ActionMenuState.SKILL].DOLocalRotate(new Vector3(0,0,180),.25f);
            icons[ActionMenuState.ROAM].DOLocalRotate(new Vector3(0,0,0),.25f);
            icons[ActionMenuState.INTERACT].DOLocalRotate(new Vector3(0,0,90),.25f);
           
            break;

            case ActionMenuState.INTERACT:
            ActionMenu.inst.border.DORotate(new Vector3(0,0, -90),.25f);
            icons[ActionMenuState.SKILL].DOLocalRotate(new Vector3(0,0,90),.25f);
            icons[ActionMenuState.ROAM].DOLocalRotate(new Vector3(0,0,-90),.25f);
            icons[ActionMenuState.INTERACT].DOLocalRotate(new Vector3(0,0,0),.25f);
            break;
        }
        ActionMenu.inst.center.text = ActionMenu.inst.currentState.ToString();
    }

    public override void Reset(){
        icons[ActionMenuState.SKILL].DOLocalRotate(new Vector3(0,0,0),.25f);
        icons[ActionMenuState.ROAM].DOLocalRotate(new Vector3(0,0,180),.25f);
        icons[ActionMenuState.INTERACT].DOLocalRotate(new Vector3(0,0,-90),.25f);
        ActionMenu.inst.center.text = ActionMenu.inst.currentState.ToString();
    }


}