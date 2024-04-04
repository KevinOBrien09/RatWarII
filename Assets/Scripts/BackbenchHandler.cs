
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEngine.UI;
using DG.Tweening;

public class BackbenchHandler : Singleton<BackbenchHandler>
{
    public GameObject canvasGO;
    public float shown,hidden,partyShown,partyHidden;
    public RectTransform rt,currentPartyRT,tabHolder;
    public CharacterTab tabPrefab;
    public PartyTab partyTabPrefab;
    public TextMeshProUGUI textPrefab;
    public TextMeshProUGUI capacityText;
    public List<CharacterCell> cells = new List<CharacterCell>();
    public  GenericDictionary<string,CharacterTab> tabs = new  GenericDictionary<string,CharacterTab>();
    public GenericDictionary<string,CharacterTab> inPartyTabs = new GenericDictionary<string,CharacterTab>();
    public GenericDictionary<string, PartyTab> pTabs = new GenericDictionary<string, PartyTab>();
    public List<TextMeshProUGUI> text = new List<TextMeshProUGUI>();
    public TMP_InputField nameField;
    public Party editingParty;
    public bool editing;
    public GameObject popUps;
    public Transform seperatorGO;
    public UnityEvent onPartyEdit;
    public Party partyToBeDeleted;
    public Character characterToBeDismissed;
    void Start(){
       rt.DOAnchorPosX(hidden,0);  
       currentPartyRT.DOAnchorPosY(partyHidden,0).OnComplete(()=>{
       canvasGO.SetActive(false);
       });  
    }


    public void NewParty()
    {
       if(!editing)
       {
            Party p = new Party();
            p.Create();
            
            PartyTab pt = MakePartyTab(p);
            pt.transform.SetSiblingIndex(1);
            PartyManager.inst.AddNewParty(p);
            
            EnterEdit(p);
       }
   
      
    }

    public void EditExistingParty(Party p){
   
        EnterEdit(p);
        foreach (var item in p.members)
        {
            CharacterTab t =   Instantiate(tabPrefab,tabHolder);
            t.Init(item.Value.character);
            t.IsPartyMember(cells[item.Value.position]);
            t.transform.SetSiblingIndex(seperatorGO.GetSiblingIndex()+1);
            tabs.Add(item.Value.character.ID, t);
            inPartyTabs.Add(item.Value.character.ID, t);
            t.ToggleDismissButton(false);
        }
        PartyManager.inst.currentParty = p.ID;
    }

    public void EnterEdit(Party p){
        editingParty = p;
        EventSystem.current.gameObject.GetComponent<Rewired.Integration.UnityUI.RewiredStandaloneInputModule>().deselectIfBackgroundClicked = true;
        HubStateHandler.inst.ChangeState(HubStateHandler.HubState.PARTYEDIT);
        HubStateHandler.inst.ChangeStateString("Party-Edit");
       // pTabs[editingParty.ID].ChangeBorderColour(  pTabs[editingParty.ID].yellow);
        pTabs[editingParty.ID].Move( pTabs[editingParty.ID].editPos);
       HubCharacterDisplay.inst.AddListener(p);
        EventSystem.current.SetSelectedGameObject(null);
       
        nameField.text = p.partyName;
        editing = true;

        foreach (var item in pTabs)
        {
          
            item.Value.ChangeBorderColour(Color.black);
        }
        pTabs[editingParty .ID].ChangeBorderColour( pTabs[editingParty .ID].yellow);

             HubCharacterDisplay.inst.Refresh();
        currentPartyRT.DOAnchorPosY(partyShown,.15f);  
    }

