using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System.Linq;
public class CharacterAIFunctions 
{
    public Unit unit;
    public virtual Unit GetLowestHealth(List<Unit> u)
    {
        List<Unit> hp = SortByLowestHealth(u);
        if(hp.Count > 0)
        { return hp.First(); }
        return null;
    }  
    
    public List<Unit> SortByLowestHealth(List<Unit> u)
    {return  u.OrderBy(f => f.health.currentHealth).ToList();}

    public List<Unit> SortByClosest(List<Unit> u,Vector3 point)
    {return u.Where(n => n && n != unit).OrderBy(n => (n.transform.position - point).sqrMagnitude).ToList();}

    public List<Unit> GetAllyUnits()
    {
        if(unit.side == Side.PLAYER)
        {
            return BattleManager.inst.playerUnits;
        }
        else if(unit.side == Side.ENEMY)
        {
            return BattleManager.inst.enemyUnits;
        }

        return null;
    }

    public List<Unit> GetOpposingUnits()
    {
        if(unit.side == Side.PLAYER)
        {
            return BattleManager.inst.enemyUnits;
        }
        else if(unit.side == Side.ENEMY)
        {
            return BattleManager.inst.playerUnits;
        }

        return null;
    }
    
    public bool canReposition(Castable castable)
    {
        if(canMove())
        {
            if(castable is Skill skill)
            {
                if(skill is SelfSkill selfSkill)
                {
                    if(selfSkill.radius == 0){
                        return true;
                    }
                    else
                    {
                        if(selfSkill.side == Side.ENEMY)
                        {
                            if(UnitInRadiusDIST(unit.stats().moveRange + selfSkill.radius,GetOpposingUnits())) //may have problems
                            {return true;}
                        }
                        else if(selfSkill.side == Side.PLAYER)
                        {
                            if(UnitInRadiusDIST(unit.stats().moveRange + selfSkill.radius,GetAllyUnits())) //may have problems
                            {return true;}
                        }
                    }
                }
                else if(skill is ProjectileSkill proj)
                { 
                    return GetProjectileReposSlots(proj).Count > 0;
                }
                else if(skill is RadiusSkill radiusSkill)
                {
                    if(radiusSkill.side == Side.ENEMY)
                    {
                        Debug.Log("woogly");
                        if(UnitInRadiusDIST(unit.stats().moveRange + radiusSkill.radius,GetOpposingUnits())) //may have problems
                        {return true;}
                    }
                    else if(radiusSkill.side == Side.PLAYER)
                    {
                        if(UnitInRadiusDIST(unit.stats().moveRange + radiusSkill.radius,GetAllyUnits())) //may have problems
                        {return true;}
                    }
                }

            }
            else if(castable is Item item){
                Debug.Log("Item cast");
            }
            else{
                Debug.LogAssertion("????");
                return false;
            }
            
        }

        return false;
    }

  public List<Slot> GetProjectileReposSlots(ProjectileSkill projectileSkill) //may shit itself
    {
        List<Slot> candidates = new List<Slot>();
        List<Slot> allPotentialSlots = new List<Slot>();
        List<Slot> moveRange = unit.slot.func.GetRadiusSlots(unit.stats().moveRange,null,false);
        moveRange.Add(unit.slot);
       
        foreach (var item in GetOpposingUnits())
        {
            foreach (var sl in item.slot.func.GetProjectilePath(projectileSkill))
            {  
                allPotentialSlots.Add(sl);
            }
        }
        foreach (var item in allPotentialSlots ) 
        {
            if(moveRange.Contains(item) )
            {
                candidates.Add(item);
            }
        }

        return candidates;
    }
    
    public Dictionary<Unit,List<Slot>> PathsToUnits(List<Unit> u)
    {
        Dictionary<Unit,List<Slot>> d = new Dictionary<Unit, List<Slot>>();
        foreach (var item in u)
        {
            d.Add(item,new List<Slot>());
            Slot sl = GetSlotAdjacentToUnit(item);
            if(sl != null){
                List<Node> path =  MapManager.inst.map.aStar.FindPath(unit.slot.node,sl.node); 
                if(path.Contains(unit.slot.node)) 
                {path.Remove(unit.slot.node);}
                
                foreach (var node in path)
                { d[item].Add(node.slot); }
            }
            
        }   
        return d;
    }

    public Slot GetSlotAdjacentToUnit(Unit u)
    {
        List<Slot> candidates = new List<Slot>();
        foreach (var item in u.slot.func.GetSlotsInPlusShape(1))
        {
            if(item.cont.walkable() && !item.node.isBlocked)
            {
                candidates.Add(item);
            }
        }

        List<Slot>  closest = candidates.Where(n => n && n != unit).OrderBy(n => (n.transform.position - unit.transform.position).sqrMagnitude).ToList();
        if(closest.Count > 0){
            return closest.First();
        }
        else
        {
            Debug.Log("No valid slots next to: " + u.character.characterName.fullName()); 
            return null;
        }
    }

    public Slot GetFurthestSlotFromPoint(List<Slot> slots,Slot point)
    {
        Slot sl = null;
        List<Slot> candidates = new List<Slot>();
        foreach (var item in slots)
        {
            if(item.cont.walkable()){
if(!candidates.Contains(item))
            {candidates.Add(item);}
            }
            
            
        }
        List<Slot>  orderedByclosest = candidates.Where(n => n && n != unit).OrderBy(n => (n.transform.position - point.transform.position).sqrMagnitude).ToList();
        sl = orderedByclosest.Last();
        return sl; 

        
    }
    

    public bool canMove()
    {
        List<Slot> ss = unit.slot.func.GetRadiusSlots(unit.stats().moveRange,null,false);
        foreach (var item in ss)
        {
            if(CanWalkTo(item))
            {return true;}
        }
        return false;
    }

    public bool CanWalkTo(Slot s){
        List<Node> n =  MapManager.inst.map.aStar.FindPath(unit.slot.node,s.node);
        if(n.Contains(s.node))
        {return true;}
        else
        {return false;}
    }
    
    public bool UnitInRadiusDIST(int radius, List<Unit> u)
    {
        Dictionary<Unit,List<Slot>> enemyUnitDict = PathsToUnits(u);
        foreach (var item in enemyUnitDict)
        {
            float a = Vector3.Distance(unit.slot.transform.position,item.Key.slot.transform.position);
            int r = (int)a/5;
            if(r <= radius)
            {return true;}
        }
        return false;
    }


}