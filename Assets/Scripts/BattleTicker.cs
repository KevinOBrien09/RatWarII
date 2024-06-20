using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;
using TMPro;

public class BattleTicker : Singleton<BattleTicker>
{
    public TMP_Typewriter typewriter;
    public GameObject parent;
    void Start()
    {Wipe();
    parent.SetActive(false);}

    public void Type(string s)
    {
        if(typewriter.m_textUI.text != s)
        {typewriter.Play(s,90,(()=>{}));}
    }

    public void Wipe()
    {typewriter.m_textUI.text = "";}

    
}