using System.Collections;
using System.Collections.Generic;
using UnityEngine;



[System.Serializable]
public class MapSaveData
{
    public Vector2 lastLocation;
   public GenericDictionary<Vector2,StageSave> stages = new GenericDictionary<Vector2,StageSave>();
}

[System.Serializable]
public class LocationStage{
    public Location loc;
    public int stage;
  

    public void AlterWithSave(StageSave ss){
      
    }

    public StageSave Save(){
        StageSave ss = new StageSave();
       
    

        return ss;
    }

    public Vector2 GetID()
    {
        Vector2 v = new Vector2();

        v.x = (int)loc;
        v.y = stage;
        return v;
    }
}
[System.Serializable]
public class StageSave{
  
  public bool unlocked;
}




public enum Location{CITY,NORTH,SOUTH,EAST,WEST}
public class MapTileManager : Singleton<MapTileManager>
{
    public bool debug,loadFromFile,goFromStart;
    public List<WorldMapTile> mapTiles = new List<WorldMapTile>();
    public GenericDictionary<Vector2,WorldMapTile> ld = new GenericDictionary<Vector2,WorldMapTile>();
  
   

    public void Init(){

        foreach (var item in mapTiles)
        { ld.Add(item.locationInfo.stage.GetID(),item); }


        if(loadFromFile)
        {
            SaveData sd =  SaveLoad.Load(GameManager.inst.saveSlotIndex);
            MapSaveData msd = sd.mapSaveData;
       
            foreach (var item in msd.stages)
            { 
                if(ld.ContainsKey(item.Key))
                { ld[item.Key].locationInfo. stage.AlterWithSave(item.Value);

                }
                else{
                    Debug.LogWarning("CANNOT FIND MAP TILE ID : " + item.Key);
                }
               
            }
        }
        else
        {
            if(goFromStart)
            {
              

            
              
              
            }
            
        }

        foreach (var item in mapTiles)
        {
            item.Refresh();
        }
    }

    public void RefreshCurrentLoc()
    {
        foreach (var l in ld)
        {l.Value.ToggleCurrentLocIndic(false);}
        ld[LocationManager.inst.currentLocation].ToggleCurrentLocIndic(true);
    }

   
    public MapSaveData Save()
    {
        MapSaveData msd = new MapSaveData();
        msd.lastLocation = LocationManager.inst.currentLocation;
        foreach (var l in ld)
        {
            StageSave ss =  l.Value.locationInfo.stage.Save();
            msd.stages.Add(l.Key,ss);
           
        }
        return msd;

    }
}