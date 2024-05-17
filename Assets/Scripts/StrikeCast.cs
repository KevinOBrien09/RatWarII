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
    public override void Go(CastArgs args)
    {
        BattleZoomer.inst.ZoomIn(args,(()=>
        { 
            float percent = MiscFunctions.GetPercentage(args.caster.stats().strength,100);
            args.target.Hit((int) percent,args);
            PlaySound(0,args.skill);

        }),true);
       
    }

}