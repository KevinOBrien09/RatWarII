using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using DG.Tweening;
using UnityEngine.UI;
public class WorldLeaveArea : MonoBehaviour
{
    public UnityEvent shut;
 
  
  
    public virtual void Go()
    {
       Leave();
    }

    public virtual void Leave(UnityAction a = null){
        BlackFade.inst.FadeInEvent(()=>{
        HubManager.inst.HubToMap();
        WorldHubCamera.inst.fuckOff = false;
       if(a!= null){
        a.Invoke();
       }
        });
    }


  
}