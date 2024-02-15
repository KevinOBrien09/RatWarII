using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;
 

public class BileBlastCast : SkillCastBehaviour
{
    public int baseDamage = 5;
    public ParticleSystem bilePrefab;
    int tilesTraveled;
    public override void Go(CastArgs args)
    {
        StartCoroutine(q());
        IEnumerator q()
        {
            CamFollow.inst.Focus(args.target.slot.transform,()=>
            { 
             
                CamFollow.inst.target = args.target.transform;
                CamFollow.inst.ChangeCameraState(CameraState.LOCK); 
                StartCoroutine(q());
                IEnumerator q()
                {
                    yield return new WaitForSeconds(.2f);
                    args.caster.Flip(args.targetSlot.transform.position);
                    ParticleSystem projectile = Instantiate(bilePrefab);
                    PlaySound(0,args.skill);
                    var pm = projectile.transform.GetChild(0).GetComponent<ParticleSystem>() .main;
                    float distance = Vector3.Distance(args.caster.slot.transform.position, args.targetSlot.transform.position);
                    tilesTraveled =  (int)distance/5;
                    int dist = 5;
                    for (int i = 0; i < tilesTraveled; i++)
                    { dist += 5; }

                    pm. startSpeed = dist;
                    projectile.Play();
                    projectile.transform.position = args.caster.transform.position;
                    if(args.target != null)
                    {args.target.healthBar.gameObject.transform.parent.gameObject. SetActive(true);}
                    if(args.targetSlot.node.iGridY == args.caster.slot.node.iGridY)
                    {
                        if(args.caster.facingRight)
                        {projectile.transform.rotation = Quaternion.Euler(0,90,0);}
                        else
                        {projectile.transform.rotation = Quaternion.Euler(0,-90,0);}
                    }
                    else
                    {
                        if(args.targetSlot.node.iGridY < args.caster.slot.node.iGridY)
                        {projectile.transform.rotation = Quaternion.Euler(0,180,0);}
                        else
                        {projectile.transform.rotation = Quaternion.Euler(0,0,0);}
                    }
                }   
            });  
           
            yield return new WaitForSeconds(.75f);
            PlaySound(1,args.skill);
            args.target.Hit(baseDamage,args);
            yield return new WaitForSeconds(.75f);
            SkillAimer.inst.Finish();
        }
    }
}