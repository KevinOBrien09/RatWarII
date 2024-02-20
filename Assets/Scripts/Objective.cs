using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class Objective 
{
    public enum ObjectiveEnum{CLEARAREA,RETRIEVAL,HOSTAGE}
    public ObjectiveEnum objectiveEnum;
    public int currentRetrevial,targetRetrevial;
    public Unit hostageUnit;
    List<Slot> hostageUnitDestSlots = new List<Slot>();
    public void SetUp(ObjectiveData objectiveData)
    {
        objectiveEnum = objectiveData.objectiveEnum;
        switch (objectiveEnum)
        {
            case ObjectiveEnum.CLEARAREA:
            break;
            case ObjectiveEnum.HOSTAGE:
            HostageData hd = objectiveData as HostageData;
            Slot s1 = MapManager.inst.RandomSlot();
            hostageUnit = UnitFactory.inst.CreateNPC(s1,hd.npc);
            
            hostageUnit.isHostage = true;
            hostageUnitDestSlots = MapManager.inst.HostageSlots();
            s1.MakeSpecial(ObjectiveManager.inst.hostageInteractablePrefab);
            break;
            case ObjectiveEnum.RETRIEVAL:
            targetRetrevial = 3;
            for (int i = 0; i < 3; i++)
            {
                Slot s2 = MapManager.inst.RandomSlot();
                
                RetrevialData rd = objectiveData as RetrevialData;
                s2.MakeSpecial(rd.prefab.GetComponent<SpecialSlot>());
                s2.cont.wall = true;
            }
            break;
            
            default:
            Debug.LogAssertion("DEFAULT CASE!!");
            break;
        }
    }

    public virtual bool CheckIfComplete()
    {
        switch (objectiveEnum)
        {
            case ObjectiveEnum.CLEARAREA:
            return BattleManager.inst.enemyUnits.Count <= 0;
            case ObjectiveEnum.HOSTAGE:
            return hostageUnitDestSlots.Contains(hostageUnit.slot);
            case ObjectiveEnum.RETRIEVAL:
            
            return currentRetrevial >= targetRetrevial;
            
            
            
            default:
            Debug.LogAssertion("DEFAULT CASE!!");
            break;
        }

        return false;
    
    }

}