using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;
 

public class BarrierCast : SkillCastBehaviour
{
    public GameObject shield;
    public int howManyTurns;
    public int shieldAmount = 25;
    public override void Go(CastArgs args)
    {
        BattleZoomer.inst.SoloZoom(args,(()=>{
            Transform t = args.caster.transform.Find("shield");
            if(t == null){
                GameObject s = Instantiate(shield,args.caster.transform);
                float ogX = s.transform.localScale.x;
                s.transform.localScale = new Vector3(0,s.transform.localScale.y,s.transform.localScale.z);
                s.transform.DOScale(new Vector3(ogX,s.transform.localScale.y,s.transform.localScale.z),.25f);
                s.name = "shield";
                args.caster.shieldGraphic = s.GetComponent<ParticleSystemRenderer>();
                args.caster.shieldGraphic.sortingLayerName = "Zoom";
            }
            StatusEffects.Barrier(args.caster,args.skill,howManyTurns,shieldAmount);
             //StatusEffects.Bleed(args.caster,args.skill,howManyTurns);
           
           
        }));
    }
}