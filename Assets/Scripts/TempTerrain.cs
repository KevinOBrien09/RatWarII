using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.Events;
public class TempTerrain : SpecialSlot
{
    public int turnToDieOn,howManyTurns;
    public Collider col;
    public UnityAction onKill;

    public void Kill(){
       onKill.Invoke();
        
        
    }
}