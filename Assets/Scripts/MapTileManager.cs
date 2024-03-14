using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class LocationInfo{
    public string locationName;
    public LocationStage stage;
    public List<Character> characters = new List<Character>();
    public WorldMapTile mapTile;
    [TextArea(10,10)] public string desc;


 
}

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
    public bool unlocked;

    public void AlterWithSave(StageSave ss){
        unlocked = ss.unlocked;
    }

    public StageSave Save(){
        StageSave ss = new StageSave();
        ss.unlocked = unlocked;
    

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
  
    public void Start()
    {
        foreach (var item in mapTiles)
        { ld.Add(item.locationInfo.stage.GetID(),item); }


        if(loadFromFile)
        {
            SaveData sd =  SaveLoad.Load(999);
            MapSaveData msd = sd.mapSaveData;
            foreach (var item in msd.stages)
            { ld[item.Key].locationInfo. stage.AlterWithSave(item.Value); }
        }
        else
        {
            if(goFromStart)
            {
                foreach (var item in ld)
                { item.Value.locationInfo.stage.unlocked = false;}

                ld[new Vector2((int) Location.CITY,1)].locationInfo.stage.unlocked = true;
                ld[new Vector2((int) Location.NORTH,1)].locationInfo.stage.unlocked = true;
                ld[new Vector2((int) Location.EAST,1)].locationInfo.stage.unlocked = true;
                ld[new Vector2((int) Location.WEST,1)].locationInfo.stage.unlocked = true;
                ld[new Vector2((int) Location.SOUTH,1)].locationInfo.stage.unlocked = true;
              
            }
            
        }

        foreach (var item in mapTiles)
        {
            item.Refresh();
        }
    
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