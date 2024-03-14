
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEngine.UI;
using DG.Tweening;
public class PartyOrganizer : MonoBehaviour
{
    public UnityEvent shut;
   
    public GameObject canvasGO;

    
    public void Open(){
        canvasGO.SetActive(true);
        StartCoroutine(q());
        IEnumerator q()
        {  BackbenchHandler.inst .Show();
            yield return new WaitForSeconds(.1f);
            WorldHubCamera.inst.fuckOff = false;
          
                    HubStateHandler.inst.ChangeStateString("Party");
            HubStateHandler.inst.ChangeState(  HubStateHandler.HubState.ORGANIZER);
            HubStateHandler.inst.close = shut;
        }
     
    }

    

    public void Close(){
       BackbenchHandler.inst .Hide();
    }

}