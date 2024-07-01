
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using DG.Tweening;

public class LeverInteractable : KeyPickup{
    public Transform leverOffset;
    public Vector3 leverDownRot,leverUpRot;
    public bool down,canChange;
    public UnityEvent downAction;
    public UnityEvent upAction;
    public GameObject wall;
    void Start()
    {
        leverUpRot =  leverOffset.transform.rotation.eulerAngles;

    }    

    public override void Go()
    {
        
        if(down){
            leverOffset.transform.rotation = Quaternion.Euler(leverUpRot.x,leverUpRot.y,leverUpRot.z);
            upAction.Invoke();
            down = false;
        }
        else{
            leverOffset.transform.rotation = Quaternion.Euler(leverDownRot.x,leverDownRot.y,leverDownRot.z);
            downAction.Invoke();
            down = true;
            enabled = false;
            itemContainer.itemsGone = true;
            ol.enabled = false;
        }
    }

    public void XD(){
      Destroy(wall);
    }
}