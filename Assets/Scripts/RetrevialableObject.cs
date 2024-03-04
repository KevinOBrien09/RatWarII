using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Linq;


public class RetrevialableObject : Interactable
{
    public override void Go(Unit investigator)
    {
        int i = ObjectiveManager.inst.objective.currentRetrevial+1;
        ObjectiveProgressIndicator.inst.Show("Quest Progress:<br>" +  i + "/"+ObjectiveManager.inst.objective.targetRetrevial);
        BattleTicker.inst.Type("Found an acorn! ");
        ObjectiveManager.inst.objective.currentRetrevial++;
        if(!ObjectiveManager.inst.CheckIfComplete())
        {BattleManager.inst. StartCoroutine(q());}
        Kill();
        
        IEnumerator q()
        {
            yield return new WaitForSeconds(1f);
            BattleManager.inst.EndTurn();
        }
    }
}