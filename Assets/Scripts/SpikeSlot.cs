using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class SpikeSlot : SpecialSlot 
{
    public Transform spikes;
    public int damage = 25;
    public override void Go(){
        spikes.DOLocalMoveY(.23f,.05f).OnComplete(()=>
        {
            StartCoroutine(q());
            IEnumerator q(){
            if(slot.unit != null){
                slot.unit.Hit(damage);
            }
            yield return new WaitForSeconds(.5f);
              spikes.DOLocalMoveY(.13f,.3f);
            }




        });
    }

    public override bool willUnitDie()
    {
        if(slot.unit.health.currentHealth - 25 <= 0){
            return true;
        }
        else{
            return false;
        }
    }
}