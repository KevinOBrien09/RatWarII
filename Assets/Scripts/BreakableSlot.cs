using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.EventSystems;
using System.Linq;
public class BreakableSlot : Unit
{

    public ParticleSystem kill;
    public SoundData killSfx;
    public override void Hit(int damage, CastArgs castArgs, bool bleed = false)
    {
        Debug.Log("XD");
        AudioManager.inst.GetSoundEffect().Play(killSfx);
        kill.transform.SetParent(null);
        kill.Play();
        slot.cont.unit = null;
        Destroy(gameObject);
        MapManager.inst.map.UpdateGrid();
    }

    public override void Die()
    {
        
    }

    public override bool isEntity()
    {
        return false;
    }



}