using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactable : TempTerrain
{
  
    public void Init(Slot s)
    {
        slot = s;
        s.interactable = this;
        s.node.isBlocked = true;
    }
    public virtual void Go(Unit caster)
    {
        Debug.Log(gameObject.name);
    }
}
