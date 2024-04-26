
using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using DG.Tweening;


public class FireballCast : SkillCastBehaviour
{
    public GameObject fireballPrefab;
    public ParticleSystem explosion;
    public int traDmg,explDMG;
    
    public override void Go(CastArgs args)
    {
        args.caster.Flip(args.targetSlot.transform.position);
        GameObject fb = Instantiate(fireballPrefab);
        fb.transform. GetChild(1).GetComponent<DamageUnitPassThrough>().Init(args,traDmg);
        fb.transform.position = args.caster.transform.position;

        CamFollow.inst.Focus(fb.transform,(()=>
        {
            CamFollow.inst.target = fb.transform;
           
            PlaySound(0,args.skill);
            CamFollow.inst.ChangeCameraState(CameraState.LOCK);
        }));



        StartCoroutine(q());
        IEnumerator q()
        {
            yield return new WaitForSeconds(.7f);
            fb.transform.DOMove(args.targetSlot.transform.position,.25f).OnComplete(()=>
            {   

                StartCoroutine(c());
                IEnumerator c()
                {
                    yield return new WaitForSeconds(.1f);
                    Instantiate(explosion,fb.transform.position,Quaternion.identity).Play();
                    Destroy(fb.gameObject);
                    PlaySound(1,args.skill);
                    if(args.targetSlot.cont.unit != null)
                    {args.targetSlot.cont.unit.Hit(explDMG,args);}
                    yield return new WaitForSeconds(1f);
                    SkillAimer.inst.Finish();
                }
            });
        
        }
       
    }



     
}