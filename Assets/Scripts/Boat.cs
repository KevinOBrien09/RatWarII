using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;


public class Boat : Unit
{
    public List<Slot> slots = new List<Slot>();
    public void Spawn(List<Slot> s)
    {
        slots.Clear();
        slots = new List<Slot>(s);
    }

    public override bool isEntity()
    {
        return false;
    }
}