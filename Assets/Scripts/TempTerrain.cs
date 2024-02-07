using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class TempTerrain : MonoBehaviour
{
    public Slot slot;
    public int turnToDieOn,howManyTurns;
    public Collider col;

    public void Kill(){
        Destroy(col.gameObject);
        slot.tempTerrain = null;
        MapManager.inst.grid.UpdateGrid();
        gameObject.transform.DOMoveY(-4,.25f).OnComplete(()=>{
            Destroy(gameObject);
        });
        
    }
}