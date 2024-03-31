using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using DG.Tweening;
using UnityEngine.UI;
public class WorldLeaveAreaDoor : WorldLeaveArea
{
    public Transform leftDoor,rightDoor;
    public override void Go()
    {
        HubStateHandler.inst.close = shut;
         leftDoor.DORotate(new Vector3(0,-60,0),.3f );
            rightDoor.DORotate(new Vector3(0,60,0),.3f ).OnComplete(()=>{
               Leave((()=>{
  Reset();
               }));
                    
            });
     
    }

  

    public  void Reset(){
      
        leftDoor.DORotate(new Vector3(0,0,0),0f );
        rightDoor.DORotate(new Vector3(0,0,0),0f );
    }

    public  void Close(){
     
        leftDoor.DORotate(new Vector3(0,0,0),1f );
        rightDoor.DORotate(new Vector3(0,0,0),1f );
    }
}