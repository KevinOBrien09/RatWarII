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
    public void Init(Unit u,int dmg){
        caster = u;
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
 if(s.unit != null)
                {
                    if(s.unit != caster){
                        //if(s.unit.side != caster.side){
                            s.unit.Hit(damage,null);
                      //  }
                    }
                   
                }
                }
               
            }
        }
       
    }

}