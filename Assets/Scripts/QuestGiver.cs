using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class QuestGiver: Singleton<QuestGiver>
{
   
    public GameObject canvas;
    public UnityEvent shut;
    public void Open(){
        StartCoroutine(q());
        IEnumerator q()
        {
            yield return new WaitForSeconds(.1f);
            WorldHubCamera.inst.fuckOff = false;
            canvas.SetActive(true);
            
            WorldCity.inst.ChangeState(WorldCity.CityState.QUEST);
            WorldCity.inst.close = shut;
        }
     
    }

    public void Close(){
 canvas.SetActive(false);
    }
}