using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using UnityEngine.Events;
using DG.Tweening;

public class MapGenerator : Singleton<MapGenerator>
{
    public AStar generatorAstar;
    public Grid_ mapGrid;
    public int howManyPartyMembers,howManyEnemies;
    public SpikeSlot spikeSlotPrefab;

   protected override void Awake(){
        base.Awake();
       
        mapGrid.CreateGrid();
      
    }

    public void MakeMap()
    {
       Vector3 source = MapManager.inst.SpawnRoom(mapGrid.NodeArray[0,0].vPosition);
 MapManager.inst.SpawnRoom(new Vector3( source.x + MapManager.inst.rooms[0].grid.vGridWorldSize.x/2 +2.5f ,source.y,source.z));
  
    }



  

   public void CreateStartingUnits(){
        List<Slot> shuffle = MapManager.inst.StartingRadius();
        for (int i = 0; i < howManyPartyMembers; i++)
        { UnitFactory.inst. CreatePlayerUnit(shuffle[i]);}

        for (int i = 0; i < howManyEnemies; i++)
        {UnitFactory.inst. CreateEnemyUnit(MapManager.inst.RandomSlot());}
    }
   
}