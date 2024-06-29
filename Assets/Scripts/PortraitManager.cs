using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using UnityEngine.Events;
using DG.Tweening;
using UnityEngine.SceneManagement;

public class PortraitManager : Singleton<PortraitManager>
{
    public Portrait portraitPrefab;
    public Transform holder;
    public GenericDictionary<string,Portrait> dict  = new GenericDictionary<string, Portrait>();
    public VerticalLayoutGroup verticalLayoutGroup;
    public void CreatePortrait(Unit u)
    {
        Portrait p = Instantiate(portraitPrefab,holder);    
        p.Init(u);
        dict.Add(u.character.ID,p);
    }

    public void Kill(Unit u){
        Portrait p = dict[u.character.ID] ;
        dict.Remove(u.character.ID);
        Destroy(p.gameObject);
    }
    public void toggleLayout(bool state){
        verticalLayoutGroup.enabled = state;
    }
}