using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System.Linq;
public class CharacterAI : MonoBehaviour
{
    public enum EnemyState{CHASING,INRANGE,RETREAT}
    public Unit unit;
    public bool aiMove;
    [Range(0,100)]  public int runAwayThreshhold = 20;
    public EnemyState enemyState;
    public Slot poi,loc;
    public Skill strike;
    public void Init(Unit u)
    {
        unit = u;
    }

    public virtual void ConductTurn()
    {
        StartCoroutine(q());
        IEnumerator q()
        {
            loc = GetSlot();
            yield return new WaitForSeconds(.1f);
            

            if(canCast(strike))
            {
                CastSkill(strike);
            }
            else
            {
                Debug.Log("Not In Range");
                Move();
                while(aiMove)
                {yield return null;}
                yield return new WaitForSeconds(.1f);
                if(canCast(strike))
                {
                    CastSkill(strike);
                }
                else{
                    BattleManager.inst. UnitIteration();
                }

                
            }

         
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
        else if(s is ProjectileSkill proj)
        {
            CamFollow.inst.target = unit.slot.transform;
            SkillAimer.inst.validSlots.Add(poi);
            SkillAimer.inst._skill = s;
            SkillAimer.inst.RecieveSlot(poi);
        }
        else if(s is RadiusSkill radiusSkill)
        {
            CamFollow.inst.target = unit.slot.transform;
            SkillAimer.inst.validSlots.Add(poi);
            SkillAimer.inst._skill = s;
            SkillAimer.inst.RecieveSlot(poi);
        }
    }



    public bool canCast(Skill s)
    {
        if(s is SelfSkill selfSkill)
        {
            SkillAimer.inst.slot = unit.slot;
            SkillAimer.inst.caster = unit;
           
            return true;
        }
        else if(s is ProjectileSkill proj)
        {
            SkillAimer.inst.slot = unit.slot;
            SkillAimer.inst.caster = unit;
            SkillAimer.inst.ProjectileAim(proj);
            if(SkillAimer.inst.validSlots.Contains(poi))
            {return true;}
            else
            {
                SkillAimer.inst.validSlots.Clear();
                return false;
            }
        }
        else if(s is RadiusSkill radiusSkill)
        {
            float dist = Vector3.Distance(unit.transform.position,poi.transform.position);
            float distToPOI = (int) dist/5;
            return distToPOI <= radiusSkill.radius;
        }

        return false;
    }
    

    public virtual void Move()
    {
        StartCoroutine(q());
        IEnumerator q()
        {
            aiMove = true;
           
      
            yield return new WaitForSeconds(.5f);
            UnitMover.inst.InitializeMove(loc);
            while(unit.moving)
            {yield return null;}
            SlotInfoDisplay.inst.Disable();
            yield return new WaitForSeconds(.2f);
     
            UnitMover.inst.ExitSelectionMode();
            
            aiMove = false;
        }
    }

    public virtual Slot GetSlot(Slot st = null)
    {
        if(st != null){
            return st;
        }
        UnitMover.inst.EnterSelectionMode(BattleManager.inst.currentUnit.slot);
        Slot s = null;

        List<Slot> pointsOfInterest = new List<Slot>();
        
        foreach (var item in BattleManager.inst.playerUnits)
        {pointsOfInterest.Add(item.slot);}

        if(unit.health.healthOverPercent(runAwayThreshhold))
        {
            s = closestSlotToPoint(unit.transform,pointsOfInterest);
            enemyState = EnemyState.CHASING;
        } 
        else
        { 
            s = furthestSlotToPoint(unit.transform,pointsOfInterest);
            enemyState = EnemyState.RETREAT;
        }

            return s;
            
    }


   public Slot closestSlotToPoint(Transform origin,List<Slot> pointsOfInterest)
    {
        List<Slot>  closest = pointsOfInterest.Where(n => n && n != this)
        .OrderBy(n => (n.transform.position - unit.transform.position).sqrMagnitude).ToList();
        Slot dest = null;
        if(closest.Count > 0)
        {
            Slot closestPOI = closest[0];
            poi = closestPOI;
            List<Slot>  closestSlot =  UnitMover.inst.validSlots.Where(n => n && n != this)
            .OrderBy(n => (n.transform.position - closestPOI.transform. position).sqrMagnitude).ToList();
            if(closestSlot.Count > 0){
                dest = closestSlot[0];
            }
           
        }
        if(dest != null){
               
        return dest;
        }
        else
        {
            Debug.LogAssertion("VALID DESTINATION SLOT NOT FOUND, RETURNING RANDOM SLOT!!!");
            return  UnitMover.inst.validSlots[Random.Range(0,UnitMover.inst.validSlots.Count)];
        }
    }

    Slot furthestSlotToPoint(Transform origin,List<Slot> pointsOfInterest)
    {
        List<Slot>  furthest = pointsOfInterest.Where(n => n && n != this)
        .OrderBy(n => (n.transform.position - unit.transform.position).sqrMagnitude).ToList();
        Slot dest = null;
        if(furthest.Count > 0)
        {
            Slot furthestPOI = furthest.Last();
            poi = furthestPOI;
            List<Slot>  furthestSlot =  UnitMover.inst.validSlots.Where(n => n && n != this)
            .OrderBy(n => (n.transform.position - furthestPOI.transform. position).sqrMagnitude).ToList();
            dest =  furthestSlot.Last();
        }
        if(dest != null)
        {
            return dest;
        }
        else
        {
            Debug.LogAssertion("VALID DESTINATION SLOT NOT FOUND, RETURNING RANDOM SLOT!!!");
            return  UnitMover.inst.validSlots[Random.Range(0,UnitMover.inst.validSlots.Count)];
        }
    }



}