    public void LeaveEdit()
    {
        SaveLoad.Save(GameManager.inst.saveSlotIndex);
        EventSystem.current.gameObject.GetComponent<Rewired.Integration.UnityUI.RewiredStandaloneInputModule>().deselectIfBackgroundClicked = false;
        if(editingParty != null)
        {
            editingParty.partyName = nameField.text;
           
            if(pTabs.ContainsKey(editingParty.ID))
            { 
                pTabs[editingParty.ID].Move( pTabs[editingParty.ID].ogPos);
                  pTabs[editingParty.ID].ChangePartyName();
            }


            foreach (var item in editingParty.members)
            {
                if(tabs.ContainsKey(item.Value.character.ID))
                {
                
                    tabs[item.Value.character.ID].dragger.dragDropCell.draggable = null;
                    Destroy(    tabs[item.Value.character.ID].dragger.gameObject);
                    Destroy(    tabs[item.Value.character.ID].gameObject);
                    tabs.Remove(item.Value.character.ID);
                }

                if(inPartyTabs.ContainsKey(item.Value.character.ID))
                {
                    inPartyTabs[item.Value.character.ID].dragger.dragDropCell.draggable = null;
                    Destroy(inPartyTabs[item.Value.character.ID].dragger.gameObject);
                    Destroy(inPartyTabs[item.Value.character.ID].gameObject);
                }
            }
        }
        
        foreach (var item in pTabs)
        { item.Value.ChangeBorderColour(Color.black); }
        if(PartyManager.inst.parties.ContainsKey(editingParty .ID))
        { pTabs[editingParty .ID].ChangeBorderColour( pTabs[editingParty .ID].yellow); }
        else
        { pTabs[PartyManager.inst.currentParty].ChangeBorderColour( pTabs[PartyManager.inst.currentParty].yellow); }
        
            
        HubCharacterDisplay.inst.RemoveListeners();
        inPartyTabs.Clear();
        editingParty = null;
       
        EventSystem.current.SetSelectedGameObject(null);
        editing = false;
        HubCharacterDisplay.inst.Refresh();
        foreach (var item in tabs)
        {
            item.Value.ToggleDismissButton(true);
        }
        
       
        HubStateHandler.inst.ChangeStateString("Party");
        HubStateHandler.inst.ChangeState(  HubStateHandler.HubState.ORGANIZER);
        currentPartyRT.DOAnchorPosY(partyHidden,.15f);  
        
    }

    public bool CheckIfReady()
    {
        if(editingParty.members.Count == 0){
            popUps.SetActive(true);
            popUps.transform.GetChild(1).gameObject. SetActive(true);
            return false;
        }
        return true;
    }

    public void ReturnToPartyEditFromButton()
    { 
        EventSystem.current.SetSelectedGameObject(null);
        popUps.SetActive(false);
        popUps.transform.GetChild(1).gameObject.SetActive(false);
      
    }

    public void DiscardCurrentEditPartyFromButton()
    { 
       DiscardParty(editingParty);
    }

    public void DismissCharacterPopup(){
        popUps.SetActive(true);
        popUps.transform.GetChild(3).gameObject.SetActive(true);
    }

    public void ReturnFromDismiss(){
        characterToBeDismissed = null;
        popUps.SetActive(false);
        popUps.transform.GetChild(3).gameObject.SetActive(false);
    }

    public void ActuallyDismiss()
    {
        PartyManager.inst.Dismiss(characterToBeDismissed);
        popUps.SetActive(false);
        popUps.transform.GetChild(3).gameObject.SetActive(false);
        CharacterTab ct =  tabs[characterToBeDismissed.ID];
        if(ct.dragger.dragDropCell !=null)
        {ct.dragger.dragDropCell.draggable = null;}
        Destroy(  ct.dragger.gameObject);
        if(tabs.ContainsKey(characterToBeDismissed.ID))
        {tabs.Remove(characterToBeDismissed.ID);}
        if(inPartyTabs.ContainsKey(characterToBeDismissed.ID))
        {inPartyTabs.Remove(characterToBeDismissed.ID);}
        Destroy(ct.gameObject);
        characterToBeDismissed = null;
    }

