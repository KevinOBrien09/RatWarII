
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEngine.UI;
using DG.Tweening;

public class BattlePositionSlot : MonoBehaviour
{
    public Vector2 coordinates;
    public RectTransform unitHolder,rt;
    public BattlePositionDragger dragger;
    public void Take(BattlePositionDragger d,bool isSwap = false){
        dragger = d;
        dragger.startParent = unitHolder;
        dragger.transform.SetParent(unitHolder);
        dragger.slot = this;
        dragger.SetY();
    
       
        Party xd =  PartyManager.inst.parties[PartyManager.inst.currentParty];
        if(!isSwap){
          
            xd.battlePositions.Remove(xd.members[dragger.character.ID].battlePosition);
            xd.members[dragger.character.ID].battlePosition = coordinates;
            xd.battlePositions.Add(xd.members[dragger.character.ID].battlePosition,dragger.character.ID);
            xd.SavePartyEdit();
    
        }
        else{
         
            xd.members[dragger.character.ID].battlePosition = coordinates;
        }
         
       
    }

    public void Remove()
    {
        if(dragger !=null){
           dragger.slot = null;
        }
        dragger = null;
    }

}