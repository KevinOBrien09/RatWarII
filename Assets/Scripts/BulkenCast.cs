using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;
 

public class BulkenCast : StatModifierCast
{
    public ParticleSystem pSystem;
    public Zoomer zoomer;
    public override void Go(CastArgs args)
    {   
        SoloZoomer sz = Instantiate(zoomer) as SoloZoomer;
        sz.AttachToBattle();
        sz.Go(args,(()=>
        {
            CamFollow.inst.Focus(args.caster.slot.transform,()=>
            { 
                CamFollow.inst.ChangeCameraState(CameraState.LOCK); 
            });
            ParticleSystem p =  Instantiate(pSystem, args.caster.transform);
            args.caster.graphic.CustomColourFlash(()=>
            {
                p.Play();
                p.gameObject.GetComponent<ParticleSystemRenderer>().sortingOrder = args.caster.graphic.allRenderers[0].sortingOrder;
                PlaySound(0,args.skill);
                StatusEffects.StatMod(args.target,args.skill,turnDuration,change, statToChange);
                ObjectPoolManager.inst.Get<BattleNumber>(ObjectPoolTag.BATTLENUMBER).Go("DEF <size=90%>^25",Color.yellow,args.caster.transform.position);
            },Color.yellow);
            
        }));
   
    }
}