using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.EventSystems;
public enum Language{ENG,FR,SPA,CH,JP}
public enum GameState{PLAYERHOVER,PLAYERUI,PLAYERSELECT, UNITMOVE,ENEMYTURN}
public class GameManager : Singleton<GameManager>
{
    public GameState currentGameState;
    public Objective.ObjectiveEnum chosenObjective;
    public bool doNotGenObjective,chosenQuest,loadFromFile;
    public int saveSlotIndex = 999;
    public Language language;

    public void Wipe()
    {
        chosenQuest = false;
        saveSlotIndex = 999;
        LocationManager.inst.Wipe();
        PartyManager.inst.Wipe();
        IconGraphicHolder.inst.Wipe();
    }
    
    public void Load()
    {
        SaveData sd =  SaveLoad.Load(saveSlotIndex);
        PartyManager.inst.Load(sd.partySaveData,sd.mapSaveData.lastLocation);
        MapSaveData msd = sd.mapSaveData;
        LocationManager.inst.currentLocation = msd.lastLocation;
    }

    #if UNITY_EDITOR
    void Update()
    { 
        if(Input.GetKeyDown(KeyCode.M))
        {SaveLoad.Save(saveSlotIndex);}

    }
    #endif
    public void LoadQuest(Objective.ObjectiveEnum o)
    {
        chosenObjective = o;
        chosenQuest = true;
    }

    public void GameInit()
    {
        if(LocationManager.inst.locationTravelingTo!= null){
            MapGenerator.inst.BeginGeneration(LocationManager.inst.locationTravelingTo);
        }
        else if(MapGenerator.inst.testing != null){
            LocationManager.inst.locationTravelingTo = MapGenerator.inst.testing;
            MapGenerator.inst.BeginGeneration(LocationManager.inst.locationTravelingTo);
        }
        else{
            Debug.LogWarning("Cannot Load a map!");
        }
       
    }

    public void GameSetUp()
    {
        StartCoroutine(q());
        IEnumerator q()
        {
            while(MapGenerator.inst.generating)
            {yield return null;}
            //MapManager.inst.InitStartRoom();
            if(!doNotGenObjective){
                ObjectiveManager.inst.GenerateObjective();
            }
            CreateOverworldAndBattleUnits();
            //CreateStartingUnits();
            if(MapGenerator.inst.brain != null)
            {
                if(MapGenerator.inst.brain.overrideLocationInfoMusic != null)
                {
                    MusicManager.inst.FadeAndChange(MapGenerator.inst.brain.overrideLocationInfoMusic);
                }
                else if(LocationManager.inst.locationTravelingTo!= null)
                {
                    if(LocationManager.inst.locationTravelingTo.locationMusic!= null)
                    {
                        MusicManager.inst.FadeAndChange(LocationManager.inst.locationTravelingTo.locationMusic);
                    }
                }

            }
            else if(LocationManager.inst.locationTravelingTo!= null)
            {
                if(LocationManager.inst.locationTravelingTo.locationMusic!= null)
                {
                    MusicManager.inst.FadeAndChange(LocationManager.inst.locationTravelingTo.locationMusic);
                }
            }

            MusicManager.inst.ChangeAmbience( LocationManager.inst.locationTravelingTo.ambience.audioClip);
            BattleManager.inst.ambushes = new List<Ambush>(LocationManager.inst.locationTravelingTo.ambushes);
            // PartyController.inst.GrabUnits();    
            BattleManager.inst.overworld.SetActive(true);
            MapManager.inst.map.gameObject.SetActive(false);
            foreach (var item in BattleManager.inst.playerUnits)
            {
                item.gameObject.SetActive(false);
                
            }
            PartyController.inst.TakeControl();  
            //BattleManager.inst.Begin();


            BlackFade.inst.FadeOut();
       }
    }

    public void CreateOverworldAndBattleUnits(){
        
        if(!GameManager.inst.loadFromFile)
        {
            List<Character> chars = new List<Character>();
            int howMany = 4;
            for (int i = 0; i < howMany; i++)
            {
                Character c = CharacterBuilder.inst.GenerateCharacter();
                chars.Add(c);
                
            }
            Party p = new Party();
            p.Create();
            PartyManager.inst.AddNewParty(p);
            PartyManager.inst.currentParty = p.ID;
            int w = 0;
           
            Dictionary<Vector2,Slot>  d =    MapManager.inst.map.GetPlayerStartingSlots();
            foreach (var item in chars)
            {
                w ++;
                p.FakePartyAdd(item,w);
                Unit u = UnitFactory.inst. CreatePlayerUnit(d[p.members[item.ID].battlePosition] ,item);
                OverworldUnit o = UnitFactory.inst.CreateOverworldUnit(item);
               
                u.overworldUnit = o;
                o.battleUnit = u;
                PartyController.inst.playerUnits.Add(o);
               
             
            }
           
            PartyController.inst.Organize();
            
            
        }
        else
        {

            Dictionary<Vector2,Slot>  d =    MapManager.inst.map.GetPlayerStartingSlots();
            foreach (var item in  PartyManager .inst.parties[PartyManager .inst.currentParty]. members)
            {
                Character c = item.Value.character;
                Unit u = UnitFactory.inst.CreatePlayerUnit(d[item.Value.battlePosition],item.Value.character);
                OverworldUnit o = UnitFactory.inst.CreateOverworldUnit(c);
                u.overworldUnit = o;
                o.battleUnit = u;
                PartyController.inst.playerUnits.Add(o);
                 //w ++;
            }
            PartyController.inst.Organize();
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
        if(MapTileManager.inst != null)
        {
            save.mapSaveData = MapTileManager.inst.Save();
        }
        else
        {
            save.mapSaveData =  SaveLoad.Load(saveSlotIndex).mapSaveData;
        }
        return save;
    }



}