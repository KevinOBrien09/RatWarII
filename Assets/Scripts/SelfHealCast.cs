using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using DG.Tweening;


public class SelfHealCast : SkillCastBehaviour
{
    public int value;
    public override void Go(CastArgs args)
    {
        BattleZoomer.inst.SoloZoom(args,(()=>{
       
            args.caster.Heal(value);
        }));


        
        //  CamFollow.inst.Focus(args.caster.slot.transform,()=>
        //         { CamFollow.inst.ChangeCameraState(CameraState.LOCK); });
        //  SkillAimer.inst.Skip();
    }
}