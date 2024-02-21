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
    public void GenerateObjective()
    {
        objective = new Objective();
        Objective.ObjectiveEnum oe = Objective.ObjectiveEnum.CLEARAREA;
        // MiscFunctions.RandomEnumValue<Objective.ObjectiveEnum>();
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
          
        }
        
    }

    public bool CheckIfComplete()
    {
        bool b = objective.CheckIfComplete();
        if(b){
            BattleManager.inst.Win();
        }
        return b;
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
public class RetrevialData : ObjectiveData
{
    public RetrevialableObject prefab;
    

}