    public void DiscardParty(Party p){
 EventSystem.current.SetSelectedGameObject(null);
        popUps.SetActive(false);
        popUps.transform.GetChild(1).gameObject.SetActive(false);
       Dictionary<string, CharacterHolder> d =  new Dictionary<string, CharacterHolder>(p.members) ;
        foreach (var item in d)
        {
            p.PartyToBench(item.Value.character);
        }
     
        PartyManager.inst.RemoveParty(p);
        Destroy(pTabs[p.ID].gameObject);
        pTabs.Remove(p.ID);
             KillTabs();
        SpawnTabs();      
        LeaveEdit();
    }

    public void DeletePopUp(){
        popUps.SetActive(true);
        popUps.transform.GetChild(2).gameObject.SetActive(true);
    }

    public void NoDelete(){
        popUps.SetActive(false);
        popUps.transform.GetChild(2).gameObject.SetActive(false);
        partyToBeDeleted = null;
    }

    public void ConfirmDeletion(){
        BackbenchHandler.inst.DiscardParty(partyToBeDeleted);
        NoDelete();
        HubCharacterDisplay.inst.Refresh();
    }

   
    public void Show(){
        SpawnTabs(); 
        rt.DOAnchorPosX(shown,.15f);
        
    }

    void SpawnTabs(){
    
        TextMeshProUGUI partyText =  Instantiate(textPrefab,tabHolder);
        text.Add(partyText);
        partyText.autoSizeTextContainer= true; // doesnt work???
        partyText.fontSize = 40;
        partyText.text = "<u>Parties";
        partyText.ForceMeshUpdate(true, true);
        partyText.transform.SetSiblingIndex(0);
        if(PartyManager.inst.parties.Count == 0)
        {
            TextMeshProUGUI noPartyText =  Instantiate(textPrefab,tabHolder);
            text.Add(noPartyText);
            noPartyText.fontSize = 25;
        
            noPartyText.text = "No Parties";
        }
        else
        {
        foreach (var item in PartyManager.inst.parties)
        {
            if(item.Value.mapTileID == LocationManager.inst.currentLocation){
            MakePartyTab(item.Value);
            }
          
           
         
        }
        }
        foreach (var item in pTabs)
        {
          
            item.Value.ChangeBorderColour(Color.black);
        }
        if(PartyManager.inst.currentParty != string.Empty){
            pTabs[PartyManager.inst.currentParty].ChangeBorderColour( pTabs[PartyManager.inst.currentParty].yellow);
        }
        
        TextMeshProUGUI seperator =  Instantiate(textPrefab,tabHolder);
        text.Add(seperator);
        seperator.text = "----------------------";
        seperatorGO = seperator.gameObject.transform;
        RectTransform rt =  seperator.GetComponent<RectTransform>();
        rt.sizeDelta = new Vector2(rt.sizeDelta.x,25);
        foreach (var item in PartyManager.inst.benched)
        {
            if(item.Value.mapTileID == LocationManager.inst.currentLocation){
                CharacterTab t =   Instantiate(tabPrefab,tabHolder);
                t.Init(item.Value.character);
                tabs.Add(item.Value.character.ID, t);
            }
          
        }
        
        capacityText.text = tabs.Count +"/12";
    }

   

    PartyTab MakePartyTab(Party p)
    {  
        PartyTab pTab = Instantiate(partyTabPrefab,tabHolder);
        pTab.Init(p);
        pTabs.Add(p.ID, pTab);
        return pTab;
    }
    
    void KillTabs()
    {
        foreach (var item in tabs)
        {
            item.Value.dragger.dragDropCell.draggable = null;
            Destroy(item.Value. dragger.gameObject);
            Destroy(item.Value. gameObject);
        }

        foreach (var item in text)
        {
            Destroy(item.gameObject);
        }
        
        foreach (var item in pTabs)
        {
            Destroy(item.Value. gameObject);
        }
        pTabs.Clear();
        tabs.Clear();
        text.Clear();
    }

    public void Hide(){
        KillTabs();
        WorldHubCamera.inst.cam.DOFieldOfView(40,.2f);
        rt.DOAnchorPosX(hidden,.15f).OnComplete(()=>{
            //canvasGO.SetActive(false);
        });
        currentPartyRT.DOAnchorPosY(partyHidden,.15f);  
    }

}