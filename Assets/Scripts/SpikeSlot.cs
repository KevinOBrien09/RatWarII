using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class SpikeSlot : SpecialSlot 
{
    public Transform spikes;
    public int damage = 25;
    public override void Tick()
    {
        
        spikes.DOLocalMoveY(.23f,.05f).OnComplete(()=>
        {
            AudioManager.inst.GetSoundEffect().Play(sounds[0]);
            StartCoroutine(q());
            IEnumerator q(){
            if(slot.cont.unit != null){
                slot.cont.unit.Hit(damage,null);
                AudioManager.inst.GetSoundEffect().Play(sounds[1]);
            }
            yield return new WaitForSeconds(.5f);
              spikes.DOLocalMoveY(.13f,.3f);
            }




        });
    }

    public override bool willUnitDie()
    {
        if(slot.cont.unit.health.currentHealth - 25 <= 0){
            return true;
        }
        else{
            return false;
        }
    }
}