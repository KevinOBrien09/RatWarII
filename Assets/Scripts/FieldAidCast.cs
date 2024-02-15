using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;
 

public class FieldAidCast : SkillCastBehaviour
{
    public override void Go(CastArgs args)
    {
        StartCoroutine(q());
        IEnumerator q(){
            CamFollow.inst.ChangeCameraState(CameraState.LOCK);
            CamFollow.inst.target = args.target.transform;

            if(args.target != args.caster)
            {
                args.caster.Flip(args.targetSlot.transform.position);
            }

            if(args.target.statusEffects[StatusEffectEnum.BLEED].Count > 0)
            {
                args.target.RemoveStatusEffect(StatusEffectEnum.BLEED);
                ObjectPoolManager.inst.Get<BattleNumber>(ObjectPoolTag.BATTLENUMBER).Go("<i>Removed Bleed!",Color.white,args.target.transform.position);
            }
 PlaySound(0,args.skill);
            yield return new WaitForSeconds(.75f);
        
            SkillAimer.inst.Finish();
        }
       


    }
}