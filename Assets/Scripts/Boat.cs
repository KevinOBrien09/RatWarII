 using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;


public class Boat : Unit
{
    //public List<Island> accessibleIslands = new List<Island>();
    public GenericDictionary<Slot,NodeOverrider> poopDeck = new GenericDictionary<Slot,NodeOverrider>();
    public Transform flag;
    void Start()
    {
   
        foreach (var t in poopDeck)
        {
            MapManager.inst.allSlots.Add(t.Key);
        }
    }
    public void Dock()
    {
        Depart();
     
        OverrideNodes();
    
    }

    public void OverrideNodes(){
        foreach (var item in poopDeck)
        {
            item.Value.ResetOGNode();
            item.Value.OverrrideNode();
            
        }
    }

    public void Depart(){
        foreach (var item in poopDeck)
        {
            item.Value.ResetOGNode();
            
        }
    }

    

  
    public List<BoatSlot>Grab(int radius)
    {
        List<BoatSlot> candidateSlots = new List<BoatSlot>();
        List<BoatSlot> validSlots = new List<BoatSlot>();
        BoatSlot bs = slot as BoatSlot;
        Collider[] c = Physics.OverlapSphere(bs.transform.position,radius*5);
        foreach(var item in c)
        {
            BoatSlot s = null;
            bool v = item.TryGetComponent<BoatSlot>(out s);
            if(v)  
            {candidateSlots.Add(s);}
        }

        List<BoatSlot> ss = new List<BoatSlot>(bs.FilterUnadjacentsBOAT(candidateSlots,new List<BoatSlot>(),true));
        
        if(ss.Contains(bs))
        {ss.Remove(bs);}
      
        return ss;
    }
    
   
    public override bool isEntity()
    {
        return false;
    }

    public override void Flip(Vector3 v)
    {
        if( transform.position.x != v.x)
        {
            bool movingRight = transform.position.x <= v.x ;
            if(movingRight)
            {
              flag.transform.localScale = Vector3.one;
            }
            else
            { 
                 flag.transform.localScale = new Vector3(-1,1,1);
            }
        }
    }

}