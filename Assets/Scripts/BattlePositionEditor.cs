
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEngine.UI;
using DG.Tweening;

public class BattlePositionEditor : MonoBehaviour
{
    public GameObject holder;
    public List<BattlePositionSlot> slots = new List<BattlePositionSlot>();
    public Dictionary <Vector2,BattlePositionSlot> unitPlacers = new  Dictionary<Vector2, BattlePositionSlot>();
    public List<BattlePositionDragger> draggers = new List<BattlePositionDragger>();
    public BattlePositionDragger draggerPrefab;
    public Party party;
    void Start(){
        foreach (var item in slots)
        {
            unitPlacers.Add(item.coordinates,item);
        }
    }
    public void OpenHubButton(){
        party = BackbenchHandler.inst.editingParty;
        Open();
    }
    public void Open(){
        if(HubStateHandler.inst.currentState == HubStateHandler.HubState.PARTYEDIT)
        {
            Clear();
            holder.SetActive(true);
            HubStateHandler.inst.ChangeStateString("Party-Positions");
            HubStateHandler.inst.ChangeState(  HubStateHandler.HubState.BATTLE_POSITIONS);
            SpawnDraggers();
           
        }
      
    }

    public void SpawnDraggers()
    {
        if(party != null){
            foreach (var item in party.members)
            {
                BattlePositionSlot slot = unitPlacers[item.Value.battlePosition];
                BattlePositionDragger dragger = Instantiate(draggerPrefab,slot.unitHolder);
                dragger.SetY();
                dragger.Init(item.Value.character,this,slot);
                draggers.Add(dragger);
            }
        }
        else{
            Debug.LogWarning("PARTY IS NULL!!");
        }       
    }

    public void Reset(){
        Clear();
        party = null;
    }

    public void Clear(){
        foreach (var item in unitPlacers)
        {item.Value.Remove();}
        foreach (var item in draggers)
        {
            Destroy(item.gameObject);
        }
        draggers.Clear();
    }

    public void Close(){
        holder.SetActive(false);
        Reset();
        HubStateHandler.inst.ChangeStateString("Party-Edit");
        HubStateHandler.inst.ChangeState(  HubStateHandler.HubState.PARTYEDIT);
    }

}