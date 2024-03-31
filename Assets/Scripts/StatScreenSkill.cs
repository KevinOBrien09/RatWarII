using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;
using UnityEngine.UI;
using System.Linq;
using UnityEngine.EventSystems;
public class StatScreenSkill : MonoBehaviour
{
    public TextMeshProUGUI skillName;
    Skill skill;
    
    public void Init(Skill s)
    {
        skillName.text = s.skillName;
        skill = s;
    }

    public void Click()
    {
        EventSystem.current.SetSelectedGameObject(null);
        SkillWindow.inst.Open(skill,CharacterStatSheet.inst.currentCharacter);
    }
}