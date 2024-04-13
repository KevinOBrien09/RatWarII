using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using DG.Tweening;

public class SlotInfoDisplay : Singleton<SlotInfoDisplay>
{
    public TextMeshProUGUI charName,hp,speciesClass,level;
    public TextMeshProUGUI speed,moveRange,strength,magic;
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
        if(slot.cont.unit != null)
        { 
            if(slot.cont.unit.isEntity())
            {
                healthBar.health = slot.cont.unit.health;
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
                
                if( ! GameManager.inst.checkGameState(GameState.PLAYERUI) && ! GameManager.inst.checkGameState(GameState.INTERACT)) 
                {
                    if(ActionMenu.inst.currentState == ActionMenuState.ROAM){
                    CamFollow.inst.Focus(slot.transform,()=>{CamFollow.inst.ChangeCameraState(CameraState.FREE);});
                    }
                    else{
                        CamFollow.inst.Focus(slot.transform,()=>{});
                    }
                
                }
                
                Unit u = slot.cont.unit;
                Character c = slot.cont.unit.character;
                charName.text = c.characterName.fullName();
            
                level.text = c.exp.level.ToString();
                if(slot.cont.unit.side == Side.PLAYER && !sl.cont.unit.isHostage){
                    speciesClass.text = c.job.ToString() + " "+ c.species.ToString();
                }
                else{
                    speciesClass.text = slot.cont.unit.enemy.tagLine;
                }
                stackHandler.Kill();
                stackHandler.Spawn(u);
                List<SlotContents> sc = new List<SlotContents>(u.slot.cont.slotContents);

                if(slot.cont.specialSlot != null){
                    sc.Add(slot.cont.specialSlot.slotContents);
                }
            
                stackHandler.SlotContents(sc);
                speed.text = "SPEED:" + u.stats().speed.ToString();
                strength.text = "STR:" + u.stats().strength.ToString();
                moveRange.text = "MOVE:" + u.stats().moveRange.ToString();
                magic.text = "MGK:" + u.stats().magic.ToString();
                icon.gameObject.SetActive(true);
                if(slot.cont.unit.side == Side.PLAYER)
                {
                if(slot.cont.unit.isHostage) 
                {
                icon.texture = slot.cont.unit.enemy.icon;
                }
                else{
    
                    icon.texture = IconGraphicHolder.inst.dict[u.character.ID];
                }
                
                }
                else{
                    icon.texture = slot.cont.unit.enemy.icon;
                }
                
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
            
                if( !GameManager.inst.checkGameState(GameState.PLAYERUI)&& ! GameManager.inst.checkGameState(GameState.INTERACT)) 
                {
                    if(ActionMenu.inst.currentState == ActionMenuState.ROAM)
                    {CamFollow.inst.Focus(slot.transform,()=>{CamFollow.inst.ChangeCameraState(CameraState.FREE);});}
                    else
                    {CamFollow.inst.Focus(slot.transform,()=>{});}
                }
                if(slot.cont.specialSlot == null){
                    charName.text = "Empty Slot";
                    icon.texture = null;    
                    icon.gameObject.SetActive(false);
                }
                else{
                    charName.text = slot.cont.specialSlot.slotContents.contentName;
                    icon.texture =  slot.cont.specialSlot.slotContents.picture;   
                    icon.gameObject.SetActive(true );
                }
                
            
                healthBar.gameObject.SetActive(false);
                stackHandler.Kill();
            
                stackHandler.SlotContents(slot.cont.slotContents);
                
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
        
        stackHandler.Kill();
        sl = null;
        healthBar.health = null;
        rt.DOAnchorPos(hidden,.2f).OnComplete(()=>
        {
           // gameObject.SetActive(false);
        });

    }

  
}