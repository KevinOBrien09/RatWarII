using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using DG.Tweening;
using System.Collections;

public class LickWoundsCast : SkillCastBehaviour
{
    public int value;
    public override void Go(CastArgs args)
    {
        BattleZoomer.inst.SoloZoom(args,(()=>
        {
           BattleManager.inst.StartCoroutine(q());
            IEnumerator q()
            {
            
              
                PlaySound(0,args.skill);
                if(args.caster.health.notFull()){
                float percent = MiscFunctions.GetPercentage(args.caster.health.maxHealth,10);
          
                args.caster.Heal((int)percent);
                }
                yield return new WaitForSeconds(1f);
                if(args.caster.statusEffects[StatusEffectEnum.BLEED].Count > 0)
                {
                    args.caster.RemoveStatusEffect(StatusEffectEnum.BLEED);
                    ObjectPoolManager.inst.Get<BattleNumber>(ObjectPoolTag.BATTLENUMBER).Go("<i>Removed Bleed!",Color.white,args.caster.transform.position);
                }
            }
            
            
        }));


        
        //  CamFollow.inst.Focus(args.caster.slot.transform,()=>
        //         { CamFollow.inst.ChangeCameraState(CameraState.LOCK); });
        //  SkillAimer.inst.Skip();
    }
}