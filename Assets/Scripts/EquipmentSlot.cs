using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class EquipmentSlot : DropArea
{
	
	public DraggableComponent currentItem = null;
	public Transform holder;
	public bool HasItem = false;
	
	public void Initialize(DraggableComponent currentItem)
	{
		if (currentItem == null)
		{
			Debug.LogError("Tried to initialize the slot with an null item!");
			return;
		}

		Drop(currentItem);
	}

	public override bool Accepts(DraggableComponent draggable)
	{
		if(draggable.Accept(this))
		{
			return currentItem == null;
		}
		else{
			return false;
		}
		
		
	}

	public override void Drop(DraggableComponent draggable)
	{
		var draggableTransform = draggable.transform;
		draggableTransform.SetParent(holder);
		draggableTransform.localPosition = Vector3.zero;
		draggable.currentSlot.currentItem = null;	
		currentItem = draggable;
		draggable.currentSlot = this;
	
	
		HasItem = true;
	}

	
	private void CurrentItemEndDragHandler(PointerEventData eventData, bool dropped)
	{
		

		if (!dropped)
		{
			return;
		}

	
	
		currentItem = null; //We no longer have an item in this slot, so we remove the refference
		HasItem = false;
	}
}