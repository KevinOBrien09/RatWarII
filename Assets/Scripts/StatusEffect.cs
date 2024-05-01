using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class StatusEffect
{
    public Unit unit;
    public int turnToKill;
    public UnityAction add, tick,remove;
    public Skill skill;
    public StatusEffectEnum statusEffectEnum;
    public StatEnum statEnum;
    public void Init(UnityAction _add,UnityAction _tick, UnityAction _remove,Unit u,int killTurn,Skill _skill,StatusEffectEnum _statusEffectEnum)
    {
        //add.Invoke();
        add = _add;
        tick = _tick;
        remove = _remove;
        unit = u;
        turnToKill = killTurn;
        skill = _skill;
        statusEffectEnum = _statusEffectEnum;
    }

    public void Tick()
    {   
      
       
    }

    public void CheckForKill()
    {
        if(BattleManager.inst.turn == turnToKill)
        {Remove();}
    }
    

    public bool willBeDead(){
        return BattleManager.inst.turn > turnToKill;
    }

    public void Remove(){
        unit.RemoveStatusEffect(this);

    }
}