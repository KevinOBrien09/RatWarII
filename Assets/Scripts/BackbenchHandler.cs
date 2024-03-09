
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
    public TextMeshProUGUI capacityText;
    public List<CharacterCell> cells = new List<CharacterCell>();
    public List<CharacterTab> tabs = new List<CharacterTab>();

   
    void Start(){
       rt.DOAnchorPosX(hidden,0);  
       currentPartyRT.DOAnchorPosY(partyHidden,0).OnComplete(()=>{
       canvasGO.SetActive(false);
       });  
    }
    public void Show(){
        SpawnTabs(); 
        rt.DOAnchorPosX(shown,.15f);
        currentPartyRT.DOAnchorPosY(partyShown,.15f);  
    }

    void SpawnTabs(){
        int i = 0;
        foreach (var item in Party.inst.activeParty)
        {
            CharacterTab t =   Instantiate(tabPrefab,tabHolder);
            t.Init(item.Value.character);
            t.IsPartyMember(cells[item.Value.position]);
            tabs.Add(t);
            i++;
        }
        foreach (var item in Party.inst.benched)
        {
            CharacterTab t =   Instantiate(tabPrefab,tabHolder);
            t.Init(item.Value.character);
            tabs.Add(t);
        }
        
        capacityText.text = tabs.Count +"/12";
    }

    void KillTabs(){
        foreach (var item in tabs)
        {
            item.dragger.dragDropCell.draggable = null;
            Destroy(item.dragger.gameObject);
            Destroy(item.gameObject);
        }
        
        tabs.Clear();
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