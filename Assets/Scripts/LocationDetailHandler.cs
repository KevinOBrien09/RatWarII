using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;
public class LocationDetailHandler : Singleton<LocationDetailHandler>
{
    public RectTransform rt;
    public float shown,hidden;
    public Image locationPicture;
    public TextMeshProUGUI locationName,stage,description;
    public PartyTab partyTab;
    public SoloCharacterMapTab soloTab;
    public List<GameObject> tabs = new List<GameObject>();
    public List<LocationTab> locTabs = new List<LocationTab>();
    public List<PartyTab> partyTabs = new List<PartyTab>();
    public List<Button> options = new List<Button>();
    public RectTransform holder,locationHolder,locationTabParent;
    public Vector2 locationTabParentPos;
    public bool open;
    public LocationTab locationTabPrefab;
    public TextMeshProUGUI locationTabString;
    bool inLocationSelection;
    public Button backBut;
    public Party clickedInParty;
    Vector2 currentID ;
    void Start(){
    // string username = (System.Environment.UserName);
    // Debug.Log(username);
    currentID = new Vector2(-1,-1);
        rt.DOAnchorPosX(hidden,0).OnComplete(()=>{

        gameObject.SetActive(false);
        });
    }
    public void Show(LocationInfo i){
        if(!open){
        gameObject.SetActive(true);
        rt.DOAnchorPosX(hidden,0);
        rt.DOAnchorPosX(shown,.25f);
        open = true;
        }
        EditDetails(i);

      
    
    }

    public void VisitButton(){
        LocationManager.inst.Visit(currentID);
    }

    public void EditDetails(LocationInfo info)
    {
        currentID = info.stage.GetID();
        locationName.text = info.locationName;
        description.text = info.desc;

        foreach (var item in tabs)
        { Destroy(item.gameObject); }
        tabs.Clear();
        locTabs.Clear();
       partyTabs.Clear();
        locationTabString.text = "Nearby locations";
        if(info.locationPic != null)
        {
            locationPicture.sprite  = info.locationPic;
        }



        foreach (var item in options)
        {item.gameObject.SetActive(false);}

        if(LocationManager.inst.currentLocation != currentID )
        {options[0].gameObject.SetActive(true);}

        foreach (var item in LocationManager.inst.adjacentLocations(currentID))
        {
            LocationTab lt = Instantiate(locationTabPrefab,locationHolder);
            lt.Init(MapTileManager.inst.ld[item].locationInfo,currentID);
            tabs.Add(lt.gameObject);
            lt.interactable = false;
            locTabs.Add(lt);

        }

        clickedInParty = null;
        backBut.gameObject.SetActive(false);
        // if(LocationManager.inst.TileHasCharacterAdjacentToIt(currentID))
        // {options[1].gameObject.SetActive(true);}

//locationTabParent.DOAnchorPosX(locationTabParentPos.y,0);
        foreach (var item in PartyManager.inst.parties)
        {
            if(item.Value.mapTileID == currentID)
            {       
                PartyTab pt = Instantiate(partyTab,holder);
              
                pt.Init(item.Value);
                pt.SetToMapMode();
               
                pt.mainTabButton.onClick.AddListener(()=>{
                    EnterLocationSelection(pt);

                });
                tabs.Add(pt.gameObject);
                  partyTabs.Add(pt);
            }     
        }

        foreach (var item in PartyManager.inst.benched)
        {
            if(item.Value.mapTileID == currentID)
            {    
                SoloCharacterMapTab scmt = Instantiate(soloTab,holder);
                scmt.Init(item.Value);
                tabs.Add(scmt.gameObject);
            }
        }

    }

    public void BackButton(){
        clickedInParty = null;
        EventSystem.current.SetSelectedGameObject(null);
        backBut.gameObject.SetActive(false);
        foreach (var item in locTabs)
        {
            item.interactable = false;
        }
        locationTabString.text = "Nearby locations";
        inLocationSelection = false;
        foreach (var item in partyTabs)
        {item.ChangeBorderColour(Color.black); 
        item.mainTabButton.interactable = true;}
    }

    public void EnterLocationSelection(PartyTab pt){
        // locationTabParent.DOAnchorPosX(locationTabParentPos.y,0);
        // locationTabParent.DOAnchorPosX(locationTabParentPos.x,.25f);
        clickedInParty = pt.party;
        //pt.mainTabButton.onClick.RemoveAllListeners();
        EventSystem.current.SetSelectedGameObject(null);
        locationTabString.text = pt.party.partyName + " will travel to...";
        foreach (var item in locTabs)
        {
            item.interactable = true;
        }
        foreach (var item in partyTabs)
        {item.mainTabButton.interactable = false;
            
        }
        inLocationSelection = true;
        backBut.gameObject.SetActive(true);
        pt.ChangeBorderColour(pt.yellow);
        
    }



    public void Hide(){
        currentID = new Vector2(-1,-1);
        rt.DOAnchorPosX(hidden,.25f).OnComplete(()=>{
        open = false;
            gameObject.SetActive(false);
           });
    }
}