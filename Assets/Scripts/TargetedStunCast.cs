using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetedStunCast :  SkillCastBehaviour
{
    public override void Go(CastArgs args)
    {
        StartCoroutine(q());
        IEnumerator q()
        {
               
            args.target.Stun();
            yield return new WaitForSeconds(.33f);
            SkillAimer.inst.Finish();
        }
       
       
    }
}
