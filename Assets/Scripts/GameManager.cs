using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
public enum GameState{PLAYERHOVER,PLAYERUI,PLAYERSELECT, UNITMOVE,ENEMYTURN,INTERACT}
public class GameManager : Singleton<GameManager>
{
    public GameState currentGameState;
    public Objective.ObjectiveEnum chosenObjective;
    public bool doNotGenObjective,chosenQuest,loadFromFile;
    public int saveSlotIndex = 999;
  

    public void Load(){
        SaveData sd =  SaveLoad.Load(saveSlotIndex);
        PartyManager.inst.Load(sd.partySaveData,sd.mapSaveData.lastLocation);
        MapSaveData msd = sd.mapSaveData;
        LocationManager.inst.currentLocation = msd.lastLocation;
    }

    #if UNITY_EDITOR
    void Update(){ //REMOVE
        if(Input.GetKeyDown(KeyCode.M)){
 SaveLoad.Save(saveSlotIndex);
        }
//   if(Input.GetKeyDown(KeyCode.L)){
//               Load();
//   }
    }//REMOVE
    #endif
    public void LoadQuest(Objective.ObjectiveEnum o)
    {
        chosenObjective = o;
        chosenQuest = true;
    }

    public void GameInit()
    {
       // MapManager.inst.grid.InitGrid();

        MapGenerator.inst.BeginGeneration(LocationManager.inst.nextLocBrain);
        //ParseQuirks();
        
    }

    public void GameSetUp(){
StartCoroutine(q());
        IEnumerator q()
        {
            while(MapGenerator.inst.generating)
            {yield return null;}
            MapManager.inst.InitStartRoom();
            if(!doNotGenObjective){
                ObjectiveManager.inst.GenerateObjective();
            }
            
         CreateStartingUnits();
            BattleManager.inst.Begin();
            BlackFade.inst.FadeOut();
       }
    }


      public void CreateStartingUnits()
    {
        List<Slot> shuffle = MapManager.inst.StartingRadius();
        if(!GameManager.inst.loadFromFile)
        {
            for (int i = 0; i < 3; i++)
            { UnitFactory.inst. CreatePlayerUnit(shuffle[i]);}
        }
        else
        {
            int i = 0;
            foreach (var item in  PartyManager .inst.parties[PartyManager .inst.currentParty]. members)
            {
                UnitFactory.inst.CreatePlayerUnit(shuffle[i],item.Value.character);
                i++;
            }
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

    public SaveData Save()
    {

        SaveData save =  new SaveData();
        save.partySaveData = PartyManager.inst.Save();
        if(MapTileManager.inst != null){
 save.mapSaveData = MapTileManager.inst.Save();
        }
        else{
            save.mapSaveData =  SaveLoad.Load(saveSlotIndex).mapSaveData;
        }
       
       
        return save;
    }



}