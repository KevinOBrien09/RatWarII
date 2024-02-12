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
            
            PlaySound(0,args.skill);
            TempTerrain pillar = Instantiate(pillarPrefab, args.targetSlot.transform.position,Quaternion.identity);   
            pillar.transform.DOMoveY(-4,0);
            args.targetSlot.tempTerrain = pillar;
            args.caster.tempTerrainCreated.Add(pillar);
            pillar.turnToDieOn = pillar.howManyTurns + BattleManager.inst.turn;
            pillar.slot = args.targetSlot;
            CamFollow.inst.target = args.targetSlot.transform;
            CamFollow.inst.Focus(args.targetSlot.transform,()=>
            { 
                CamFollow.inst.ChangeCameraState(CameraState.LOCK);
                pillar.transform.DOMoveY(0,.25F);
                MapManager.inst.grid.UpdateGrid();
            
            });
         
            yield return new WaitForSeconds(.6f);

            SkillAimer.inst.Finish();
        }
    }

}