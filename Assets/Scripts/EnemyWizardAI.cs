using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System.Linq;
public class EnemyWizardAI : CharacterAI
{
   
    public int comfortDistance;
    public int charge; 


    public override List<EnemyAction>  WhatToDo()
    {
        RadiusSkill rs = possibleActions["strike"][0].castable as RadiusSkill;
        var v = PreRadiusSkill(rs);
        if(v.allNearbyUnits.Count > 0)
        {
            //attack if in correct pos or retreat then strike. 
            return LockInRadiusSkill(v.allNearbyUnits,v.fullRange,rs,"strike");
        }
        return null;

    }


   



   
   
}