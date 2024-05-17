using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class SlashCast : SkillCastBehaviour
{
    public int damage;
    public ParticleSystem slashVFXPrefab;
    public override void Go(CastArgs args)
    {
        StartCoroutine(q());
        IEnumerator q()
        {
            ParticleSystem ps =   Instantiate(slashVFXPrefab,args.caster.slot.transform.position,Quaternion.identity);
            ps.Play(); 
            PlaySound(0,args.skill);
            Vector3 ogRot = args.caster.transform.rotation.eulerAngles;
            StartCoroutine(Rotate(ps.main.duration));
            IEnumerator Rotate(float duration)
            {
                float startRotation = transform.eulerAngles.y;
                float endRotation = startRotation + 360.0f;
                float t = 0.0f;
                while ( t  < duration )
                {
                    t += Time.deltaTime;
                    float yRotation = Mathf.Lerp(startRotation, endRotation, t / duration) % 360.0f;
                      args.caster.transform.eulerAngles = new Vector3( 0, yRotation, 0);
                    yield return null;
                }
            }
            
            yield return new WaitForSeconds(ps.main.duration);
            args.caster.transform.rotation = Quaternion.Euler(ogRot.x,ogRot.y,ogRot.z);
            foreach (var item in SkillAimer.inst.validSlots)
            {
                if(item != args.caster.slot)
                {
                    
                    if(item.cont.unit != null)
                    {

                        if(item.cont.unit.side != args.caster.side)
                        {  
                            Unit i = item.cont.unit;
                            
                            if(i.health.willUnitDie(damage)){
                            
                            }
                            Knockback.Hit(1,args.caster,i,true);
                            i.Hit(damage,args);
                          
                            
                        }
                        else if(!item.cont.unit.isEntity()){
                            Unit i = item.cont.unit;
                            i.Hit(damage,args);  
                        }
                        
                    }
                  
                }
            }
            yield return new WaitForSeconds(.5f);


            SkillAimer.inst.Finish(); 
        }
    }
}
