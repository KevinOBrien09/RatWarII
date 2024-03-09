using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections.Generic;
public class DragDropCell : MonoBehaviour
{
    public Draggable draggable;
    public Transform holder;

    public virtual void Start()
    {
        if(holder == null)
        {holder = transform;}
    }

    public virtual int canTake(Draggable d)
    {
        if(draggable == null)
        { return 1; }
        else
        { return 0; }
    }

    public virtual void Take(Draggable d)
    {
        d.dragDropCell.draggable = null;
        d.dragDropCell = null;
        draggable = d;
        d.dragDropCell = this;
        d.startParent = holder;
        //transform.SetParent(  d.startParent);
        d.Snap();
    }

}