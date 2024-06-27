using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using DG.Tweening;
// using TMPro;
//Barrier Lickwounds Bulken  SelfHeal
// Bloody Reave   Strike
public class Zoomer : MonoBehaviour
{
    public CanvasGroup group;
    public Vector3 offset = new Vector3(0,-5.15f,2.5f);
    public virtual void Go(CastArgs args,UnityAction action){
    
    }

    public virtual void AttachToBattle(){
        transform.SetParent(null);
        transform.SetParent(CamFollow.inst.transform);
        transform.localPosition = offset;
    }

    public virtual void AttachToOverworld(){
        transform.SetParent(null);
        transform.SetParent(OverworldCamera.inst.transform);
        transform.localPosition = offset;
    }

    public virtual void OddballAttach(){

    }

}