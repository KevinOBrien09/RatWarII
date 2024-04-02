
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using DG.Tweening;

public class PartyTab : MonoBehaviour
{
    public TextMeshProUGUI partyName,partyLevel;
    public GenericDictionary<int,PartyTabIcon> icons = new GenericDictionary<int,PartyTabIcon>();
    public List<Image> border = new List<Image>();
    public List<GameObject> mapModeToggles = new List<GameObject>();
    public Color32 yellow;
    public Party party;
    public RectTransform holder;
    public float editPos,ogPos;
  
   
    public Button mainTabButton;
 

    public void Init(Party p)
    {        
        party = p;
        ChangeBorderColour(Color.black);
        UpdateIcons();

        partyLevel.text = p.TotalPartyLevel().ToString();
        partyName.text = p.partyName;
        party.onPartyEdit += UpdateIcons;
    }

    public void SetToMapMode(){
        foreach (var item in mapModeToggles)
        {
               item.SetActive(false);
        }
        foreach (var item in icons)
        {item.Value.rawImage.raycastTarget = false;
            
        }
        mainTabButton.enabled = true;
    
    }

    public void Move(float p)
    {
        holder.DOAnchorPosX(p,.2f);
    }

    public void ChangePartyName(){
                partyName.text = party.partyName;
    }

    public void UpdateIcons()
    {
        foreach (var item in icons)
        {
            item.Value.Wipe();
        }
        foreach (var item in party.members)
        {   
            icons[item.Value.position].Init(item.Value.character);
            
        }
    }

    public void EditButton(){
        if(CharacterStatSheet.inst.open){
            return;
        }

        if(!BackbenchHandler.inst.editing){
            EventSystem.current.SetSelectedGameObject(null);
            BackbenchHandler.inst.EditExistingParty(party);
        }
        else
        {
            BackbenchHandler.inst.LeaveEdit();
        }
 
    }

    public void Discard(){
       BackbenchHandler.inst.partyToBeDeleted = party;
       BackbenchHandler.inst.DeletePopUp();
    }

    public void SetActiveParty()
    {
        if(PartyManager.inst.currentParty != party.ID)
        {
         
       
   
            PartyManager.inst.currentParty = party.ID;
            HubCharacterDisplay.inst.Refresh();
      
           // ChangeBorderColour(yellow);
        }
    }

    public void ChangeBorderColour(Color c){
        foreach (var item in border)
        {item.color = c;}
    }

    void OnDestroy(){
        party.onPartyEdit -= UpdateIcons;
    }
}
