using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using DG.Tweening;


public class AOEHealCast : SkillCastBehaviour
{
    public int value;
    public ParticleSystem healVFXPrefab;

    public override void Go(CastArgs args)
    {
        StartCoroutine(q());
        IEnumerator q()
        {
            ParticleSystem ps =   Instantiate(healVFXPrefab,args.caster.slot.transform.position,Quaternion.identity);
            ps.Play(); 
            
            Vector3 ogRot = args.caster.transform.rotation.eulerAngles;
           
            yield return new WaitForSeconds(1);
            args.caster.transform.rotation = Quaternion.Euler(ogRot.x,ogRot.y,ogRot.z);
            foreach (var item in SkillAimer.inst.validSlots)
            {
               
                    
                if(item.cont.unit != null)
                {
                    if(item.cont.unit.side == args.caster.side)
                    {
                        if(item.cont.unit. health.notFull())
                        {
                            item.cont.unit.Heal(value);
                        }
                    }
                    
                }
                    
                
            }
            yield return new WaitForSeconds(1f);
            SkillAimer.inst.Finish();       
        }
    }


}