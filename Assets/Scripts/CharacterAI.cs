using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System.Linq;
public class CharacterAI : MonoBehaviour
{
    public CharacterAIFunctions chrFunc;
    public List<Material> test = new List<Material>();
    public Unit unit;
    public Skill strike;
    public GenericDictionary<string,List<EnemyAction>> possibleActions = new GenericDictionary<string, List<EnemyAction>>();
    public Queue<EnemyAction> actionQ = new Queue<EnemyAction>();
    public Slot reposSlot,targetSlot;
    public void Init(Unit u)
    {
        chrFunc = new CharacterAIFunctions();
        chrFunc.unit = u;
        unit = u;
        
        unit.baselineTokens = new BattleTokens();
        unit.baselineTokens.actionToken = 1;
        unit.baselineTokens.moveToken = 1;
        unit.battleTokens = new BattleTokens(unit.baselineTokens);
    }

    public virtual void ConductTurn()
    {
        actionQ.Clear();
        List<EnemyAction> e = WhatToDo();
        if(e!= null){
            foreach (var item in e)
            { actionQ.Enqueue(item); }
            DoNextAction();
        }
        else{
            Debug.LogAssertion("ENEMY ACTIONS ARE NULL!!!");
            DoNextAction();
        }
          
   
    }

    public virtual List<EnemyAction>  WhatToDo(){
        
        return possibleActions.First().Value;
    }

    public virtual void DoNextAction()
    {
       StartCoroutine(q());
        IEnumerator q()
        {
            yield return new WaitForSeconds(.2f);
            if(actionQ.Count > 0)
            {
                EnemyAction ea = actionQ.Dequeue();

                switch(ea.action)
                {
                    case E_Action.RUN_AWAY:
                    unit.battleTokens.DeductMoveToken();
                    Move(GetRunAwaySlot(3));
                    break;

                    case E_Action.SKILL:
                    unit.battleTokens.DeductActionToken();
                    CastSkill(ea.castable as Skill);
                    break;

                    case E_Action.RUN_TOWARD:
                    unit.battleTokens.DeductMoveToken();
                    Move(reposSlot);
                    reposSlot = null;
                    break;

                    case E_Action.END:
                    Fin();
                    break;

                    case E_Action.REPOS:
                    if(reposSlot != null){
                        unit.battleTokens.DeductMoveToken();
                        Move(reposSlot);
                        reposSlot = null;
                    }
                    else
                    {
                     
                        Debug.LogAssertion("REPOS SLOT IS NULL!!!");
                        Fin();
                    }
                    break;

                    default:
                    Fin();
                    break;

                }
            }
            else
            {
                Fin();
            }

           
        }
    }
    void Fin()
    {
        BattleManager.inst.UnitIteration();
        MapManager.inst.map.UpdateGrid();
    }

    
    public virtual void Move(Slot slot)
    {
        StartCoroutine(q());
        IEnumerator q()
        {
            yield return new WaitForSeconds(.2f);
            UnitMover.inst.EnterSelectionMode(BattleManager.inst.currentUnit.slot);
            UnitMover.inst.InitializeMove(slot);
            while(unit.moving)
            {yield return null;}
            SlotInfoDisplay.inst.Disable();
            yield return new WaitForSeconds(.1f);
            UnitMover.inst.ExitSelectionMode();
            yield return new WaitForSeconds(.1f);
            DoNextAction();
        }
    }
    
    

    

    public virtual Unit WhoToTarget(List<Unit> u)
    {
        return chrFunc.GetLowestHealth(u);
    }

  
    
    public  void CastSkill(Skill s)
    {
        if(s is SelfSkill selfSkill)
        {
            SkillAimer.inst._skill = s;
            CamFollow.inst.target = unit.slot.transform;
            foreach (var item in unit.slot.func.GetRadiusSlots(selfSkill.radius,selfSkill,false))
            {
                SkillAimer.inst.validSlots.Add(item);
            }
        
            SkillAimer.inst.validSlots.Add(unit.slot);
            SkillAimer.inst.RecieveSlot(unit.slot);
        }
        else if(s is RadiusSkill radiusSkill)
        {
            if(targetSlot != null){
                CamFollow.inst.target = unit.slot.transform;
                SkillAimer.inst.validSlots.Add(targetSlot);
                SkillAimer.inst._skill = s;
                SkillAimer.inst.RecieveSlot(targetSlot);
                targetSlot = null;
            }
            else{
                Debug.LogAssertion("TARGET SLOT IS NULL");
                Fin();

            }
           
        }
        // else if(s is ProjectileSkill proj)
        // {
        //     CamFollow.inst.target = unit.slot.transform;
        //     SkillAimer.inst.validSlots.Add(poi);
        //     SkillAimer.inst._skill = s;
        //     SkillAimer.inst.RecieveSlot(poi);
        // }
     
    }



