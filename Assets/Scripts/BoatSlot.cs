using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class BoatSlot : Slot
{
    public BoxCollider bc;
    public bool Valid(){
        
        Vector3 v =bc.size;
        Collider[] hit =  Physics.OverlapSphere(bc.transform.position,1.25f);
        Debug.Log(hit.Length);
        foreach (var item in hit)
        {
            Slot s = null;
            if(item.gameObject.TryGetComponent<Slot>(out s)) {
                if(!s.isWater){
                    return false;
                }
            }
        }

        return true;
    }

    // public void OnDrawGizmos(){
    //     Gizmos.color = Color.magenta;
    //     Gizmos.DrawWireSphere(bc.transform.position,1.25f);
    // }
}