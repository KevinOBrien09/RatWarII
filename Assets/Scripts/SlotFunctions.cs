using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System.Linq;
public class SlotFunctions
{
    public Slot slot;
    public List<Direction> CheckIfSideSlot()
    {
        Dictionary<Direction,Vector3> dirDict = new Dictionary<Direction, Vector3>();
        List<Slot> slots = new List<Slot>();
        List<Direction> directions = new List<Direction>();
        dirDict.Add(Direction.UP,  slot.rayShooter.forward);
        dirDict.Add(Direction.DOWN,-slot.rayShooter.forward);
        dirDict.Add(Direction.RIGHT,slot.rayShooter.right);
        dirDict.Add(Direction.LEFT,-slot.rayShooter.right);

        foreach (var item in dirDict)
        {
            RaycastHit hit ;
            if(Physics.Raycast(slot. rayShooter.position,item.Value * 5,maxDistance: 5,hitInfo: out hit))  
            {
                Slot s = null;
                if(hit.collider.gameObject !=null)
                {
                    if(hit.collider.gameObject.TryGetComponent<Slot>(out s))
                    { slots.Add(s); }
                }
                else
                {
                    directions.Add(item.Key);
                }
            }
            else{
                directions.Add(item.Key);
            }
        }
        
        return directions;
        
    }

    public List<Wall> WallChecker(){
        List<Wall> walls = new List<Wall>();
        List<Vector3> directions = new List<Vector3>();
        directions.Add(slot.rayShooter.forward);
        directions.Add(-slot.rayShooter.forward);
        directions.Add(slot.rayShooter.right);
        directions.Add(-slot.rayShooter.right);
        foreach (var item in directions)
        {
            RaycastHit hit ;
            if(Physics.Raycast(new Vector3(slot. rayShooter.position.x,slot. rayShooter.position.y+1,slot. rayShooter.position.z) ,item * 5,maxDistance: 5,hitInfo: out hit))  
            {
                Wall w = null;
                if(hit.collider.gameObject !=null)
                {
                    if(hit.collider.gameObject.TryGetComponent<Wall>(out w))
                    { walls.Add(w); }
                }
            }
        }


        return walls;

    }

   public List<Slot> GetNeighbouringSlots()
    {
        List<Slot> slots = new List<Slot>();
        List<Vector3> directions = new List<Vector3>();
        directions.Add(slot.rayShooter.forward);
        directions.Add(-slot.rayShooter.forward);
        directions.Add(slot.rayShooter.right);
        directions.Add(-slot.rayShooter.right);

        foreach (var item in directions)
        {
            RaycastHit hit ;
            if(Physics.Raycast(slot. rayShooter.position,item * 5,maxDistance: 5,hitInfo: out hit))  
            {
                Slot s = null;
                if(hit.collider.gameObject !=null)
                {
                    if(hit.collider.gameObject.TryGetComponent<Slot>(out s))
                    { slots.Add(s); }
                }
            }
        }
        return slots;
    }
    
    public List<Slot> FilterUnadjacents(List<Slot> candidates,List<Slot> slots,bool skill)
    {
        foreach (var item in GetNeighbouringSlots())
        {
            if(candidates.Contains(item))
            {
                if(!slots.Contains(item))
                {
                    if(skill)
                    {
                        slots.Add(item);
                        item.func.FilterUnadjacents(candidates,slots,skill);
                    }
                    else
                    {
                        if(item.cont.walkable())
                        {
                            slots.Add(item);
                            item.func.FilterUnadjacents(candidates,slots,skill);
                        }
                    }
                }
            }
        }
        return slots;
    }
 

