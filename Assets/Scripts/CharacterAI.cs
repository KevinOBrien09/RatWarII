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
                   // Move(chrFunc.GetFurthestSlotToWalkTo());
                    break;

                    case E_Action.SKILL:
                    unit.battleTokens.DeductActionToken();
                    CastSkill(ea.castable as Skill);
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
    

  

    public List<EnemyAction> LockInRadiusSkill(List<Unit> allNearbyUnits,List<Slot> fullRange,RadiusSkill rs,string actionTag)
    {
        bool nearbyEnemyUnit = false;
        Unit closestUnit = null;
        List<Slot> nearby = unit.slot.func.GetRawRadiusSlots(1);
        foreach (var item in nearby)
        {
            if(item.cont.unit != null)
            {
                if(item.cont.unit.side == Side.PLAYER)
                {
                    Debug.Log(item.cont.unit.character.characterName.firstName);
                    nearbyEnemyUnit = true;
                }
            }
        }

        if(nearbyEnemyUnit) //kinda workimg
        {
            Slot sl = null;
            closestUnit = chrFunc.SortByClosest(allNearbyUnits,unit.transform.position).First();

            Debug.Log(closestUnit.character.characterName.firstName + " is 1 tile away from caster!");

             List<Slot> potentialMoveTos = closestUnit.slot.func.GetRing(rs.radius);
            List<Slot>  orderedByclosest = potentialMoveTos.Where(n => n && n != this).OrderBy(n => (n.transform.position - unit.transform.position).sqrMagnitude).ToList();
            orderedByclosest.Reverse();
            foreach (var item in orderedByclosest)
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
            //Slot sl = chrFunc.GetFurthestSlotFromPoint(unit.slot.func.GetRadiusSlots(move,null,false),closestUnit.slot);
          

        }
        List<Slot> strikeRadius = unit.slot.func.GetRawRadiusSlots(rs.radius);
        foreach (var pUnit in chrFunc.SortByLowestHealth(BattleManager.inst.playerUnits))
        {
            if(allNearbyUnits.Count > 0)
            {
                Unit target =  pUnit;
                List<Slot> potentialMoveTos = target.slot.func.GetRing(rs.radius);
                List<Slot>  orderedByclosest = potentialMoveTos.Where(n => n && n != this).OrderBy(n => (n.transform.position - unit.transform.position).sqrMagnitude).ToList();
                if(potentialMoveTos.Contains(unit.slot))
                {
                    if(!nearbyEnemyUnit)
                    {
                        targetSlot = target.slot;
                        return possibleActions[actionTag];
                    }
                   
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







}