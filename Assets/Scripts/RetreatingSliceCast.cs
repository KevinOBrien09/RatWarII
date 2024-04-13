using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class RetreatingSliceCast : SkillCastBehaviour
{
    public int damage = 10;
    public override void Go(CastArgs args)
    {
        StartCoroutine(Q());
        IEnumerator Q()
        {
            args.caster.Flip(args.targetSlot.transform.position);
            CamFollow.inst.ChangeCameraState(CameraState.LOCK);
            CamFollow.inst.target = args.caster.transform;
            if(args.targetSlot.cont.unit != null){
                float percent = MiscFunctions.GetPercentage(args.caster.stats().strength,200);
                args.targetSlot.cont.unit.Hit((int)percent,args);
            }
          
            PlaySound(0,args.skill);
         
            Knockback.Hit(1,args.target,args.caster,true);
            yield return new WaitForSeconds(.75f);
            args.caster.inKnockback = false;
            SkillAimer.inst.Finish(); 
        }
     
       
    }
}