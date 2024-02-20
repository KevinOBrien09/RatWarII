using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System.Linq;
public class FlyAI : CharacterAI
{
    public Skill shoot;
    public override void ConductTurn()
    {
        UnitMover.inst.EnterSelectionMode(BattleManager.inst.currentUnit.slot);
        List<Slot> pointsOfInterest = new List<Slot>();
        foreach (var item in BattleManager.inst.playerUnits)
        {pointsOfInterest.Add(item.slot);}
        List<Slot>  closest = pointsOfInterest.Where(n => n && n != this)
        .OrderBy(n => (n.transform.position - unit.transform.position).sqrMagnitude).ToList();
        Slot closestPOI = closest[0];
        poi = closestPOI;
        float dist = Vector3.Distance(unit.transform.position,poi.transform.position);
        float distToPOI = (int) dist/5;
        ProjectileSkill ps = (ProjectileSkill) shoot;
        bool inRange = distToPOI <= ps.howManyTiles;
        if(inRange) 
        { 
            int casterX = BattleManager.inst.currentUnit.slot.node.iGridX;
            int casterY = BattleManager.inst.currentUnit.slot.node.iGridY;
            int targetX = poi.node.iGridX;
            int targetY = poi.node.iGridY;
            bool xMatch = casterX == targetX ;
            bool yMatch = casterY == targetY;
            if(xMatch || yMatch )
            {
                if(canCast(shoot)) //free shot
                {CastSkill(shoot);}
                else //if blocked
                {StartCoroutine(Repos(true)); }
            }
            else //in range but X/Y is off.
            {StartCoroutine(Repos(false));}
        }
        else //nowhere close, move closer to slot and then check if in range
        {StartCoroutine(bigMove());}
      

        IEnumerator Repos(bool furthest)
        {
            List<Slot> plus = poi.func.GetSlotsInPlusShape( ps.howManyTiles,shoot).Where(n => n && n != this)
            .OrderBy(n => (n.transform.position - unit.transform.position).sqrMagnitude).ToList();
            IEnumerable<Slot> g = null;
            if(furthest)
            {g =  Enumerable.Reverse(plus);}
            else
            {g = plus;}
          
            Queue<Slot> q = new Queue<Slot>();
            foreach (var item in g)
            {q.Enqueue(item);}
            for (int i = 0; i < q.Count; i++)
            {
                Slot c =  q.Dequeue();
                if(c.cont.walkable())
                {
                    loc = c;
                    break;
                }
                else
                {continue;}
            }
            if(loc == null)
            {
                StartCoroutine(bigMove());
                Debug.LogAssertion("no valid slots");
                yield   break;
            }
            else
            {
                Move();
                while(aiMove)
                {yield return null;}
                yield return new WaitForSeconds(.1f);
                if(canCast(shoot)) //now in range, shoot
                { CastSkill(shoot); }
                else //in case of fucky shit
                {  
                    StartCoroutine(bigMove());
                    Debug.LogAssertion("in range but blocked");
                }
            }
        }
            
        IEnumerator bigMove(Slot s = null)
        {
            loc =  GetSlot();
            Move();
            while(aiMove)
            {yield return null;}
            yield return new WaitForSeconds(.1f);
            float dist = Vector3.Distance(unit.transform.position,poi.transform.position);
            float distToPOI = (int) dist/5;
            bool inRange = distToPOI <= ps.howManyTiles;
            if(inRange)
            { 
                int casterX = BattleManager.inst.currentUnit.slot.node.iGridX;
                int casterY = BattleManager.inst.currentUnit.slot.node.iGridY;
                int targetX = poi.node.iGridX;
                int targetY = poi.node.iGridY;
                bool xMatch = casterX == targetX ;
                bool yMatch = casterY == targetY;
                if(xMatch || yMatch ) //moved and XY matched
                {
                    if(canCast(shoot)) //fire shot
                    {CastSkill(shoot);}
                    else //in range X/Y match but is blocked
                    {BattleManager.inst.UnitIteration();}
                }
                else //in range but X/Y mismatch already moved so no repos.
                {BattleManager.inst. UnitIteration();}
            }
            else // not in range end turn.
            {BattleManager.inst. UnitIteration();}
        }
    }
}