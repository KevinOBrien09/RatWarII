using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class SpecialSlot : MonoBehaviour
{

    public string tickerText;
    public List<SoundData> sounds = new List<SoundData>();
   public Slot slot;
    public virtual void Go(){

    }

    public virtual bool willUnitDie(){
        return false;
    }
}