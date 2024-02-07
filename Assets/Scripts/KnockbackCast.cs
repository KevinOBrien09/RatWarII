using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using DG.Tweening;


public class KnockbackCast : SkillCastBehaviour
{
  
    public override void Go(CastArgs args)
    {
        StartCoroutine(q());
        IEnumerator q()
        {
     
            // BattleZoomer.inst.ZoomIn(args,(()=>
            // {
            //     args.target.Hit(1);

            // }),false);
      
           
        
            
            // yield return new WaitForSeconds(2f);
            bool stun = !Knockback.Hit(1,args.caster,args.target);
            
            if(stun) {
                Debug.Log("Stun");
            }  
            else{
                Debug.Log("Sucse");
            }
            yield return new WaitForSeconds(.45f);
            CamFollow.inst.Focus(args.caster.slot.transform,()=>
                { CamFollow.inst.ChangeCameraState(CameraState.LOCK); });
              args.target.inKnockback = false;
           SkillAimer.inst.Finish(); 
           // SkillAimer.inst.Skip();
        }
    }




}