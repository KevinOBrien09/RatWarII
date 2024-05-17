using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using DG.Tweening;

public class SlotInfoDisplay : Singleton<SlotInfoDisplay>
{
    public TextMeshProUGUI charName,hp,resource,speciesClass,level,moveTokenCount;
    public TextMeshProUGUI speed,moveRange,strength,magic,defence;
    public GenericDictionary<SkillResource.Catagory,Sprite> resBarDict = new GenericDictionary<SkillResource.Catagory, Sprite>();
    public RawImage icon;
    public Image resourceFill;
    public GameObject resourceBar;
    public Vector2 shown,hidden;
    public RectTransform rt;
    public StatusEffectStackHandler stackHandler;
    public HealthBar healthBar;
    public List<GameObject>moveTokes = new List<GameObject>();
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
                resourceBar.gameObject.SetActive(true);
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
                resourceFill.sprite = resBarDict[u.skillResource.catagory];
                resourceFill.DOFillAmount((float)u.skillResource.current/(float)u.skillResource.max,0);
                resource.text = u.skillResource.abbrv() + u.skillResource.current.ToString() +"/" + u.skillResource.max.ToString();
                stackHandler.SlotContents(sc);
                SetStats(u);
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

                moveTokenCount.text = " MT:"+ u.currentMoveTokens.ToString();
                foreach (var item in moveTokes)
                {item.SetActive(false);}
                for (int i = 0; i < u.currentMoveTokens; i++)
                { moveTokes[i].SetActive(true); }
                
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
                resourceBar.gameObject.SetActive(false);
                stackHandler.Kill();
            
                stackHandler.SlotContents(slot.cont.slotContents);
                
                hp.text = string.Empty;
                level.text = "0";
                speciesClass.text = string.Empty;
                speed.text =string.Empty;
                strength.text =string.Empty;
                moveRange.text = string.Empty;
                magic.text = string.Empty;
                defence.text = string.Empty;
                    
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

    public void SetStats(Unit u){
        string normal =  "<color=white>";
        string increase = "<color=yellow>";
        string decrease = "<color=lightblue>";
        string end = "</color>";


        string speedModColour = normal;
        if(u.stats().speed > u.character.baseStats.speed)
        {speedModColour = increase;}
        else if(u.stats().speed < u.character.baseStats.speed)
        {speedModColour = decrease;}
        speed.text = "SPD:" + speedModColour + u.stats().speed.ToString() + end;

        string strModColour = normal;
        if(u.stats().strength > u.character.baseStats.strength)
        {strModColour = increase;}
        else if(u.stats().strength <  u.character.baseStats.strength)
        {strModColour = decrease;}
        strength.text = "STR:" + strModColour + u.stats().strength.ToString() + end;

        string moveModColour = normal;
        if(u.stats().moveRange > u.character.baseStats.moveRange)
        {moveModColour = increase;}
        else if(u.stats().moveRange < u.character.baseStats.moveRange)
        {moveModColour = decrease;}
        moveRange.text = "MVE:" + moveModColour + u.stats().moveRange.ToString() + end;

        string mgkModColour = normal;
        if(u.stats().magic > u.character.baseStats.magic)
        {mgkModColour = increase;}
        else if(u.stats().magic < u.character.baseStats.magic)
        {mgkModColour = decrease;}
        magic.text = "MGK:" + mgkModColour+ u.stats().magic.ToString() + end;

        string defModColour = normal;
        if(u.stats().defence > u.character.baseStats.defence)
        {defModColour = increase;}
        else if(u.stats().defence < u.character.baseStats.defence)
        {defModColour = decrease;}
        defence.text ="DEF:"+ defModColour + u.stats().defence.ToString() +"%</color>";
    }

  
}