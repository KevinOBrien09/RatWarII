using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using DG.Tweening;

public class SlotInfoDisplay : Singleton<SlotInfoDisplay>
{
    public TextMeshProUGUI charName,hp,speciesClass,level;
    public TextMeshProUGUI speed,moveRange,strength;
    public RawImage icon;
    public Vector2 shown,hidden;
    public RectTransform rt;
    public StatusEffectStackHandler stackHandler;
    public HealthBar healthBar;
   public Slot sl;
    void Start()
    {
        shown = rt.anchoredPosition;
        Disable();
     
    }

    public void Apply(Slot slot)
    {
        
        sl = slot;
        if(slot.unit != null)
        { 
       
            healthBar.health = slot.unit.health;
            healthBar.gameObject.SetActive(true);
            healthBar.Refresh();
            if(!gameObject.activeSelf){
                gameObject.SetActive(true);
                rt.DOAnchorPos(hidden,0);
                rt.DOAnchorPos(shown,.2f);
            }
            else{      gameObject.SetActive(true);
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
            //hp.text = "HP:"+ u.health.currentHealth.ToString()+ "/" + u.stats().hp.ToString();
            level.text = c.exp.level.ToString();
            if(slot.unit.side == Side.PLAYER){
                speciesClass.text = c.job.ToString() + " "+ c.species.ToString();
            }
            else{
                  speciesClass.text = slot.unit.enemy.tagLine;
            }
            stackHandler.Kill();
            stackHandler.Spawn(u);
            List<SlotContents> sc = new List<SlotContents>(u.slot.slotContents);

            if(slot.specialSlot != null){
                sc.Add(slot.specialSlot.slotContents);
            }
            stackHandler.SlotContents(sc);
            speed.text = "SPEED:" + u.stats().speed.ToString();
            strength.text = "STR:" + u.stats().strength.ToString();
            moveRange.text = "MOVE:" + u.stats().moveRange.ToString();
            icon.gameObject.SetActive(true);
            if(slot.unit.side == Side.PLAYER){
                sl.unit.graphic.cam.gameObject.SetActive(true);
                icon.texture = u.graphic.cam.activeTexture;
            }
            else{
                  icon.texture = slot.unit.enemy.icon;
            }
            
        }
        else
        {   
            if(!gameObject.activeSelf){
                gameObject.SetActive(true);
                rt.DOAnchorPos(hidden,0);
                rt.DOAnchorPos(shown,.2f);
            }
            else{      gameObject.SetActive(true);
                   rt.DOAnchorPos(shown,.2f);
            }
           
            if( !GameManager.inst.checkGameState(GameState.PLAYERUI)) 
            {
                if(ActionMenu.inst.currentState == ActionMenu.ActionMenuState.ROAM)
                {CamFollow.inst.Focus(slot.transform,()=>{CamFollow.inst.ChangeCameraState(CameraState.FREE);});}
                else
                {CamFollow.inst.Focus(slot.transform,()=>{});}
            }
            if(slot.specialSlot == null){
                charName.text = "Empty Slot";
                icon.texture = null;    
                icon.gameObject.SetActive(false);
            }
            else{
                charName.text = slot.specialSlot.slotContents.contentName;
                icon.texture =  slot.specialSlot.slotContents.picture;   
                icon.gameObject.SetActive(true );
            }
            
           
            healthBar.gameObject.SetActive(false);
            stackHandler.Kill();
            stackHandler.SlotContents(slot.slotContents);
            
            hp.text = string.Empty;
            level.text = "0";
            speciesClass.text = string.Empty;
            speed.text =string.Empty;
            strength.text =string.Empty;
            moveRange.text = string.Empty;
           
        }
    }

    public void Disable()
    {
        if(sl != null)
        {
            if(sl.unit != null)
            {   
                if(sl.unit.side == Side.PLAYER){
  sl.unit .graphic.cam.gameObject.SetActive(false);
                }
              
            }
        }
        stackHandler.Kill();
        sl = null;
        healthBar.health = null;
        rt.DOAnchorPos(hidden,.2f).OnComplete(()=>
        {
           // gameObject.SetActive(false);
        });

    }

  
}