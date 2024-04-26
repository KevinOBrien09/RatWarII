using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.Events;
using DG.Tweening;
public class DamageUnitPassThrough : MonoBehaviour
{ 
    public int damage;
    bool initDone;
    Unit caster;
    CastArgs castArgs;
    public void Init(CastArgs args,int dmg){
        caster = args.caster;
        castArgs = args;
        initDone = true;
        damage = dmg;
    }

    void OnTriggerEnter(Collider other)
    {
        if(initDone){
        Slot s = null;
            bool slotExists = other.TryGetComponent<Slot>(out s);
            if(slotExists)
            {
                if(SkillAimer.inst.validSlots.Contains(s)){
 if(s.cont.unit != null)
                {
                    if(s.cont.unit != caster){
                        //if(s.unit.side != caster.side){
                            s.cont.unit.Hit(damage,castArgs);
                      //  }
                    }
                   
                }
                }
               
            }
        }
       
    }

}