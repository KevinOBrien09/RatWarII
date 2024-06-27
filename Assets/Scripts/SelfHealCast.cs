using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using DG.Tweening;


public class SelfHealCast : SkillCastBehaviour
{
    public Zoomer zoomer;
    public override void Go(CastArgs args)
    {
        SoloZoomer sz = Instantiate(zoomer) as SoloZoomer;
        sz.AttachToBattle();
        sz.Go(args,(()=>{
       
            args.caster.Heal(args.skill.value[0]);
        }));
    }
}