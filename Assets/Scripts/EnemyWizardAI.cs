using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System.Linq;
public class EnemyWizardAI : CharacterAI
{
    public int lookRadius = 2;
    public int charge; 
    public int summonCD;
    public override List<EnemyAction>  WhatToDo()
    {

        // List<Unit> surrUnits = unit.slot.func.GetSurrondingUnits(lookRadius);
        // List<Unit> surrAlly = new List<Unit>();
        // List<Unit> surrEnemy = new List<Unit>();
        // foreach (var item in surrUnits)
        // {
        //     if(chrFunc.GetAllyUnits().Contains(item))
        //     {
        //         surrAlly.Add(item);
        //     }
        //     else if(chrFunc.GetOpposingUnits().Contains(item))
        //     {
        //         surrEnemy.Add(item);
        //     }
        // }
        
        if(chrFunc.canMove())
        {
            RadiusSkill rs2 = possibleActions["2strike"][0].castable as RadiusSkill; 
            var v2 = PreRadiusSkill(rs2);
            if(v2.allNearbyUnits.Count > 0)
            {
                if(chrFunc.UnitInRadiusDIST(rs2.radius+unit.stats().moveRange,chrFunc.GetOpposingUnits()))
                { 
                    return ReposIntoRadiusSkillCAUTIOUS(v2.allNearbyUnits,v2.fullRange,rs2,"2strike",2); 
                }
                else
                {
                    reposSlot = MoveTowardUnitGroup(chrFunc.GetOpposingUnits());
                    return possibleActions["movetoward"];
                }
            }
             else
                {
                    reposSlot = MoveTowardUnitGroup(chrFunc.GetOpposingUnits());
                    return possibleActions["movetoward"];
                }
        }
        else
        {
            reposSlot = MoveTowardUnitGroup(chrFunc.GetOpposingUnits());
            return possibleActions["movetoward"];
        }
        return null;
    }


   



     // }
                // else if(v.allNearbyUnits.Count > 0){
                //     List<Unit> c = chrFunc.SortByClosest(chrFunc.GetOpposingUnits(),unit.transform.position);
                //     return ReposIntoRadiusSkillNORMAL(v.allNearbyUnits,v.fullRange,rs,"strike",c);
                // }
                // else
                // {
                //     // no target that he can hit and there is no targets in the look radius
                //     reposSlot = MoveTowardUnitGroup(chrFunc.GetOpposingUnits());
                //     return possibleActions["movetoward"];
                // }
            // }
            // else
            // {
            //     Debug.Log("Blokced");
            // }
   
}