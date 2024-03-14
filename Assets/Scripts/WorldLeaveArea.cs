using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using DG.Tweening;
using UnityEngine.UI;
public class WorldLeaveArea : MonoBehaviour
{
    public UnityEvent shut;
    public Transform leftDoor,rightDoor;
    public CanvasGroup cg;
    public Image blackFade;
    void Start(){
        cg.gameObject.SetActive(false);
        cg.alpha = 0;
    }
    public void Go()
    {
        // HubStateHandler.inst.ChangeState(HubStateHandler.HubState.LEAVE);
        // HubStateHandler.inst.ChangeStateString("Leave");
       
        HubStateHandler.inst.close = shut;
         leftDoor.DORotate(new Vector3(0,-60,0),.3f );
            rightDoor.DORotate(new Vector3(0,60,0),.3f ).OnComplete(()=>{
                BlackFade.inst.FadeInEvent(()=>{
                   HubManager.inst.HubToMap();
                    WorldHubCamera.inst.fuckOff = false;
                    });
            });
        // cg.alpha = 0;
        // cg.gameObject.SetActive(true);
        // cg.DOFade(1,.1f);
    }

   public void AcceptPrompt()
    {
        cg.DOFade(0,.1f).OnComplete(()=>{

            leftDoor.DORotate(new Vector3(0,-60,0),.3f );
            rightDoor.DORotate(new Vector3(0,60,0),.3f ).OnComplete(()=>{
                BlackFade.inst.FadeInEvent(()=>{
                   HubManager.inst.HubToMap();
                    });
            });
        });
    }

    public void Reset(){
         cg.DOFade(0,0).OnComplete(()=>{

            cg.gameObject.SetActive(false);
        });
        leftDoor.DORotate(new Vector3(0,0,0),0f );
        rightDoor.DORotate(new Vector3(0,0,0),0f );
    }

    public void Close(){
        cg.DOFade(0,.1f).OnComplete(()=>{

            cg.gameObject.SetActive(false);
        });
        leftDoor.DORotate(new Vector3(0,0,0),1f );
        rightDoor.DORotate(new Vector3(0,0,0),1f );
    }
}