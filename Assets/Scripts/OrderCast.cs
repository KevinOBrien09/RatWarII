using System.Collections;
using UnityEngine;
using DG.Tweening;

public class OrderCast : SkillCastBehaviour
{
    public override void Go(CastArgs args)
    {
        StartCoroutine(q());
        IEnumerator q()
        {
            CamFollow.inst.ChangeCameraState(CameraState.LOCK);
            CamFollow.inst.target = args.caster.transform;

            Slot casterSlot = args.caster.slot;
            Slot targetSlot = args.target.slot;
            args.caster.Reposition(targetSlot);
            args.target.Reposition(casterSlot);
         
            args.caster.transform.DOMove(targetSlot.transform.position,.2f).OnComplete(()=>{
                
              
            });
            args.target.transform.DOMove(casterSlot.transform.position,.2f).OnComplete(()=>{

              
            });
   
           
            BattleManager.inst.ToggleHealthBars(false);
            yield return new WaitForSeconds(1);
            SkillAimer.inst.Finish();
        }
      
    }
}