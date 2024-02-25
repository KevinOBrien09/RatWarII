using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
public class PillarCast :  SkillCastBehaviour
{
    public TempTerrain pillarPrefab;
    public override void Go(CastArgs args)
    {
        StartCoroutine(q());
        IEnumerator q()
        {   
            args.caster.Flip(args.targetSlot.transform.position);
            PlaySound(0,args.skill);
            SpecialSlot pillar = args.targetSlot.MakeSpecial((SpecialSlot) pillarPrefab);
            TempTerrain tt = pillar as TempTerrain;
            args.targetSlot.DisableHover();
            pillar.transform.DOMoveY(-4,0);
            args.caster.tempTerrainCreated.Add(tt);
            tt.turnToDieOn = tt.howManyTurns + BattleManager.inst.turn;
            pillar.slot = args.targetSlot;
            pillar.slot.cont.wall = true;
            //pillar.slot.cont.slotContents.Add(pillar.slotContents);
            CamFollow.inst.target = args.targetSlot.transform;
            CamFollow.inst.Focus(args.targetSlot.transform,()=>
            { 
                CamFollow.inst.ChangeCameraState(CameraState.LOCK);
                pillar.transform.DOMoveY(0,.25F);
                MapManager.inst.map.UpdateGrid();
            
            });

            tt.onKill =()=>
            {
                tt.createdByUnit = true;
                Destroy( tt.col.gameObject);
                tt.slot.cont.wall = false;
               // pillar.slot.cont.slotContents.Remove(pillar.slotContents);
                 MapManager.inst.map.UpdateGrid();
                tt.gameObject.transform.DOMoveY(-4,.25f).OnComplete(()=>{
                Destroy(tt.gameObject);
                });
            };
         
            yield return new WaitForSeconds(.6f);

            SkillAimer.inst.Finish();
        }
    }

}