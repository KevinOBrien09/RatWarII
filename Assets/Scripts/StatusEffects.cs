using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;
 
public enum StatusEffectEnum{BARRIER,BLEED,BILE}
public static class StatusEffects
{

    public static void Barrier(Unit u,Skill skill, int howManyTurns,int shieldAmount)
    {
        StatusEffect barrier = new StatusEffect();
        int kill = BattleManager.inst.turn + howManyTurns;
        barrier.Init
        (
            _add: ()=>{u.health.GainShield(barrier,shieldAmount,u);},
            _tick:null,
            _remove:()=>{u.health.RemoveShield(barrier);},
           u,kill,skill,StatusEffectEnum.BARRIER
        );
       u.AddStatusEffect(barrier);
    }


    public static void Bleed(Unit u,Skill skill, int howManyTurns)
    {
        StatusEffect bleed = new StatusEffect();
        int kill = BattleManager.inst.turn + howManyTurns;
        bleed.Init
        (
            _add: ()=>{},
            _tick:()=>
            { },
            _remove:()=>{},
            u,kill,skill,StatusEffectEnum.BLEED
        );
        u.AddStatusEffect(bleed);
       // ObjectPoolManager.inst.Get<BattleNumber>(ObjectPoolTag.BATTLENUMBER).Go("BLEED!",Color.red,u.transform.position);
    }


}
