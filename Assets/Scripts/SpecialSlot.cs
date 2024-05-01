using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using DG.Tweening;

public class SpecialSlot : MonoBehaviour
{
    public List<SoundData> sounds = new List<SoundData>();
    public Slot slot;
    public SlotContents slotContents;
    public SortingGroup sorter;
    public IntrusiveMeshHandler intrusiveMesh;
    public bool createdByUnit;
    public Interactable interactable;
    public bool sotTrigger;

    public virtual void Init(){
        if(sorter != null){
          sorter.sortingOrder =   -slot.node.iGridY;
        }

    }
    
    public virtual void Tick(){

    }



    public virtual bool willUnitDie(){
        return false;
    }
}