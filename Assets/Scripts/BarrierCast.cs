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
    public override void Go(CastArgs args)
    {
        BattleZoomer.inst.SoloZoom(args,(()=>{

            GameObject s = Instantiate(shield,args.caster.transform);
            args.caster.health.GainShield(25);
            float ogX = s.transform.localScale.x;
            s.transform.localScale = new Vector3(0,s.transform.localScale.y,s.transform.localScale.z);
            s.transform.DOScale(new Vector3(ogX,s.transform.localScale.y,s.transform.localScale.z),.25f);
            s.name = "shield";
            s.GetComponent<ParticleSystemRenderer>().sortingLayerName = "Zoom";
        }));
    }
}