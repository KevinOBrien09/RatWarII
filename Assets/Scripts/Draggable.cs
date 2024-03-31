using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections.Generic;
public class Draggable : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public static bool isDragging;
    private Vector3 startPosition;
    public Transform startParent;
    public Transform priorityTransform;
    public DragDropCell dragDropCell;
    RectTransform rt;
    Canvas canvas;
    void Start(){
        isDragging = false;
        rt = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
        priorityTransform = GameObject.Find("PriorityTransform").transform;
        startParent = transform.parent;
    }

    public virtual void OnBeginDrag(PointerEventData eventData)
    {
        transform.SetParent(priorityTransform);
        isDragging = true;
    }

    public virtual void OnDrag(PointerEventData eventData)
    {
     rt.anchoredPosition += eventData.delta / canvas.scaleFactor;
    }

    public virtual void OnEndDrag(PointerEventData eventData)
    {  
        isDragging = false;
        var results = new List<RaycastResult>();
		EventSystem.current.RaycastAll(eventData, results);
        DragEnd(results);
    }

    public virtual void DragEnd( List<RaycastResult> results)
    {
        DragDropCell cell = null;
	    foreach (var result in results)
		{
            cell = result.gameObject.GetComponent<DragDropCell>();
            if (cell != null)
			{
                int i = cell.canTake(this);
                if(i == 1)
                {cell.Take(this);}
                else if(i ==2)
                {Swap(cell);}
                else if(i == 0)
                {Snap();}
                break;
			}
        }

        if(cell == null)
        { NoCell(); }
    }

    public virtual void Swap(DragDropCell sittingCell)
    {
        Draggable a = sittingCell.draggable;
        Draggable b = this;
        DragDropCell cellA = sittingCell;
        DragDropCell cellB = b.dragDropCell;
        cellA.draggable = b;
        b.dragDropCell = cellA;
        b.startParent = cellA.holder;

        cellB.draggable = a;
        a.dragDropCell = cellB;
        a.startParent = cellB.holder;
        
        a.Snap();
        b.Snap();
    }

    public virtual void NoCell()
    { Snap();  }

    public virtual void Snap()
    {
        transform.SetParent(startParent);
        transform.localPosition = Vector3.zero;
    }

    
}