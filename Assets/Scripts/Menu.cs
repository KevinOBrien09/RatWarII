using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
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
    void Start()
    {
        partyTabOG = partyTabHolder.anchoredPosition.x;
        holder.SetActive(false);
        open = false;
        SwapToItemState();
    }

    public void SwapToItemState()
    {
        currentState = State.ITEM;
        Apply();
    }

    public void SwapToPartyState()
    {
        currentState = State.PARTY;
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
        Apply();
    }

    public void SwapToOptionsState()
    {
        currentState = State.OPTIONS;
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
            holder.SetActive(false);
        }
        else
        {
            open = true;  
            holder.SetActive(true);
        }
    }

    public void ShowStatScreen(Unit u){
        partyTabHolder.DOAnchorPosX(-875,.2f);
        menuStatScreen.Load(u);
        //-875
    }   

    public void HideStateScreen(){
        partyTabHolder.DOAnchorPosX(partyTabOG,.2f);
        menuStatScreen.gameObject.SetActive(false);
    }
}
