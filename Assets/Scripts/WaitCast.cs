using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using DG.Tweening;


public class WaitCast : SkillCastBehaviour
{
    public override void Go(CastArgs args){
         CamFollow.inst.Focus(args.caster.slot.transform,()=>
                { CamFollow.inst.ChangeCameraState(CameraState.LOCK); });
         SkillAimer.inst.Skip();
    }
}