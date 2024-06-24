using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.Events;


public class AmbushHandler : Singleton<AmbushHandler>
{
    public ParticleSystem dig;
    
    public void SpawnAmbush(Ambush ambush)
    {
        Dictionary<Vector2,DefinedCharacter> enemyDict = new Dictionary<Vector2, DefinedCharacter>();
        foreach (var item in ambush.datas)
        {
            if(enemyDict.ContainsKey(item.battlePosition.ToVector2())){
                Debug.LogAssertion(ambush.name + " HAS TWO IDENTICAL POSITIONS!!!");
            }
            else{
                enemyDict.Add(item.battlePosition.ToVector2(),item.enemy);
            }

        }
        Dictionary<Vector2,Slot> candidates = MapManager.inst.map.GetEnemyStartingSlots();
       
        foreach(var item in enemyDict)
        {
            Slot s =  candidates[item.Key];
            if(s.cont.walkable())
            {
                UnitFactory.inst.CreateEnemyUnit(s,item.Value);
                
            }
            else{
                Debug.LogAssertion("SLOT WAS NOT WALKABLE!!!");
            }
           
        }
        BattleManager.inst.ResetTurnOrder();
        Debug.Log("SpawnAmbush");   
    }

    public  List<Slot> GetEnemyStart()
    {
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