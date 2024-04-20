using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SlotContainer
{
     public Slot slot;
     public Unit unit;
     public List<SlotContents> slotContents = new List<SlotContents>();
     public SpecialSlot specialSlot;
     public bool wall;
     public bool invisible;
     
     public void AddContent(SlotContents slotc)
     {slotContents.Add(slotc);}
     
     public bool walkable()
     {
          if(slot.node == null)
          {
               Debug.Log("Overriden Tile");
               return false;
          }
          if(slot.node.isBlocked | slot.cont.wall| slot.isWater)
          { return false; }
          else if(unit != null)
          { return unit.stats().passable; }
          else if(unit == null && !slot.node.isBlocked)
          { return true;}
          return true;
    
     }

}