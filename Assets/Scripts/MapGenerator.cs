using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using UnityEngine.Events;
using DG.Tweening;

public class MapGenerator : Singleton<MapGenerator>
{
    
    public int howManyPartyMembers,howManyEnemies;
    public SpikeSlot spikeSlotPrefab;
    IEnumerator Start()
    {
        BlackFade.inst.FadeOut();
       
        yield return new WaitForEndOfFrame();
        ObjectiveManager.inst.GenerateObjective();
        CreateStartingUnits();
        for (int i = 0; i < 15; i++)
        {
            Slot s = MapManager.inst.RandomSlot();
            s.MakeSpecial(spikeSlotPrefab);
        }
       
        BattleManager.inst.Begin();
    }

    void CreateStartingUnits(){
        List<Slot> shuffle = MapManager.inst.StartingRadius();
        for (int i = 0; i < howManyPartyMembers; i++)
        { UnitFactory.inst. CreatePlayerUnit(shuffle[i]);}

        for (int i = 0; i < howManyEnemies; i++)
        {UnitFactory.inst. CreateEnemyUnit(MapManager.inst.RandomSlot());}
    }
   
}