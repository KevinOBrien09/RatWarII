using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class PullTowardsCast : SkillCastBehaviour
{
    public GameObject grabberPrefab;
   
    public override void Go(CastArgs args)
    {
        GameObject grabber = Instantiate(grabberPrefab,args.caster.slot.cont.unit.transform);
        Transform tip = grabber.transform. GetChild(0);
        args.caster.Flip(args.targetSlot.transform.position);
        
        CamFollow.inst.target = tip;
        CamFollow.inst.ForceFOV(35);

        float distance = Vector3.Distance(args.caster.slot.transform.position, args.targetSlot.transform.position);
        float tilesTraveled =  (int)distance/5;
        float scale = .25f;
        for (int i = 0; i < tilesTraveled; i++)
        {scale +=.5f;}

        grabber.transform.DOScaleX(0,0);
        CamFollow.inst.Focus(tip,(()=>
        {CamFollow.inst.ChangeCameraState(CameraState.LOCK);
            grabber.transform.DOScaleX(scale,.33f).OnComplete(()=>
            {
                CamFollow.inst.target = args.target.transform;
                
                Slot s = null;  
                Vector3 overShoot = new Vector3();
                Vector3 p = new Vector3();
                if(args.caster.slot.node.iGridX > args.target.slot.node.iGridX) //pulled to left
                {
                    Vector2 v = new Vector2(args.caster.slot.node.iGridX-1,args.caster.slot.node.iGridY);
                    if(MapManager.inst.nodeIsValid(v))
                    {
                        s =  MapManager.inst.map.NodeArray[(int) v.x, (int)v.y].slot;
                        p = new Vector3(s.transform.position.x,args.target.transform.position.y,s.transform.position.z);
                        overShoot = new Vector3(p.x+2.5f,p.y,p.z);

                    }

                }
                else //pulledToRight
                {
                    Vector2 v = new Vector2(args.caster.slot.node.iGridX+1,args.caster.slot.node.iGridY);
                    if(MapManager.inst.nodeIsValid(v))
                    {
                        s =  MapManager.inst.map.NodeArray[(int) v.x, (int)v.y].slot;
                        p = new Vector3(s.transform.position.x,args.target.transform.position.y,s.transform.position.z);
                        overShoot = new Vector3(p.x-2.5f,p.y,p.z);
                        
                    }
                }
                if(s!=null)
                {
                    grabber.transform.DOScaleX(0,.2f).OnComplete(()=>{
                        Destroy(grabber.gameObject);
                    });
                    args.target.transform.DOMove(overShoot,.2f).OnComplete(()=>
                    {args.target.transform.DOMove(p,.3f);});
                    args.target.Reposition(s);

                    CamFollow.inst.ForceFOV(CamFollow.inst.baseFOV);
                    StartCoroutine(q());
                }
                else
                {  
                    StartCoroutine(q());
                    Debug.LogAssertion("SLOT IS NULL!!!");
                }
            });

            IEnumerator q(){
                yield return new WaitForSeconds(1f);
                SkillAimer.inst.Finish();
            }
           


        }));
        
        
    }

}