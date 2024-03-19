
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using DG.Tweening;
public class PartyTab : MonoBehaviour
{
    public TextMeshProUGUI partyName;
    public GenericDictionary<int,RawImage> icons = new GenericDictionary<int, RawImage>();
    public List<Image> border = new List<Image>();
    public Color32 yellow;
    public Party party;
    public RectTransform holder;
    public float editPos,ogPos;
    public GameObject setActiveButton;
    public Image starIcon;

    public void Init(Party p)
    {        
        party = p;
        ChangeBorderColour(Color.black);
        UpdateIcons();
        partyName.text = p.partyName;
        party.onPartyEdit += UpdateIcons;
    }

    public void Move(float p)
    {
        holder.DOAnchorPosX(p,.2f);
    }

    public void UpdateIcons(){
        foreach (var item in icons)
        {
            item.Value.enabled = false;
        }
        foreach (var item in party.members)
        {   
            icons[item.Value.position].enabled = true;
            icons[item.Value.position].texture =  IconGraphicHolder.inst.dict[item.Value. character.ID];
        }
    }

    public void EditButton(){
        if(!BackbenchHandler.inst.editing){
            EventSystem.current.SetSelectedGameObject(null);
            BackbenchHandler.inst.EditExistingParty(party);
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
            BackbenchHandler.inst.ActivePartyRefresh();
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
