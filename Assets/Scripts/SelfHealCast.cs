using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using DG.Tweening;


public class SelfHealCast : SkillCastBehaviour
{
  
    public override void Go(CastArgs args)
    {
        BattleZoomer.inst.SoloZoom(args,(()=>{
       
            args.caster.Heal(args.skill.value[0]);
        }));


        
        //  CamFollow.inst.Focus(args.caster.slot.transform,()=>
        //         { CamFollow.inst.ChangeCameraState(CameraState.LOCK); });
        //  SkillAimer.inst.Skip();
    }
}