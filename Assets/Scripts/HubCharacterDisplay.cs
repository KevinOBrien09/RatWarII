
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEngine.UI;
using System.Linq;
using DG.Tweening;
using System;
public class HubCharacterDisplay : Singleton<HubCharacterDisplay>
{
   
    public List<Transform> holders;
    public List<CharacterGraphic> graphics = new List<CharacterGraphic>();
    public TextMeshProUGUI fandfLogo;
    void Start()
    {
        BlackFade.inst.fade.DOFade(1,0);
        fandfLogo.gameObject.SetActive(true);
        fandfLogo.DOFade(1,0);
        fandfLogo.DOFade(0,1f).OnComplete(()=>{
            fandfLogo.gameObject.SetActive(false);
        });
        BlackFade.inst.FadeInEvent(()=>{


      
        if(GameManager.inst. loadFromFile){
            Refresh();
        }
        BlackFade.inst.FadeOut();
        });
       
    }

    public void AddListener(Party p)
    {
        p.onPartyEdit += Refresh;
    }
    
    public void RemoveListeners()
    { 
        foreach (var item in PartyManager.inst.parties)
        {
            item.Value.onPartyEdit -= Refresh;
        }
    }

    public void Refresh()
    {
        foreach (var item in graphics)
        {
            Destroy(item.gameObject);
        }
        graphics.Clear();
        Party p = null;
        if(BackbenchHandler.inst.editing)
        {
            p = BackbenchHandler.inst.editingParty;
        }
        else
        {
            if(PartyManager.inst.parties.ContainsKey(PartyManager.inst.currentParty)){
p = PartyManager.inst.parties[PartyManager.inst.currentParty];
            }
            else{
              Debug.LogWarning("NO CURRENT PARTY!");
            }
            
        }
        
          
        if(p!= null)
        {
            foreach (var item in p. members)
            {
                CharacterGraphic cg = CharacterBuilder.inst.GenerateGraphic(item.Value.character);
                cg.KillCamera();
                graphics.Add(cg);
                cg.transform.localScale = new Vector3(.2f,.2f,.2f);
                cg.transform.SetParent(holders[item.Value.position]);
                cg.transform.localPosition = Vector3.zero;
            }
        
        }
        else{ Debug.LogWarning("No party found!");  }
         
        
      
    }
    
    void OnDestroy(){
     RemoveListeners();
    }
}