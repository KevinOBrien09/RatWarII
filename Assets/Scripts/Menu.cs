using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;
public enum State{ITEM,PARTY,OPTIONS}
public class Menu : Singleton<Menu>
{
    public GenericDictionary<State,GameObject> dict = new GenericDictionary<State, GameObject>();
    public bool open;
    public GameObject holder;
    public State currentState;
    public MenuPartyTab partyTabPrefab;
    public RectTransform partyTabHolder;
    public List<MenuPartyTab> partyTabs = new List<MenuPartyTab>();
    public MenuStatScreen menuStatScreen;
    public float partyTabOG;
    public BattlePositionEditor battlePositionEditor;
    public TextMeshProUGUI partyName;
    public GameObject otherShit;
    public InventoryViewer inventoryViewer;
    void Start()
    {
        partyTabOG = partyTabHolder.anchoredPosition.x;
        holder.SetActive(false);
        open = false;
        SwapToItemState();
        if(PartyManager.inst.currentParty != string.Empty){
            battlePositionEditor.party = PartyManager.inst.parties[PartyManager.inst.currentParty];
        }
        partyName.text = "Party:" + battlePositionEditor.party.partyName;
       
    }

    public void SwapToItemState()
    {
        
        currentState = State.ITEM;
        inventoryViewer.Load();
        Apply();
    }

    public void SwapToPartyState()
    {
        currentState = State.PARTY;
         inventoryViewer.Exit();
        menuStatScreen.gameObject.SetActive(false);
        foreach (var item in partyTabs)
        {Destroy(item.gameObject);}
        partyTabs.Clear();
        foreach (var item in BattleManager.inst.playerUnits)
        {
            MenuPartyTab mpt = Instantiate(partyTabPrefab,partyTabHolder);
            mpt.Init(item);
            partyTabs.Add(mpt);
            
        }
        // if(GameManager.inst.loadFromFile){
            battlePositionEditor.Reset();
            battlePositionEditor.party = PartyManager.inst.parties[PartyManager.inst.currentParty];
        
            battlePositionEditor.SpawnDraggers();
      //  }
       
        Apply();
    }

    public void SwapToOptionsState()
    {
       
        currentState = State.OPTIONS;
         inventoryViewer.Exit();
        Apply();
    }

    public void Apply()
    {
        foreach (var item in dict)
        {item.Value.SetActive(false);}

        dict[currentState].SetActive(true);
    }
    
    void Update()
    {
        if(InputManager.inst.player.GetButtonDown("Menu"))
        {
           Toggle();
        }
    }

    public void Toggle(){
        if(open)
        {
            open = false;  
            inventoryViewer.Exit();
           
            holder.SetActive(false);
        }
        else
        {
            open = true;   
            if(currentState == State.ITEM){
                inventoryViewer.BuildAll();
            }
            holder.SetActive(true);
        }
    }

    public void ShowStatScreen(Unit u){
        partyTabHolder.DOAnchorPosX(-875,.2f);
        menuStatScreen.Load(u);
          otherShit.SetActive(false);
        //-875
    }   

    public void HideStateScreen(){
        otherShit.SetActive(true);
        partyTabHolder.DOAnchorPosX(partyTabOG,.2f);
        menuStatScreen.gameObject.SetActive(false);
    }
}
