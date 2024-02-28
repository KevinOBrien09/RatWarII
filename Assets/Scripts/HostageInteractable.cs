using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class HostageInteractable : Interactable
{
    public override void Go(Unit investigator)
    {
        BattleManager.inst.AddNewUnit(slot.cont.unit,Side.PLAYER);
        BattleTicker.inst.Type("Freed Hostage!");
         ObjectiveProgressIndicator.inst.Show("Quest Progress:<br>" + "Escort the hostage to safety!" );
        Kill();
       
        BattleManager.inst. StartCoroutine(q());
        IEnumerator q()
        {
            yield return new WaitForSeconds(1f);
            BattleManager.inst.EndTurn();
        }
        Debug.Log(slot.cont.unit.character.characterName.firstName);
    }
}