using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
public enum GameState{PLAYERHOVER,PLAYERUI,PLAYERSELECT, UNITMOVE,ENEMYTURN,INTERACT}
public class GameManager : Singleton<GameManager>
{
    public GameState currentGameState;
    
    public void Start()
    {
        GameInit();
       
    }

    void GameInit()
    {
        //MapManager.inst.grid.InitGrid();
        MapGenerator.inst.MakeMap();
         MapManager.inst.  ChangeRoom(    MapManager.inst.rooms[0]);
        ObjectiveManager.inst.GenerateObjective();
        MapGenerator.inst. CreateStartingUnits();
        BattleManager.inst.Begin();
        BlackFade.inst.FadeOut();
    }


    
  
    public void ChangeGameState(GameState newGameState)
    {
        currentGameState = newGameState;
    }

    public bool checkGameState(GameState gameState)
    {
        if(gameState == currentGameState)
        {return true;}
        return false;
    }

}