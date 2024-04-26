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
    public Skill skill;
    public Color32 color1,color2;

  protected override  void Start(){
            txt.color = color2;
        bg.color = color1;
    }

    public void Init(int i,Skill s){
        txt.text = s.skillName;
        skill = s;
    }

    public override void OnSelect(BaseEventData eventData)
    {
        base.OnSelect(eventData);
        BattleTicker.inst.Type(skill.skillName + ":" +SkillParser.Parse( skill.desc,u: BattleManager.inst.currentUnit) );
        txt.color = color1;
        bg.color = color2;
        SkillHandler.inst.hoveredSkill = skill;
        SkillHandler.inst.hoveredBehaviour = this;
        SkillHandler.inst.ChangeCostDetails(skill);
    }

      public override void OnDeselect(BaseEventData eventData)
    {
        base.OnSelect(eventData);
        txt.color = color2;
        bg.color = color1;
    }

}