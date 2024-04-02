using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[System.Serializable]
public class Objective 
{
    public enum ObjectiveEnum{CLEARAREA,RETRIEVAL,HOSTAGE,BOSS}
    public ObjectiveEnum objectiveEnum;
    public int currentRetrevial,targetRetrevial;
    public Unit hostageUnit;
   public List<Slot> hostageUnitDestSlots = new List<Slot>();
    public void SetUp(ObjectiveData objectiveData)
    {
        objectiveEnum = objectiveData.objectiveEnum;
        switch (objectiveEnum)
        {
            case ObjectiveEnum.CLEARAREA:
            break;
            case ObjectiveEnum.HOSTAGE:
            HostageData hd = objectiveData as HostageData;
            Slot s1 = MapManager.inst.map.endRoom. RandomSlot();
            s1.marked = true;
            hostageUnit = UnitFactory.inst.CreateNPC(s1,hd.npc);
            
            hostageUnit.isHostage = true;
            hostageUnitDestSlots = MapManager.inst.HostageSlots();
            s1.MakeSpecial(ObjectiveManager.inst.hostageInteractablePrefab);
            break;
            case ObjectiveEnum.RETRIEVAL:
            targetRetrevial = 3;
            List<Room> r = new List<Room>(MapManager.inst.map.rooms);
            r.Remove(MapManager.inst.map.startRoom);
            r.Remove(MapManager.inst.map.endRoom);
            CreateRetrievalObject(MapManager.inst.map.endRoom);
            System.Random rng = new System.Random();
            if(MapManager.inst.map.rooms.Count <= 4)
            {Debug.LogAssertion("RANDOMLY SELECTED RETRIEVAL AND THERE IS NOT ENOUGH ROOMS FOR RETRIEVAL MISSION");}
            r.OrderBy(_ => rng.Next()).ToList();
            for (int i = 0; i < 2; i++)
            {CreateRetrievalObject(r[i]);}

            void CreateRetrievalObject(Room r){
                Slot s2 = r.RandomSlot();
                
                RetrevialData rd = objectiveData as RetrevialData;
                s2.MakeSpecial(rd.prefab.GetComponent<SpecialSlot>());
                s2.cont.wall = true;
            }
            break;

            case ObjectiveEnum.BOSS:
            Debug.LogWarning("BOSS LOGIC NEEDS TO BE IMPLEMENTED!!");
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
            List<bool> roomsCleared = new List<bool>();
            foreach (var item in MapManager.inst.map.rooms)
            {roomsCleared.Add(item.roomClear);}
            if(!roomsCleared.Contains(false))
            {return true;}
            else
            {return false; }
            
            case ObjectiveEnum.HOSTAGE:
            return hostageUnitDestSlots.Contains(hostageUnit.slot);
            case ObjectiveEnum.RETRIEVAL:
            
            return currentRetrevial >= targetRetrevial && MapManager.inst.currentRoom.roomClear;
            
            case ObjectiveEnum.BOSS:
            return false;
            
            default:
            Debug.LogAssertion("DEFAULT CASE!!");
            break;
        }

        return false;
    
    }

}