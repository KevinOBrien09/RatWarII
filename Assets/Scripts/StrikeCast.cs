using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;
 

public class StrikeCast : SkillCastBehaviour
{
    public int damage = 25;
        public Zoomer zoomer;
    public override void Go(CastArgs args)
    {
        StrikeZoomer sz = Instantiate(zoomer) as StrikeZoomer;
        sz.AttachToBattle();
        sz.Go(args,(()=>
        { 
            CameraShake.inst.Shake(.2f,1f);
            float percent = MiscFunctions.GetPercentage(args.caster.stats().strength,100);
            args.target.Hit((int) percent,args);
            PlaySound(0,args.skill);

        }));
       
    }

}