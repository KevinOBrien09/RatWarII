using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using DG.Tweening;


public class SkillCastBehaviour : MonoBehaviour
{
    public virtual void Go(CastArgs args){
        Debug.LogWarning(args.skill.skillName + " was cast");
    }
}

[System.Serializable]
public class CastArgs
{
    public Unit caster;
    public Unit target;
    public Skill skill;
    public Slot targetSlot;
    public UnityAction castEffects;
}