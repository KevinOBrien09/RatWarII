using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class LoadDungeon : MonoBehaviour,IPointerEnterHandler
{ 
    public SoundData enter,error;
    public Objective.ObjectiveEnum q;

    public void Click()
    {
        if(PartyManager .inst.parties[PartyManager .inst.currentParty]. members.Count ==0){
            Debug.Log("No party members");
            AudioManager.inst.GetSoundEffect().Play(error); 
        }
        else{
            GameManager.inst.LoadQuest(q);
            SceneManager.LoadScene("Arena");
        }
        
    }

    public void OnPointerEnter(PointerEventData eventData)
    {  AudioManager.inst.GetSoundEffect().Play(enter); }
}