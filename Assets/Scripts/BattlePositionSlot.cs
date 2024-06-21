
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
    public void Take(BattlePositionDragger d){
        dragger = d;
        dragger.startParent = unitHolder;
        dragger.transform.SetParent(unitHolder);
        dragger.slot = this;
        dragger.SetY();
        if(GameManager.inst. loadFromFile)
        {
            Party xd =  PartyManager.inst.parties[PartyManager.inst.currentParty];
            xd.members[dragger.character.ID].battlePosition = coordinates;
            xd.SavePartyEdit();
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