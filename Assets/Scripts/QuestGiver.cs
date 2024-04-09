using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

using DG.Tweening;
public class QuestGiver: Singleton<QuestGiver>
{
   
    public Transform camPos;
    public GameObject canvas;
    public UnityEvent shut;
    public void Open(){
        StartCoroutine(q());
        IEnumerator q()
        {
            yield return new WaitForSeconds(.1f);
            WorldHubCamera.inst.fuckOff = false;
            canvas.SetActive(true);
            HubStateHandler.inst.ChangeStateString("Quests");
            HubStateHandler.inst.ChangeState(HubStateHandler.HubState. QUEST);
            HubStateHandler.inst.close = shut;
        }
     
    }

    public void Go(){
        if( !WorldHubCamera.inst.fuckOff)
        {  
            CharacterRecruiter.inst.state = 3;
            
            WorldHubCamera.inst.fuckOff = true;
            WorldHubCamera.inst.cam.DOFieldOfView(40,.2f);
            WorldHubCamera.inst.Move(camPos,()=>
            { Open(); });
          
        }
    }

    public void Close(){
 canvas.SetActive(false);
    }
}