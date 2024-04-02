using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.Events;
using DG.Tweening;


public class ObjectiveManager : Singleton<ObjectiveManager>
{
    public Objective objective;
    public List<HostageData> hostageDatas = new List<HostageData>();
    public List<RetrevialData> retrevialDatas = new List<RetrevialData>();
    public SpecialSlot hostageInteractablePrefab;
    public bool predecideObjective;
    public Objective.ObjectiveEnum predecidedObjective;
    public void GenerateObjective()
    {
        objective = new Objective();
        Objective.ObjectiveEnum oe;
        if(GameManager.inst.chosenQuest)
        {oe = GameManager.inst.chosenObjective;}
        else if(predecideObjective)
        {oe = predecidedObjective;}
        else
        {oe =  MiscFunctions.RandomEnumValue<Objective.ObjectiveEnum>();}
        switch (oe)
        {
            case Objective.ObjectiveEnum.CLEARAREA:
            ObjectiveData od = new ObjectiveData();
            od.objectiveEnum = oe;
            objective.SetUp(od);
            break;
            case Objective.ObjectiveEnum.HOSTAGE:
            HostageData hd = hostageDatas[Random.Range(0,hostageDatas.Count)];
            hd.objectiveEnum = oe;
            objective.SetUp(hd);
            break;
            case Objective.ObjectiveEnum.RETRIEVAL:
            RetrevialData rd = retrevialDatas[Random.Range(0,retrevialDatas.Count)];
            rd.objectiveEnum = oe;
            objective.SetUp(rd);
            break;
            case Objective.ObjectiveEnum.BOSS:
          BossObjectiveData boss = new   BossObjectiveData();
            boss.objectiveEnum = oe;
            objective.SetUp(boss);
            break;
        }
        
    }

    public bool CheckIfComplete()
    {
        if(GameManager.inst.doNotGenObjective)
        { return false;}
        bool b = objective.CheckIfComplete();
        if(b){
            BattleManager.inst.Win();
        }
        return b;
    }

    public bool hostageInPlayerPossession(){
       return BattleManager.inst.playerUnits.Contains(ObjectiveManager.inst.objective.hostageUnit);
    }
}

[System.Serializable]
public class ObjectiveData
{
    public Objective.ObjectiveEnum objectiveEnum;
}

[System.Serializable]
public class HostageData : ObjectiveData
{
    public DefinedCharacter npc;
    

}

[System.Serializable]
public class BossObjectiveData : ObjectiveData
{
   

}

[System.Serializable]
public class RetrevialData : ObjectiveData
{
    public RetrevialableObject prefab;
    

}