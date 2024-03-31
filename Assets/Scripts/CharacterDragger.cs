using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections.Generic;
public class CharacterDragger : Draggable 
{
   public CharacterTab tab;
   public SoundData drop,snap;


    public override void NoCell()
    {
        CharacterCell a = dragDropCell as CharacterCell;
        if(a.cellType == CharacterCell.CellType.PARTY)
        {  tab.inPartySignifier.gameObject.SetActive(false);
            tab.cell.Take(this);
            AudioManager.inst.GetSoundEffect().Play(drop);
            BackbenchHandler.inst.editingParty.PartyToBench(tab.character);
            tab.ToggleDismissButton(true);
        }
        else
        {
            base.Snap();
        }
    }

    public override void OnBeginDrag(PointerEventData eventData){
        if(!CharacterStatSheet.inst.open){
base.OnBeginDrag(eventData);
        }
        
    }

    public override void OnDrag(PointerEventData eventData){
        if(!CharacterStatSheet.inst.open){
base.OnDrag(eventData);
        }
    }

     public override void OnEndDrag(PointerEventData eventData){
        if(!CharacterStatSheet.inst.open){
base.OnEndDrag(eventData);
        }
    }

    

    

    public override void Swap(DragDropCell sittingCell)
    {
        Draggable a = sittingCell.draggable;
        Draggable b = this;
        DragDropCell cellA = sittingCell;
        DragDropCell cellB = b.dragDropCell;
        cellA.draggable = b;
        b.dragDropCell = cellA;
        b.startParent = cellA.holder;

        cellB.draggable = a;
        a.dragDropCell = cellB;
        a.startParent = cellB.holder;
        
        a.Snap();
        b.Snap();


        CharacterCell cca = cellA as CharacterCell;
        CharacterCell ccb = cellB as CharacterCell;

        
        CharacterDragger da = cca.draggable as CharacterDragger;
        CharacterDragger db = ccb.draggable as CharacterDragger;
        BackbenchHandler.inst.editingParty. UpdatePosition(da.tab.character,cca.position);
        BackbenchHandler.inst.editingParty.UpdatePosition(db.tab.character,ccb.position);
    }

  
}