    public List<Slot> GetRadiusSlots(int radius,Skill skill,bool fuckItWeBall)
    {
        List<Slot> candidateSlots = new List<Slot>();
        List<Slot> validSlots = new List<Slot>();
        Collider[] c = Physics.OverlapSphere(slot.transform.position,radius*5);
        foreach(var item in c)
        {
            if(item.gameObject.name == "XD"){
                Debug.Log("Picking up boat tiles");
            }
            Slot s = null;
            bool v = item.TryGetComponent<Slot>(out s);
            if(v && MapManager.inst.slotBelongsToGrid(s))  
            {
                if(fuckItWeBall){
                    if(s.cont.walkable())
                    {candidateSlots.Add(s);}
                }
                else if(skill != null)
                {
                    // if(s.cont.unit != null){
                    //     if(!s.cont.unit.isEntity())
                    //     {
                    //         if(skill.canHitBreakableSlots)
                    //         {
                    //             candidateSlots.Add(s);
                    //           //"Breakable and we can break"
                    //             continue;
                    //         }
                    //         else
                    //         {
                    //            //"Current slot is occupied by breakable but we cannot break"
                    //             continue;
                    //         }
                          
                    //     }
                      
                    // }


                    if(skill is RadiusSkill rSkill)
                    {
                        if(rSkill.cannotCastOnSpecialSlot) //temp terrain
                        {
                            if(s.cont.walkable() && s.cont.specialSlot == null)
                            {
                                candidateSlots.Add(s);
                            }
                        }
                         else{
                            
                            candidateSlots.Add(s);
                        }
                        
                    }
                    else{
                       candidateSlots.Add(s);
                    }

                  
                }
                else
                {candidateSlots.Add(s);}
            }
         
        }
     
        List<Slot> ss = new List<Slot>(FilterUnadjacents(candidateSlots,new List<Slot>(),skill));
       
        if(ss.Contains(slot))
        {ss.Remove(slot);}
      
        return ss;
    }

    public List<Slot> GetHorizontalSlots(int length,Skill skill = null)
    {
        List<Slot> validSlots = new List<Slot>();
        int maxX =  MapManager.inst.map.iGridSizeX-1;
        List<Slot> right =   Loop(length, slot.node.iGridX,maxX,true,true,skill);//right
        List<Slot> left =   Loop(length,slot.node.iGridX,maxX,true,false,skill);//left
        foreach (var item in left)
        {
            if(!validSlots.Contains(item))
            {
                if(MapManager.inst.slotBelongsToGrid(item)){
                    validSlots.Add(item);
                }
                
            }
        }

        foreach (var item in right)
        {
            if(!validSlots.Contains(item))
            {
                if(MapManager.inst.slotBelongsToGrid(item))
                {
                    validSlots.Add(item);
                }
            }
        }

        return validSlots;
    }

    public List<Slot> GetVerticalSlots(int length,Skill skill = null)
    {
        List<Slot> validSlots = new List<Slot>();
        int maxY = MapManager.inst.map.iGridSizeY-1;
     
     
        List<Slot> up = Loop(length,slot.node.iGridY,maxY,false,true,skill);//up
        List<Slot> down = Loop(length,slot.node.iGridY,maxY,false,false,skill);//down

          foreach (var item in up)
        {
            if(!validSlots.Contains(item))
            {
                if(MapManager.inst.slotBelongsToGrid(item)){
                    validSlots.Add(item);
                }
                
            }
        }

        foreach (var item in down)
        {
            if(!validSlots.Contains(item))
            {
                if(MapManager.inst.slotBelongsToGrid(item))
                {
                    validSlots.Add(item);
                }
            }
        }
        return validSlots;
    }

    public List<Slot> GetSlotsInPlusShape(int length,Skill skill = null)
    {
        List<Slot> validSlots = new List<Slot>();

        List<Slot> vertical = GetVerticalSlots(length,skill);
        List<Slot> horizontal = GetHorizontalSlots(length,skill);

        foreach (var item in horizontal)
        {
            if(!validSlots.Contains(item))
            {validSlots.Add(item);}
        }

        foreach (var item in vertical)
        {
            if(!validSlots.Contains(item))
            {validSlots.Add(item);}
        }
        return validSlots;
    }

