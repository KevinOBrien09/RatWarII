using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;

public class ActionMenuFormation : MonoBehaviour
{
    public GenericDictionary<ActionMenuState,RectTransform> icons = new GenericDictionary<ActionMenuState, RectTransform>();
    public ActionMenu.Formation formation;

    public virtual void Activate()
    {Debug.Log("Activated:" + formation.ToString());}
    
    public virtual void Deactivate()
    {Debug.Log("Deactivated:" + formation.ToString());}

    public virtual void MoveLeft()
    {Debug.Log(formation.ToString() + " Left");}

    public virtual void MoveRight()
    {Debug.Log(formation.ToString()+ " Right");}

    public virtual void ChangeState(ActionMenuState newState)
    {Debug.Log("new state is " + newState);}

    public virtual void Reset()
    {Debug.Log("Reset Formation");}

    public virtual void HideIcons()
    {
        ActionMenu.inst.center.gameObject.SetActive(false);
        foreach (var item in icons)
        {item.Value.gameObject.SetActive(false);}
    }

    public virtual void ShowIcons()
    {
        ActionMenu.inst.center.gameObject.SetActive(true);
        foreach (var item in icons)
        {item.Value.gameObject.SetActive(true);}
    }
}