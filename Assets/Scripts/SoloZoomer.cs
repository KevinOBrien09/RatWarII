using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using DG.Tweening;

public class SoloZoomer : Zoomer
{
    public Transform holder;
    public HealthBar healthBar;
    public override void Go(CastArgs args,UnityAction action)
    {
        CamFollow.inst.STOPMOVING = true;
        CamFollow.inst.target = args.caster.slot.transform;
        Unit u = args.caster;
        
        float  ogY = u.transform.position.y;
        Transform ogParent = u.transform.parent;
        u.transform.SetParent(holder);
        u.transform.DOLocalMove(Vector3.zero,.1f);
        u.activeUnitIndicator.gameObject.SetActive(false);

        healthBar.health = u.health;
        healthBar.Refresh();
        UnityAction HpAction = ()=> {healthBar.Refresh();};

     
        healthBar.gameObject.SetActive(true);
      
        foreach (var item in u.graphic.allRenderers)
        {item.sortingLayerName = "Zoom";}
        //CamFollow.inst.ForceFOV(45);
        StartCoroutine(BeginMove());
        IEnumerator BeginMove()
        {

            yield return new WaitForSeconds(.2f);
            action.Invoke();
            healthBar.Refresh();
            yield return new WaitForSeconds(.5f);


            u.transform.SetParent(ogParent);
            u.transform.DOMove(new Vector3(u.slot.transform.position.x,ogY,u.slot.transform.position.z) ,.2f).OnComplete(()=>{

              
                u.graphic.ChangeSpriteSorting(u.slot.node);
            });

            CamFollow.inst.Focus(u.slot.transform,()=>
            { CamFollow.inst.ChangeCameraState(CameraState.LOCK); });
           // CamFollow.inst.ForceFOV( CamFollow.inst.baseFOV);
            yield return new WaitForSeconds(.45f);
    
            u. health.onHit.RemoveListener(HpAction);
            // if(args.skill.ID == "35433542-3142-45a9-b3d4-93096ef99883")
            // {
            //     ParticleSystemRenderer r =   u.transform.Find("shield") .GetComponent<ParticleSystemRenderer>();
            //     r.sortingLayerName = "Unit";
            //     r.sortingOrder = u.slot.node.iGridY * 10;
            
            // }
           
            healthBar.health = null;
            group.DOFade(0,.2f).OnComplete(()=>
            {Destroy(gameObject);});
            SkillAimer.inst.Finish();


    }
    }

}