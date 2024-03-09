
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEngine.UI;
using System.Linq;
using DG.Tweening;
public class HubCharacterDisplay : Singleton<HubCharacterDisplay>
{
    UnityAction a;
    public List<Transform> holders;
    public List<CharacterGraphic> graphics = new List<CharacterGraphic>();
    void Start()
    {
        a = ()=>{Refresh();};
        Party.inst.onPartyEdit.AddListener(a);
        if(GameManager.inst. loadFromFile){
            Refresh();
        }

        
    }
    public void Refresh()
    {
        foreach (var item in graphics)
        {
            Destroy(item.gameObject);
        }
        graphics.Clear();
        
        foreach (var item in Party.inst.activeParty)
        {
            CharacterGraphic cg = CharacterBuilder.inst.GenerateGraphic(item.Value.character);
            cg.KillCamera();
            graphics.Add(cg);
            cg.transform.localScale = new Vector3(.2f,.2f,.2f);
            cg.transform.SetParent(holders[item.Value.position]);
            cg.transform.localPosition = Vector3.zero;
        }
    }
    
    void OnDestroy(){
        Party.inst.onPartyEdit.RemoveListener(a);
    }
}