using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;
 

public class StatModifierCast : SkillCastBehaviour
{
    public int change;
    public StatEnum statToChange;
    public int turnDuration;
    public override void Go(CastArgs args)
    {
        CamFollow.inst.Focus(args.caster.slot.transform,()=>
        { CamFollow.inst.ChangeCameraState(CameraState.LOCK); });
        
        StatusEffects.StatMod(args.target,args.skill,turnDuration,change, statToChange);

        StartCoroutine(q());
        IEnumerator q()
        {
            
            yield return new WaitForSeconds(.5f);
            SkillAimer.inst.Finish(false);

       }
    }
}