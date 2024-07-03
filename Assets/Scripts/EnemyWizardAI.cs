using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System.Linq;
public class EnemyWizardAI : CharacterAI
{
    public List<EnemyAction> possibleActions = new List<EnemyAction>();
    public int comfortDistance;
    Queue<EnemyAction> actionQ = new Queue<EnemyAction>();
    public override void ConductTurn()
    {
        foreach (var item in possibleActions)
        {
            actionQ.Enqueue(item);
        }
        DoNextAction();
    }
        

    public override void DoNextAction()
    {
       StartCoroutine(q());
        IEnumerator q()
        {
            yield return new WaitForSeconds(.2f);
            if(actionQ.Count > 0)
            {
                EnemyAction ea = actionQ.Dequeue();

                switch(ea.action){
                    case E_Action.RUN_AWAY:
                    unit.battleTokens.DeductMoveToken();
                    Move(GetFurthestSlotToWalkTo());
                    break;
                    case E_Action.SKILL:
                    unit.battleTokens.DeductActionToken();
                    CastSkill(ea.castable as Skill);
                    break;
                    case E_Action.END:
                
                    BattleManager.inst.UnitIteration();
                       MapManager.inst.map.UpdateGrid();
                    break;
                    default:
               
                    BattleManager.inst.UnitIteration();
                       MapManager.inst.map.UpdateGrid();
                    break;

                }
            }
            else
            {
               
                BattleManager.inst.UnitIteration();
                   MapManager.inst.map.UpdateGrid();
            }
        }
    }
    

    public virtual void WhatToDo()
    {
        if(PlayerUnitInRadiusDIST(2)) // run away
        {
            if(unit.battleTokens.canMove())
            {
                if(canMove())
                {
                    unit.battleTokens.DeductMoveToken();
                    Move(GetFurthestSlotToWalkTo());
                }
                else
                {
                    if( unit.battleTokens. canAct()){
                        Debug.Log("Attack");
                        StartCoroutine(q());
                        IEnumerator q()
                        {
                            yield return new WaitForSeconds(.1f);
                            BattleManager.inst.UnitIteration();
                        }
                    }
                    else
                    {
                        StartCoroutine(q());
                        IEnumerator q()
                        {
                            yield return new WaitForSeconds(.1f);
                            BattleManager.inst.UnitIteration();
                        }
                    }
                }
            }
        }
    }



   
   
}