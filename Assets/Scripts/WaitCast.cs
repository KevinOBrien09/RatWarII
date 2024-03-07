using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using DG.Tweening;
using System.Collections;

public class WaitCast : SkillCastBehaviour
{
    public override void Go(CastArgs args)
    {
        CamFollow.inst.Focus(args.caster.slot.transform,()=>
        { CamFollow.inst.ChangeCameraState(CameraState.LOCK); });
        if(args.caster.sounds != null)
        {AudioManager.inst.GetSoundEffect().Play(args.caster.sounds.move);}
               
        StartCoroutine(q());
        IEnumerator q()
        {
            
            yield return new WaitForSeconds(.5f);
            SkillAimer.inst.Finish(true);

       }
    }
}