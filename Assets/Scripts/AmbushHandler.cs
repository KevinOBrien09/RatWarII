using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.Events;


public class AmbushHandler : Singleton<AmbushHandler>
{
    public ParticleSystem dig;
    
    public void SpawnAmbush()
    {
        List<Slot> candidates = GetEnemyStart();
        //BattleManager.inst.currentUnit.slot.func.GetRadiusSlots(6,null,true);
        // foreach (var item in candidates)
        // {
        //     item.areaIndicator.gameObject.SetActive(true);
        //     item.areaIndicator.color = Color.magenta;
        // }
        System.Random rng = new System.Random();
        candidates = candidates.OrderBy(_ => rng.Next()).ToList();
        for (int i = 0; i < 1; i++)
        {
            Slot s =  candidates[i];
            if(s.cont.walkable())
            {
                UnitFactory.inst.CreateEnemyUnit(s);
                
            }
           
        }
        BattleManager.inst.ResetTurnOrder();
        Debug.Log("SpawnAmbush");   
    }

    public  List<Slot> GetEnemyStart(){
         List<Slot> s = new List<Slot>();
        for (int x = 0; x < MapManager.inst.map.iGridSizeX; x++)
        {
            for (int y = MapManager.inst.map.iGridSizeY-3; y < MapManager.inst.map.iGridSizeY; y++)
            {
                s.Add( MapManager.inst.map.NodeArray[x,y].slot);
            }
            
        }

        return s;
    }
}