using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using DG.Tweening;

public class SlotInfoDisplay : Singleton<SlotInfoDisplay>
{
    public TextMeshProUGUI charName,hp,speciesClass,level;
    public TextMeshProUGUI speed,moveRange;
    public RawImage icon;
   public Vector2 shown,hidden;
 public   RectTransform rt;
    void Start()
    {
        Disable();
        shown = rt.anchoredPosition;
    }

    public void Apply(Slot slot)
    {
        
       
        if(slot.unit != null)
        { 
            if(!gameObject.activeSelf){
            gameObject.SetActive(true);
            rt.DOAnchorPos(hidden,0);
            rt.DOAnchorPos(shown,.2f);
            }
           

            if( ! GameManager.inst.checkGameState(GameState.PLAYERUI)) 
            {
                if(ActionMenu.inst.currentState == ActionMenu.ActionMenuState.ROAM){
                CamFollow.inst.Focus(slot.transform,()=>{CamFollow.inst.ChangeCameraState(CameraState.FREE);});
                }
                else{
                     CamFollow.inst.Focus(slot.transform,()=>{});
                }
               
            }
            
            Unit u = slot.unit;
            Character c = slot.unit.character;
            charName.text = c.characterName.fullName();
            hp.text = "HP:"+ u.currentHP.ToString()+ "/" + u.stats().hp.ToString();
            level.text = c.exp.level.ToString();
            speciesClass.text = c.job.ToString() + " "+ c.species.ToString();
            speed.text = "SPEED:" + u.stats().speed.ToString();
            moveRange.text = "MOVE:" + u.stats().moveRange.ToString();
            icon.gameObject.SetActive(true);
            icon.texture = u.graphic.cam.activeTexture;
        }
        else
        {   
     if(!gameObject.activeSelf){
            gameObject.SetActive(true);
            rt.DOAnchorPos(hidden,0);
            rt.DOAnchorPos(shown,.2f);
            }
           
            if( !GameManager.inst.checkGameState(GameState.PLAYERUI)) 
            {
                if(ActionMenu.inst.currentState == ActionMenu.ActionMenuState.ROAM)
                {CamFollow.inst.Focus(slot.transform,()=>{CamFollow.inst.ChangeCameraState(CameraState.FREE);});}
                else
                {CamFollow.inst.Focus(slot.transform,()=>{});}
            }
            
            charName.text = "Empty Slot";
            hp.text = string.Empty;
            level.text = "0";
            speciesClass.text = string.Empty;
            speed.text =string.Empty;
            moveRange.text = string.Empty;
            icon.texture = null;    
            icon.gameObject.SetActive(false);
        }
    }

    public void Disable(){
          rt.DOAnchorPos(hidden,.2f).OnComplete(()=>{

  gameObject.SetActive(false);
          });

    }

  
}