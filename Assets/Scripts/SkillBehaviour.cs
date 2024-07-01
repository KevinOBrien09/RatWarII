using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;
 

public class SkillBehaviour : Selectable
{
    public TextMeshProUGUI txt;
    public Image bg;
    public Castable skill;
    public Color32 color1,color2,colour3,colour4;
    public SoundData hoverSFX;
    public int itemCount;
    protected override  void Start(){
       
    }

    public void Init(Castable s){
        txt.text = s.GetName();
        skill = s;
        Skill ability = skill as Skill;
        Item item = skill as Item;
        if(ability != null){
            txt.color = color2;
            bg.color = color1;
        }
        else{
            txt.color = colour4;
            bg.color = colour3;
        }
    }
    public void ItemStack(Item i){
        itemCount++;
        txt.text = i.GetName() + " x" + itemCount;
    }

    public void RemoveItem(Item i){
        itemCount--;
        txt.text = i.GetName() + " x" + itemCount;
    }

    public override void OnSelect(BaseEventData eventData)
    {
      
        Skill ability = skill as Skill;
        Item item = skill as Item;
        base.OnSelect(eventData);
        BattleTicker.inst.Type(skill.GetName() + ":" +SkillParser.Parse( skill.GetDesc(),u: BattleManager.inst.currentUnit) );
       

        if(ability != null){
            txt.color = color1;
            bg.color = color2;
            SkillHandler.inst.hoveredSkill = skill;
            SkillHandler.inst.hoveredBehaviour = this;
            SkillHandler.inst.ChangeCostDetails(ability);
        }
        else if(item != null)
        {
            txt.color = colour3;
            bg.color = colour4;
            SkillHandler.inst.hoveredSkill = skill;
            SkillHandler.inst.hoveredBehaviour = this;
        }
       
        AudioManager.inst.GetSoundEffect().Play(hoverSFX);
    }

    public override void OnDeselect(BaseEventData eventData)
    {
        base.OnSelect(eventData);
        Skill ability = skill as Skill;
        Item item = skill as Item;
        if(ability != null){
            txt.color = color2;
            bg.color = color1;
        }
        else{
            txt.color = colour4;
            bg.color = colour3;
        }
    }

   protected override void OnDisable(){
    base.OnDisable();
         Skill ability = skill as Skill;
        Item item = skill as Item;
        if(ability != null){
            txt.color = color2;
            bg.color = color1;
        }
        else{
            txt.color = colour4;
            bg.color = colour3;
        }
    }

}