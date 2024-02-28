using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
public enum GameState{PLAYERHOVER,PLAYERUI,PLAYERSELECT, UNITMOVE,ENEMYTURN,INTERACT}
public class GameManager : Singleton<GameManager>
{
    public GameState currentGameState;
    public bool doNotGenObjective;
    public void Start()
    {
        //GameInit();
       
    }

    public void GameInit()
    {
       // MapManager.inst.grid.InitGrid();
        MapGenerator.inst.Generate();
        StartCoroutine(q());
        IEnumerator q()
        {
            while(MapGenerator.inst.generating)
            {yield return null;}
           MapManager.inst.InitStartRoom();
            if(!doNotGenObjective){
                ObjectiveManager.inst.GenerateObjective();
            }
            
            MapGenerator.inst. CreateStartingUnits();
            BattleManager.inst.Begin();
            BlackFade.inst.FadeOut();
       }
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