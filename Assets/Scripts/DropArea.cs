using System;
using System.Collections.Generic;
using UnityEngine;

public class DropArea : MonoBehaviour
{
    public virtual bool Accepts(DraggableComponent draggable)
	{
		return true;
	}

	public virtual void Drop(DraggableComponent draggable)
	{
		
	}
}