
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEngine.UI;
using DG.Tweening;
public class HubCharacterDisplay : Singleton<HubCharacterDisplay>
{
    UnityAction a;
    public List<Transform> holders;
    public GenericDictionary<CharacterCell,Transform> holderDict = new GenericDictionary<CharacterCell, Transform>();
    public List<CharacterGraphic> graphics = new List<CharacterGraphic>();
    void Start()
    {
        a = ()=>{Refresh();};
        Party.inst.onPartyEdit.AddListener(a);
    }
    public void Refresh()
    {
        foreach (var item in graphics)
        {
            Destroy(item.gameObject);
        }
        graphics.Clear();
        int i =0;
        foreach (var item in Party.inst.activeParty)
        {
           CharacterGraphic cg = CharacterBuilder.inst.GenerateGraphic(item,false);
           graphics.Add(cg);
           cg.transform.localScale = new Vector3(.2f,.2f,.2f);
           cg.transform.SetParent(holders[i]);
           cg.transform.localPosition = Vector3.zero;

            i++;
        }
        if(WorldCity.inst.currentState == WorldCity.CityState.ORGANIZER)
        {
            foreach (var item in graphics)
            {
                item.transform.SetParent(null);
                foreach (var p in holderDict)
                {
                    if(p.Key.character == item.character && p.Key.currentItem != null){
                        item.transform.SetParent(p.Value);    item.transform.localPosition = Vector3.zero;
                    }
                
                    
                }
            }
           
            Debug.Log("Refresh");
        }
    
    }

    void OnDestroy(){
        Party.inst.onPartyEdit.RemoveListener(a);
    }
}