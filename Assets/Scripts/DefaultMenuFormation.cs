using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;

public class DefaultMenuFormation : ActionMenuFormation
{
    public SoundData left,right;
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
            ChangeState(ActionMenuState.SKIP);
            break;
            
            case ActionMenuState.MOVE:
            ChangeState(ActionMenuState.SKILL);
            break;
            
            case ActionMenuState.SKIP:
            ChangeState(ActionMenuState.ROAM);
            break;
            
            case ActionMenuState.ROAM:
            ChangeState(ActionMenuState.ITEM);
            break;

            case ActionMenuState.ITEM:
            ChangeState(ActionMenuState.MOVE);
            break;

        }
            AudioManager.inst.GetSoundEffect().Play(left);
         ChangeCenterText();
    }

    public override void MoveRight(){
        switch( ActionMenu.inst.currentState)
        {
            case ActionMenuState.SKILL:
            ChangeState(ActionMenuState.MOVE);
            break;
            
            case ActionMenuState.MOVE:
            ChangeState(ActionMenuState.ITEM);
            break;
            
            case ActionMenuState.ITEM:
            ChangeState(ActionMenuState.ROAM);
            break;
            
            case ActionMenuState.ROAM:
            ChangeState(ActionMenuState.SKIP);
            break;

            case ActionMenuState.SKIP:
            ChangeState(ActionMenuState.SKILL);
            break;
        }
        AudioManager.inst.GetSoundEffect().Play(right);
        ChangeCenterText();
    }

    public override void ChangeState(ActionMenuState newState)
    {
        ActionMenu.inst.ChangeActionMenuState(newState);
        switch(newState)
        {
            case ActionMenuState.SKILL:
            rt.DORotate(Vector3.zero,.25f);
            foreach (var item in icons)
            {
                item.Value.DOLocalRotate(Vector3.zero,.25f);
            }
            break;
            
            case ActionMenuState.MOVE:
            rt.DORotate(new Vector3(0,0,60),.25f);
             foreach (var item in icons)
            {
                item.Value.DOLocalRotate(new Vector3(0,0,-60),.25f);
            }
            break;
            
            case ActionMenuState.ITEM:
            rt.DORotate(new Vector3(0,0,120),.25f);
            foreach (var item in icons)
            {
                item.Value.DOLocalRotate(new Vector3(0,0,-120),.25f);
            }
            break;
            
            case ActionMenuState.ROAM:
            rt.DORotate(new Vector3(0,0,240),.25f);
            foreach (var item in icons)
            {
                item.Value.DOLocalRotate(new Vector3(0,0,-240),.25f);
            }
            break;

            case ActionMenuState.SKIP:
            rt.DORotate(new Vector3(0,0,300),.25f);
            foreach (var item in icons)
            {
                item.Value.DOLocalRotate(new Vector3(0,0,-300),.25f);
            }
            break;
        }
        ChangeCenterText();
    }

    public override void Reset()
    {
        rt.DORotate(Vector3.zero,.25f);
        foreach (var item in icons)
        {item.Value.DOLocalRotate(Vector3.zero,.25f);}
        ChangeCenterText();
    }

    public override void ItemReset()
    {
    rt.DORotate(new Vector3(0,0,120),0);
            foreach (var item in icons)
            {
                item.Value.DOLocalRotate(new Vector3(0,0,-120),0);
            }
        ChangeCenterText();
    }

    public void ChangeCenterText(){
     ActionMenu.inst.center.text = ActionMenu.inst.currentState.ToString();
     if( ActionMenu.inst.currentState == ActionMenuState.ROAM){
           ActionMenu.inst.center.text = "ANALYSIS";
     }
    }



}