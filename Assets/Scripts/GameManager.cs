using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
public enum GameState{PLAYERHOVER,PLAYERUI,PLAYERSELECT, UNITMOVE,ENEMYTURN}
public class GameManager : Singleton<GameManager>
{
    public GameState currentGameState;
    
  
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