     public (List<Slot> fullRange,List<Unit> allNearbyUnits) PreRadiusSkill(RadiusSkill rs)
    {
        int XD = rs.radius + unit.stats().moveRange + 1;
        List<Slot> accounted = new List<Slot>();
        List<Unit> allUnitsInRadius = new List<Unit>();
        for (int i = 0; i < XD; i++)
        {
            var sl = unit.slot.func.GetRawRadiusSlots(i);
            List<Unit> un = new List<Unit>();
            foreach (var item in sl)
            {
                if(!accounted.Contains(item))
                {
                    if(item != unit.slot)
                    {
                        accounted.Add(item);
                        Unit u = item.cont.unit;
                        if(u !=null)
                        {
                            if(u.side == Side.PLAYER)
                            { 
                                un.Add(u); 
                                allUnitsInRadius.Add(u);
                            }
                        }
                    }
                }
            }
        }

        return (accounted,allUnitsInRadius);
    }

    public Slot RandomSlotInMoveRange(){
        List<Slot> moveRadius = unit.slot.func.GetRadiusSlots(unit.stats().moveRange,null,false);
        System.Random rng = new System.Random();
        moveRadius =  moveRadius.OrderBy(_ => rng.Next()).ToList();
        foreach (var item in moveRadius)
        {
            if(moveRadius.Contains(item))
            {
                if(item.cont.walkable())
                {
                    if(chrFunc.CanWalkTo(item))
                    {
                        
                        return item;
                    }
                }
            }
        }
        Debug.LogAssertion("NO RNG SLOT");
        return null;
    }
    
    public Slot MoveTowardUnitGroup(List<Unit> units){
        List<Slot> slots = new List<Slot>();
        foreach (var item in units)
        {
            slots.Add(item.slot);
        }

        Slot mid = MapManager.inst.map.startRoom.GetCenterSlotInGroup(slots);
        List<Slot> moveRadius = unit.slot.func.GetRadiusSlots(unit.stats().moveRange,null,false);
        foreach (var item in  chrFunc.GetClosestSlotFromPoint(moveRadius,mid))
        {
            if(moveRadius.Contains(item))
            {
                if(item.cont.walkable())
                {
                    if(chrFunc.CanWalkTo(item))
                    {
                        
                        return item;
                    }
                }
            }
        }
        Debug.LogAssertion("NO MOVE TOWARD SLOT");
        return null;
    }
  

    public List<EnemyAction> ReposIntoRadiusSkillCAUTIOUS(List<Unit> allNearbyUnits,List<Slot> fullRange,RadiusSkill rs,string actionTag,int lookRadius = 3)
    {
        //tries to run away from surronding units whilst remaining in a cast radius
        //targeting closest

        bool awkward = false;
        List<Unit> surrondingUnits = unit.slot.func.GetSurrondingUnits(lookRadius);
        foreach (var item in chrFunc.GetAllyUnits())
        {
            if(surrondingUnits.Contains(item))
            {surrondingUnits.Remove(item); }
        }

        awkward = surrondingUnits.Count >= 2;
     
        Unit closestUnit = null;
        // 
       var c = chrFunc.SortByClosest(allNearbyUnits,unit.transform.position);
        closestUnit =   c[0];
        //c[Random.Range(0,c.Count)];
     //   [0,]
        //.First();
     
        List<Slot> potentialMoveTos = closestUnit.slot.func.GetRing(rs.radius);
        List<Slot>  orderedByclosest = potentialMoveTos.Where(n => n && n != this).OrderBy(n => (n.transform.position - unit.transform.position).sqrMagnitude).ToList();
        orderedByclosest.Reverse();
        List<Slot> enemyCenter = new List<Slot>();
        if(surrondingUnits.Count == 0){
            surrondingUnits = new List<Unit>(chrFunc.GetOpposingUnits());
        }
        foreach (var item in surrondingUnits)
        {
            enemyCenter.Add(item.slot);
        }
        Slot mid = MapManager.inst.map.startRoom.GetCenterSlotInGroup(enemyCenter);
        foreach (var item in  chrFunc.GetFurthestSlotFromPoint(orderedByclosest,mid))
        {
            if(fullRange.Contains(item))
            {
                if(item.cont.walkable())
                {
                    if(chrFunc.CanWalkTo(item))
                    {
                        reposSlot =  item;
                        targetSlot = closestUnit.slot;
                        return possibleActions["repos-"+actionTag];
                    }
                }
            }
        }

        Debug.LogWarning("No optimal slot to move to, casting on same slot. This should not happen.");
        targetSlot = closestUnit.slot;
        return possibleActions[actionTag];
    }

