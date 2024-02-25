
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Interactable : MonoBehaviour
{
    public Slot slot;
    
    public virtual void Go(Unit investigator)
    {
        Debug.Log(gameObject.name + " Interactable");
    }

    public void Kill()
    {
        slot.cont.slotContents.Remove(slot.cont.specialSlot.slotContents);
        slot.cont.specialSlot = null;
        if(slot.cont.wall)
        {slot.cont.wall = false;}
         MapManager.inst.map.UpdateGrid();
        Destroy(gameObject);
    }

}