using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;
 
public enum StatusEffectEnum{BARRIER,BLEED,BILE,STATMOD}
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

    public static void StatMod(Unit u,Skill skill, int howManyTurns,int change, StatEnum statEnum){
        StatusEffect statMod = new StatusEffect();
        int kill = BattleManager.inst.turn + howManyTurns;
        statMod.statEnum = statEnum;
        statMod.Init
        (
            _add: ()=>
            { 
                u.statMods.Edit(statEnum,change);
            },
            _tick:null,
            _remove:()=>
            { 
                int i = 0;
                if(change > 0)
                {i = -System.Math.Abs(change);}
                else
                {i = System.Math.Abs(change);}
                u.statMods.Edit(statEnum,i);
            },
           u,kill,skill,StatusEffectEnum.STATMOD
        );
        u.AddStatusEffect(statMod);
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


    public static string StatusEffectInfo(StatusEffectEnum statusEffectEnum, Unit u)
    {
        string txt = "";
   string v = "";
        switch(statusEffectEnum){
            case StatusEffectEnum.BARRIER:
         
            if(u.health.shields.Count== 1){
                v = " Barrier ";
            }
            else{
                v = " Barriers ";
            }

            txt = u.health.shields.Count + v + "shielding<br> " + u.health.shield().ToString() + " HP.";
            break;
            case StatusEffectEnum.BLEED:
          
            if( u.statusEffects[StatusEffectEnum.BLEED].Count == 1){
                v = " Bleed ";
            }
            else{
                v = " Bleeds ";
            }
            txt = u.statusEffects[StatusEffectEnum.BLEED].Count + v + "dealing " + u.BleedDamage(u.statusEffects[StatusEffectEnum.BLEED].Count) + " DMG SoT.";
            break;

        }

        return txt;
    }


}