    List<Slot> Loop(int length, int dir,int clamp,bool X,bool increase,Skill skill)
    {
        List<Slot> candidateSlots = new List<Slot>();
        int tiles = length+1;
        var projectileSkill = skill as ProjectileSkill;
       
      
        bool breakNextOne = false;
        for (int i = 0; i < tiles; i++)
        {if(dir <= clamp && dir >= 0 )
            {
                Slot s = null;
                if(X)
                { s =  MapManager.inst.map.NodeArray[dir,(int)slot.node.iGridY].slot; }
                else
                { s =  MapManager.inst.map.NodeArray[(int)slot.node.iGridX,dir].slot; }
                
                if(s == null)
                { 
                    if(projectileSkill!= null)
                    {
                        if(!projectileSkill.goThroughWalls)
                        { break; }
                    }
                    else
                    { break; }
                }
                else
                {
                    bool wallAndDontPass = s.cont.wall && !projectileSkill.goThroughWalls;
                    bool unitInSlotNoPass = s.cont.unit != null && !projectileSkill.passThrough && s != slot;
                    if(wallAndDontPass)
                    {break;}
                    if(unitInSlotNoPass){
                        breakNextOne = true;
                    }
                    
                    AddToCandidateSlots(s); 
                 
                    if(breakNextOne)
                    {break;}
                    
                }
            }
            else
            { break; }

            if(increase)
            {dir++;}
            else
            {dir--;}
        
        }
        void AddToCandidateSlots(Slot s)
        {
            
            if(s == slot){return;}
            
            candidateSlots.Add(s);
        }
        return candidateSlots;
    }

    public List<Slot> GetAsteriskSlots(int length,Skill skill = null)
    {
        List<Slot> candidateSlots = new List<Slot>();
        List<Slot> X = GetXSlots(length,skill);
        List<Slot> plus = GetSlotsInPlusShape(length,skill);
        foreach (var item in X)
        {
            if(!candidateSlots.Contains(item))
            {candidateSlots.Add(item);}
        }
         foreach (var item in plus)
        {
            if(!candidateSlots.Contains(item))
            {candidateSlots.Add(item);}
        }

        return candidateSlots;
    }

    public List<Slot> GetXSlots(int length,Skill skill = null)
    {  
        length++;
        List<Slot> candidateSlots = new List<Slot>();
        int x = slot.node.iGridX;
        int y = slot.node.iGridY;
        var projectileSkill = skill as ProjectileSkill;
        XLoop(true,true);
        XLoop(true,false);
        XLoop(false,true);
        XLoop(false,false);

        void XLoop(bool xIncrease,bool yIncrease)
        {
            for (int i = 0; i < length; i++) //UpperRight
            {
                int side = 0;
                int up = 0;
                if(xIncrease)
                {side = x+i;}
                else
                {side = x-i;}
                if(yIncrease)
                {up = y+i;}
                else
                {up = y-i;}


                Vector2 v = new Vector2(side,up);
                if(MapManager.inst.nodeIsValid(v))
                {
                    Slot s =  MapManager.inst.map.NodeArray[(int) v.x,(int)v.y].slot;
                    if(s==slot)
                    {continue;}

                    if(s == null)
                    { 
                        if(projectileSkill!= null)
                        {
                            if(!projectileSkill.goThroughWalls)
                            { break; }
                        }
                        else
                        { break; }
                    }
                    else
                    {
                        if(s.cont.wall)
                        { 
                            if(projectileSkill.goThroughWalls)
                            { continue;}
                            else
                            { break; }
                        }
                        else
                        {
                            if(!projectileSkill.passThrough)
                            {
                                if(s.cont.unit !=null)
                                {
                                    AddToCandidateSlots(s);
                                    break;
                                }
                                else
                                {  AddToCandidateSlots(s);}
                            }
                            else
                            {  AddToCandidateSlots(s);}
                        }
                    }
                }
                else
                { break; }
            }

        }
        void AddToCandidateSlots(Slot s)
        {
            if(MapManager.inst.slotBelongsToGrid(s))
            {
                candidateSlots.Add(s);
            }
        }

        return candidateSlots;

    }
        

}