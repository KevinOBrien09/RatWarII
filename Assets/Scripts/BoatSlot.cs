using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.EventSystems;
public class BoatSlot : Slot
{
    public GameObject test;
    
    public List<Transform> rayShooters = new List<Transform>();
    public SpriteRenderer indicator;
    
    void Start(){
        indicator.color = normalColour;
       
    }

    public void CheckIfCostal()
    {
       
        List<Slot> neighbours = GetNeighbouringSlots();
      
        foreach (var item in neighbours)
        {
            if(!item.isWater)
            {
              //  test.gameObject.SetActive(true);
                Island island = item.room as Island;
                if(island != null)
                {
                    if(!island.coastalBoatSlots.Contains(this)){
                        island.coastalBoatSlots.Add(this);
                    }
                 
                   // Debug.Log("Detecting islands!");
                }
                break;
            }
        }
    }

    public List<Slot> GetNeighbouringSlots()
    { 
        List<Slot> neighbours = new List<Slot>();
        foreach (var item in rayShooters)
        {
            Transform r = item;
            List<Vector3> directions = new List<Vector3>();
            directions.Add(r.forward);
            directions.Add(-r.forward);
            directions.Add(r.right);
            directions.Add(-r.right);

            foreach (var d in directions)
            {
                RaycastHit hit ;
                if(Physics.Raycast(r.position,d * 5,maxDistance: 5,hitInfo: out hit))  
                {
                    Slot s = null;
                    if(hit.collider.gameObject !=null)
                    {
                        if(hit.collider.gameObject.TryGetComponent<Slot>(out s))
                        {
                            if(!neighbours.Contains(s))
                            {neighbours.Add(s); }
                           
                        }
                    }
                }
            }
        }
  
      
        return neighbours;
    }

    
 public List<BoatSlot> GetNeighbouringBoatSlots()
    { 
        List<BoatSlot> neighbours = new List<BoatSlot>();
        
        Transform r = rayShooter;
        List<Vector3> directions = new List<Vector3>();
        directions.Add(r.forward);
        directions.Add(-r.forward);
        directions.Add(r.right);
        directions.Add(-r.right);

        foreach (var d in directions)
        {
            RaycastHit hit ;
            if(Physics.Raycast(r.position,d * 10,maxDistance: 10,hitInfo: out hit))  
            {
                BoatSlot s = null;
                if(hit.collider.gameObject !=null)
                {
                    if(hit.collider.gameObject.TryGetComponent<BoatSlot>(out s))
                    {
                        if(!neighbours.Contains(s))
                        {neighbours.Add(s); }
                        
                    }
                }
            }
        }
    
        return neighbours;
    }

   

    public List<BoatSlot> FilterUnadjacentsBOAT(List<BoatSlot> candidates,List<BoatSlot> slots,bool skill)
    {
        List<BoatSlot>  XD =   GetNeighbouringBoatSlots();
        foreach (var item in XD)
        {
            if(candidates.Contains(item))
            {
                if(!slots.Contains(item))
                {
                    if(skill)
                    {
                        slots.Add(item);
                        item.FilterUnadjacentsBOAT(candidates,slots,skill);
                    }
                    else
                    {
                        if(item.cont.walkable())
                        {
                            slots.Add(item);
                            item.FilterUnadjacentsBOAT(candidates,slots,skill);
                        }
                    }
                }
            }
        }
        return slots;

     
    }

    public void Toggle(bool state){
        indicator.gameObject.SetActive(state);
    }
 




    public override void OnPointerClick(PointerEventData eventData)
    {
        if(cont.invisible)
        { return; }
        if(SkillAimer.inst.castDecided)
        {return;}
        // if(!MapManager.inst.slotBelongsToGrid(this)){
        //     return;
        // }

        if(GameManager.inst.checkGameState(GameState.PLAYERSELECT)  && !UnitMover.inst.inCoro)
        {
            if(UnitMover.inst.boatMover.validSlots.Contains(this))
            {
                UnitMover.inst.boatMover.InitializeMove(this); 
                
            //    DisableHover();
            }
        }
    }

    public override void OnPointerEnter(PointerEventData eventData)
    {
        if(GameManager.inst.checkGameState(GameState.PLAYERSELECT))
        {
            if(UnitMover.inst.boatMover.validSlots.Contains(this)){
                indicator.color = moveColour;
                List<Node> path =UnitMover.inst.boatMover.swampGeneratorBrain .waterGrid .aStar.FindPath(UnitMover.inst.boatMover.boat.slot.node,node);
                UnitMover.inst.NewHover(path);
           
            }
        }
    }

    public override void OnPointerExit(PointerEventData eventData)
    {
        indicator.color = normalColour;
    }
    // public void OnDrawGizmos(){
    //     Gizmos.color = Color.magenta;
    //     Gizmos.DrawWireSphere(bc.transform.position,1.25f);
    // }
}