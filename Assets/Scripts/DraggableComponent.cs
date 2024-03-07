using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DraggableComponent : MonoBehaviour, IInitializePotentialDragHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
{
	public static bool isDragging;
	public bool FollowCursor { get; set; } = true;
	public Vector3 StartPosition;
	public bool CanDrag { get; set; } = true;
	public EquipmentSlot currentSlot;
	private RectTransform rectTransform;
	private Canvas mcanvas;

	public virtual void Awake()
	{
		isDragging = false;
		rectTransform = GetComponent<RectTransform>();
		mcanvas = GetComponentInParent<Canvas>();
	}

   
   public virtual bool Accept(EquipmentSlot slot)
    {
return true;
    }
	public virtual void OnBeginDrag(PointerEventData eventData)
	{
		if (!CanDrag)
		{
			return;
		}

		isDragging = true;
	}

	public virtual  void OnDrag(PointerEventData eventData)
	{
		if (!CanDrag)
		{
			return;
		}



		if (FollowCursor)
		{
			rectTransform.anchoredPosition += eventData.delta / mcanvas.scaleFactor;
		}
	}

	public virtual  void OnEndDrag(PointerEventData eventData)
	{
		if (!CanDrag)
		{
			return;
		}
		isDragging = false;
		var results = new List<RaycastResult>();
		EventSystem.current.RaycastAll(eventData, results);

		DropArea dropArea = null;

		foreach (var result in results)
		{
			dropArea = result.gameObject.GetComponent<DropArea>();

			if (dropArea != null)
			{
				break;
			}
		}

		if (dropArea != null)
		{
			if (dropArea.Accepts(this))
			{
				dropArea.Drop(this);
		
				return;
			}
		}
        Snap();
	
	}

	public virtual void Snap(){
		Debug.Log("Snap back");
		rectTransform.anchoredPosition = StartPosition;
	}

	public virtual  void OnInitializePotentialDrag(PointerEventData eventData)
	{
		StartPosition = rectTransform.anchoredPosition;
	}
}