using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections.Generic;
public class CharacterDragger : Draggable ,IPointerClickHandler
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
            Party.inst.PartyToBench(tab.character);
        }
        else
        {
            base.Snap();
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
       if(eventData.button == PointerEventData.InputButton.Right){
        Debug.Log("RightClick");
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
        Party.inst.UpdatePosition(da.tab.character,cca.position);
        Party.inst.UpdatePosition(db.tab.character,ccb.position);
    }

  
}