    public Slot GetRunAwaySlot(int radius)
    {
        List<Unit> surrondingUnits = unit.slot.func.GetSurrondingUnits(radius);
        List<Slot> enemyCenter = new List<Slot>();
        List<Slot> moveRadius = unit.slot.func.GetRadiusSlots(unit.stats().moveRange,null,false);
        foreach (var item in surrondingUnits)
        {
            if(chrFunc.GetOpposingUnits().Contains(item)){
                enemyCenter.Add(item.slot);
            }
           
        }
        Slot mid = MapManager.inst.map.startRoom.GetCenterSlotInGroup(enemyCenter);
        foreach (var item in  chrFunc.GetFurthestSlotFromPoint(moveRadius,mid))
        {
            if(moveRadius.Contains(item))
            {
                if(item.cont.walkable())
                {
                    if(chrFunc.CanWalkTo(item))
                    {
                        return item;
                    }
                }
            }
        }
        Debug.LogAssertion("RunAwaySlot");
        return null;
    }

    public List<EnemyAction> ReposIntoRadiusSkillNORMAL(List<Unit> allNearbyUnits,List<Slot> fullRange,RadiusSkill rs,string actionTag,List<Unit> targets)
    {
        //tries and space out to the maximum skill cast radius
        //if the target is already within range do not move and just cast instead.
        
        Unit closestUnit = null;
        closestUnit = chrFunc.SortByClosest(allNearbyUnits,unit.transform.position).First();
        List<Slot> potentialMoveTos = closestUnit.slot.func.GetRing(rs.radius);
        Slot sl = chrFunc.GetFurthestSlotFromPoint(unit.slot.func.GetRadiusSlots(unit.stats().moveRange,null,false),closestUnit.slot).First();
        List<Slot> strikeRadius = unit.slot.func.GetRawRadiusSlots(rs.radius);
        foreach (var pUnit in targets)
        {
            if(allNearbyUnits.Count > 0)
            {
                Unit target =  pUnit;
                List<Slot> r = target.slot.func.GetRing(rs.radius);
                
                List<Slot>  orderedByclosest = r.Where(n => n && n != this).OrderBy(n => (n.transform.position - unit.transform.position).sqrMagnitude).ToList();
                if( orderedByclosest.Contains(unit.slot))
                {
                    targetSlot = target.slot;
                    return possibleActions[actionTag];
                }
                
                foreach (var item in orderedByclosest)
                {
                    if(fullRange.Contains(item))
                    {
                        if(item.cont.walkable())
                        {
                            if(chrFunc.CanWalkTo(item))
                            {
                                reposSlot =  item;
                                targetSlot = target.slot;
                                return possibleActions["repos-"+actionTag];
                            }
                        }
                    }
                }
            }
        }

        Debug.LogAssertion("NO POSSIBLE RADIUS CAST SCENARIO, VERY BAD!!!");
        return null;

    }

    public bool NoNeedToReposRadius(List<Unit> allNearbyUnits,List<Slot> fullRange,RadiusSkill rs,string actionTag)
    {
        //tries and space out to the maximum skill cast radius whilst targeting the lowest health unit.
        //if the target is already within range do not move and just cast instead.
        
        Unit closestUnit = null;
        closestUnit = chrFunc.SortByClosest(allNearbyUnits,unit.transform.position).First();
        List<Slot> potentialMoveTos = closestUnit.slot.func.GetRing(rs.radius);
        Slot sl = chrFunc.GetFurthestSlotFromPoint(unit.slot.func.GetRadiusSlots(unit.stats().moveRange,null,false),closestUnit.slot).First();
        List<Slot> strikeRadius = unit.slot.func.GetRawRadiusSlots(rs.radius);
        foreach (var pUnit in chrFunc.SortByLowestHealth(BattleManager.inst.playerUnits))
        {
            if(allNearbyUnits.Count > 0)
            {
                Unit target =  pUnit;
                List<Slot> r = target.slot.func.GetRing(rs.radius);
                
                List<Slot>  orderedByclosest = r.Where(n => n && n != this).OrderBy(n => (n.transform.position - unit.transform.position).sqrMagnitude).ToList();
                if( orderedByclosest.Contains(unit.slot))
                {
                    return true;
                }
            }
        }
        return false;
    }



    







}