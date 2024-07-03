using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System.Linq;
public class CharacterAI : MonoBehaviour
{
//     public enum EnemyState{CHASING,INRANGE,RETREAT}
  public Unit unit;
  
//     [Range(0,100)]  public int runAwayThreshhold = 20;
//     public EnemyState enemyState;
//     public Slot poi,loc;
    public Skill strike;
    public void Init(Unit u)
    {
        unit = u;

        unit.baselineTokens = new BattleTokens();
        unit.baselineTokens.actionToken = 1;
        unit.baselineTokens.moveToken = 1;
        unit.battleTokens = new BattleTokens(unit.baselineTokens);
    }

    public virtual void ConductTurn()
    {
    
    }

    public virtual void DoNextAction(){
   //BattleManager.inst.UnitIteration();
    }

    
    public virtual void Move(Slot slot)
    {
        StartCoroutine(q());
        IEnumerator q()
        {
            yield return new WaitForSeconds(.5f);
            UnitMover.inst.EnterSelectionMode(BattleManager.inst.currentUnit.slot);
            UnitMover.inst.InitializeMove(slot);
            while(unit.moving)
            {yield return null;}
            SlotInfoDisplay.inst.Disable();
            yield return new WaitForSeconds(.2f);
            UnitMover.inst.ExitSelectionMode();
            yield return new WaitForSeconds(.2f);
            DoNextAction();
        }
    }

    public Slot GetFurthestSlotToWalkTo(){
        Slot sl = null;
        List<Slot> candidates = new List<Slot>();
        foreach (var item in unit.slot.func.GetRadiusSlots(unit.stats().moveRange,null,false))
        {
            if(CanWalkTo(item))
            {
                if(!candidates.Contains(item))
                {candidates.Add(item);}
            }
        }
        List<Slot>  orderedByclosest = candidates.Where(n => n && n != this).OrderBy(n => (n.transform.position - unit.transform.position).sqrMagnitude).ToList();
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
    
    public bool PlayerUnitInRadiusDIST(int radius)
    {
        Dictionary<Unit,List<Slot>> playerUnitDict = PathsToPlayerUnits();
        foreach (var item in playerUnitDict)
        {
            float a = Vector3.Distance(unit.slot.transform.position,item.Key.slot.transform.position);
            int r = (int)a/5;
            if(r <= radius)
            {return true;}
        }
        return false;
    }


    public (Unit,List<Slot>) GetClosestUnit()
    {
        Unit u = null;
        List<Slot> s = null;
        Dictionary<Unit,List<Slot>> playerUnitDict = PathsToPlayerUnits();
        
        foreach (var item in playerUnitDict)
        {
            if(s == null){
                u = item.Key;
                s = item.Value;
            }
            else 
            {
                float a = Vector3.Distance(unit.transform.position, item.Key.transform.position);
                float b = Vector3.Distance(unit.transform.position, u.transform.position);
                if(a < b){
                    u = item.Key;
                    s = item.Value;
                }
            }
        }
        return (u,s);
    }



   public (Unit,List<Slot>) GetFurthestUnit()
    {
        Unit u = null;
        List<Slot> s = null;
        int i = 0;
        foreach (var item in PathsToPlayerUnits())
        {
            if(i < item.Value.Count)
            {
                u = item.Key;
                s = item.Value;
            }
        }
        return (u,s);
    }

    public Dictionary<Unit,List<Slot>> PathsToPlayerUnits()
    {return PathsToUnits(BattleManager.inst.playerUnits);}

    public Dictionary<Unit,List<Slot>> PathsToEnemyUnits()
    {
        List<Unit> u = new List<Unit>(BattleManager.inst.enemyUnits);
        if(u.Contains(unit))
        {u.Remove(unit);}
        return PathsToUnits(u);
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

        List<Slot>  closest = candidates.Where(n => n && n != this).OrderBy(n => (n.transform.position - unit.transform.position).sqrMagnitude).ToList();
        if(closest.Count > 0){
            return closest.First();
        }
        else
        {
            Debug.Log("No valid slots next to: " + u.character.characterName.fullName()); 
            return null;
        }
    }


    public  void CastSkill(Skill s)
    {
        if(s is SelfSkill selfSkill)
        {
            SkillAimer.inst._skill = s;
            CamFollow.inst.target = unit.slot.transform;
            SkillAimer.inst.validSlots.Add(unit.slot);
            SkillAimer.inst.RecieveSlot(unit.slot);
        }
        // else if(s is ProjectileSkill proj)
        // {
        //     CamFollow.inst.target = unit.slot.transform;
        //     SkillAimer.inst.validSlots.Add(poi);
        //     SkillAimer.inst._skill = s;
        //     SkillAimer.inst.RecieveSlot(poi);
        // }
        // else if(s is RadiusSkill radiusSkill)
        // {
        //     CamFollow.inst.target = unit.slot.transform;
        //     SkillAimer.inst.validSlots.Add(poi);
        //     SkillAimer.inst._skill = s;
        //     SkillAimer.inst.RecieveSlot(poi);
        // }
    }



//     public bool canCast(Skill s)
//     {
//         if(s is SelfSkill selfSkill)
//         {
//             SkillAimer.inst.slot = unit.slot;
//             SkillAimer.inst.caster = unit;
           
//             return true;
//         }
//         else if(s is ProjectileSkill proj)
//         {
//             SkillAimer.inst.slot = unit.slot;
//             SkillAimer.inst.caster = unit;
//             SkillAimer.inst.ProjectileAim(proj);
//             if(SkillAimer.inst.validSlots.Contains(poi))
//             {return true;}
//             else
//             {
//                 SkillAimer.inst.validSlots.Clear();
//                 return false;
//             }
//         }
//         else if(s is RadiusSkill radiusSkill)
//         {
//             float dist = Vector3.Distance(unit.transform.position,poi.transform.position);
//             float distToPOI = (int) dist/5;
//             return distToPOI <= radiusSkill.radius;
//         }

//         return false;
//     }
    





}