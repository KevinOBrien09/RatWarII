using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;


public class NodeOverrider : MonoBehaviour
{
    
    public Slot main,overridden;

    public LayerMask layerMask;
    public void OverrrideNode()
    {
        
        ResetOGNode();
        Vector3 d = transform.up;
        RaycastHit hit;
        if(Physics.Raycast(transform.position,d * 5,maxDistance: 5,hitInfo: out hit,layerMask:layerMask))  
        {Debug.Log(hit.collider.gameObject.name);
            Slot s = null;
           
            if(hit.collider.gameObject. TryGetComponent<Slot>(out s)){
              if(s.isBoat){
                Debug.Log("XDXDXDXDDXDXDXDXDDDDDDDDDDD");
            }
                overridden = s;
                overridden.gameObject.SetActive(false);
                Node n = overridden.node;
                n.slot = main;
                main.node = n;
                overridden.node = null;
                  if(main.cont.unit != null){
                overridden.cont.unit = main.cont.unit;
                  }
            }
        }
    }

    public void ResetOGNode()
    {
        if(overridden != null){
            overridden.gameObject.SetActive(true);
            Node n = main.node;
            n.slot = overridden;
            overridden.node = n;
            main.node = null;
            if(overridden.cont.unit != null){
                overridden.cont.unit = null;
            }
            overridden = null;  
            
        }
     
    
       
  
    }
}



