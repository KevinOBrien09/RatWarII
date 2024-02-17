using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using DG.Tweening;
using System.Collections;

public class BloodyReaveCast : SkillCastBehaviour
{  
    public int damage = 35;
    public int bleedDuration;
    public override void Go(CastArgs args)
    {
        BattleZoomer.inst.ZoomIn(args,(()=>
        { 
            bool bleed = Random.Range(0,2) == 1;
            if(bleed){
                StatusEffects.Bleed(args.target,args.skill,bleedDuration);
            }
            
            bool dead = args.target.health.willUnitDie(damage);
            args.target.Hit(damage,args);
            PlaySound(0,args.skill);
            if(bleed){
 BattleManager.inst.StartCoroutine(q());
            }
           
            IEnumerator q()
            {
                Unit i = args.target;
                yield return new WaitForSeconds(1f);
                if(i != null)
                { ObjectPoolManager.inst.Get<BattleNumber>(ObjectPoolTag.BATTLENUMBER).Go("<b>BLEED!",Color.red,i.transform.position);}
             
            }


        }),true);
       
    }
}