using System;
using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;
public class CharacterDragger : DraggableComponent
{
    public CharacterTab tab;
    public Canvas canvas;
    Transform hi;

    void Start(){
        hi =GameObject.Find("PriorityTransform").transform;
        
    }


   
    public void Init(CharacterTab _tab){
        tab = _tab;
        currentSlot = tab.equipmentSlot;

    }

    public override void Snap()
    {
        CharacterCell cc = currentSlot as CharacterCell;
        if(cc != null)
        {
            if(cc.cellType == CharacterCell.CellType.TAB)
            {
                base.Snap();
            }
            else
            {
                cc.character = null;
                RemoveFromParty();
            }
        }
    }

    public void RemoveFromParty(){

       tab.equipmentSlot.Drop(this);
    }



    public override bool Accept(EquipmentSlot slot)
    {
  
        return true;
    }

    public override void OnDrag(PointerEventData eventData){
        base.OnDrag(eventData);
        transform.SetParent(hi);
    }

     public override void OnEndDrag(PointerEventData eventData){
        base.OnEndDrag(eventData);
        currentSlot.Drop(this);
     
    }

    
}