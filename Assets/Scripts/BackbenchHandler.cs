
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
    public Party editingParty;
    public bool editing;
    public GameObject popUps;
    public Transform seperatorGO;
    public UnityEvent onPartyEdit;
    public Party partyToBeDeleted;
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
            ActivePartyRefresh();
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
        }
    }

    public void EnterEdit(Party p){
        editingParty = p;
        HubStateHandler.inst.ChangeState(HubStateHandler.HubState.PARTYEDIT);
        HubStateHandler.inst.ChangeStateString("Party-Edit");
       // pTabs[editingParty.ID].ChangeBorderColour(  pTabs[editingParty.ID].yellow);
        pTabs[editingParty.ID].Move( pTabs[editingParty.ID].editPos);
       HubCharacterDisplay.inst.AddListener(p);
        EventSystem.current.SetSelectedGameObject(null);

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
        SaveLoad.Save(999);
      
        if(editingParty != null)
        {
            if(editingParty.ID != PartyManager.inst.currentParty){
            if(pTabs.ContainsKey(editingParty.ID))
            { pTabs[editingParty.ID].ChangeBorderColour(Color.black);}
            }
            if(pTabs.ContainsKey(editingParty.ID))
            { pTabs[editingParty.ID].Move( pTabs[editingParty.ID].ogPos);}


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


       
        HubCharacterDisplay.inst.RemoveListeners();
        inPartyTabs.Clear();
        editingParty = null;
       
        EventSystem.current.SetSelectedGameObject(null);
        editing = false;
        HubCharacterDisplay.inst.Refresh();
        ActivePartyRefresh();
        HubStateHandler.inst.ChangeStateString("Party");
        currentPartyRT.DOAnchorPosY(partyHidden,.15f);  
        HubStateHandler.inst.ChangeState(  HubStateHandler.HubState.ORGANIZER);
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
            MakePartyTab(item.Value);
          
           
         
        }
        }
        ActivePartyRefresh();
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

    public void ActivePartyRefresh()
    {
        if(PartyManager.inst.currentParty != string.Empty)
        {
            if(pTabs.ContainsKey(PartyManager.inst.currentParty))
            {
                foreach (var item in pTabs)
                {
                    item.Value.setActiveButton.gameObject.SetActive(true);
                    item.Value.starIcon.gameObject.SetActive(false);
                    item.Value.ChangeBorderColour(Color.black);
                }
                pTabs[PartyManager.inst.currentParty].setActiveButton.gameObject.SetActive(false);
                pTabs[PartyManager.inst.currentParty].starIcon.gameObject.SetActive(true);
                pTabs[PartyManager.inst.currentParty].transform.SetSiblingIndex(1);
            pTabs[PartyManager.inst.currentParty].ChangeBorderColour( pTabs[PartyManager.inst.currentParty].yellow);
            }
            else{
                Debug.LogAssertion("PARTY TABS DO NOT CONTAIN ID :" + PartyManager.inst.currentParty);
            }
        }
        else
        {
                Debug.LogWarning("NO CURRENT PARTY!");
        }
      

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