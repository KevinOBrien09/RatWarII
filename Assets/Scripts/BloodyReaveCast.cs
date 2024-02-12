using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using DG.Tweening;


public class BloodyReaveCast : SkillCastBehaviour
{  
    public int damage = 35;
    public override void Go(CastArgs args)
    {
        BattleZoomer.inst.ZoomIn(args,(()=>
        {
            if(args.caster.health.maxHealth != args.caster.health.currentHealth){
            int i = args.target.health.dmgAmount(damage);
            args.caster.Heal(i/2);
            }
          
            args.target.Hit(damage);
            PlaySound(0,args.skill);

        }),true);
       
    }
}