using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class CharacterCell : EquipmentSlot
{

    public enum CellType{TAB,PARTY}
    public CellType cellType;
    public Character character;

    public override void Drop(DraggableComponent draggable)
    {
     
        CharacterDragger drag = draggable as CharacterDragger;
        character = drag.tab.character;
        if(cellType == CellType.TAB)
        {
            if(Party.inst.activeParty.Contains(character))
            {      
                drag.tab.inPartySignifier.SetActive(false);
                Party.inst.activeParty.Remove(character);
                Party.inst.benched.Add(character);
            }
        }
        else if (cellType == CellType.PARTY)
        {
            if(Party.inst.benched.Contains(character))
            {
                drag.tab.inPartySignifier.SetActive(true);
                Party.inst.benched.Remove(character);
                Party.inst.activeParty.Add(character);
            }
        }
        base.Drop(draggable);
        Party.inst.onPartyEdit.Invoke();

    }
}