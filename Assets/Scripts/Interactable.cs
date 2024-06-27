
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;

public class Interactable : MonoBehaviour,IPointerEnterHandler,IPointerExitHandler
{
    public bool canInteract;
    public bool inRange;
    public GameObject outline;
    float distToPlayer;
  

    public void OnPointerEnter(PointerEventData eventData)
    {
      InteractionManager.inst.SetInteractable(this);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        InteractionManager.inst.RemoveInteractable();
    }

    public virtual void Go(){
        Debug.Log("Go");
    }

    public float GetDistanceToPlayer()
    {
        return Vector3.Distance(transform.position,PartyController.inst.leader .transform.position);
    }

    public virtual void OutlineToggle(bool state,bool inRange = false)
    {
       
    }

    // public void OnPointerClick(PointerEventData eventData)
    // {
    //    Debug.Log("Pointer click :" + gameObject.name);
    // }



}