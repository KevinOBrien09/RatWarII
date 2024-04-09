using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocationManager : Singleton<LocationManager>
{
    
    public Vector2 currentLocation;
    public LocationInfo locationTravelingTo;

    public string locName;
    public bool inTravel;
    public bool INSTANT_TRAVEL;

    public void Wipe(){
        currentLocation = Vector2.zero;
        locationTravelingTo = null;
       
        locName = string.Empty;
        inTravel = false;
    }
  
    public bool BeginTravel(LocationInfo i){

        if(i.brain != null){
            locationTravelingTo = i;
            inTravel = true;
           
            return true;
        }
        else{
            Debug.LogAssertion("MAP DATA IS NULL!!!");
            return false;
        }
       
  
    }
    public void Transfer(){
           currentLocation = locationTravelingTo.stage.GetID();
        if(PartyManager.inst.currentParty != string.Empty){
            locName = MapTileManager.inst.ld[currentLocation].locationInfo.locationName;
            PartyManager.inst.parties[PartyManager.inst.currentParty].ChangeMapLocation(    currentLocation );
            HubStateHandler.inst.   ChangeLocationName(locName);
            HubCharacterDisplay.inst.Refresh();
            MapTileManager.inst.RefreshCurrentLoc();
            HubManager.inst.SpawnNewDeco(MapTileManager.inst.ld[LocationManager.inst.currentLocation].locationInfo);
            
        }   
        else{
            Debug.LogAssertion("NO PARTY IS BEING MOVED!!");
        }
        inTravel = false;
    }

    public void Visit(Vector2 v){
        BlackFade.inst.FadeInEvent(()=>{
            currentLocation = v;
            PartyManager.inst.XD(LocationManager.inst.currentLocation);
            locName = MapTileManager.inst.ld[currentLocation].locationInfo.locationName;
            HubStateHandler.inst.   ChangeLocationName(locName);
            HubCharacterDisplay.inst.Refresh();
           
            HubStateHandler.inst.close = null;
            HubStateHandler.inst.RetunToHover();
          
            MapTileManager.inst.RefreshCurrentLoc();
            HubManager.inst.SpawnNewDeco(MapTileManager.inst.ld[LocationManager.inst.currentLocation].locationInfo);
            StartCoroutine(q());
            IEnumerator q(){
                yield return new WaitForSeconds(1);
                       HubManager.inst.MapToHub();
            }
         
        });
       
    }

    public bool LocationHasCharacters(Vector2 id)
    {
        foreach (var item in PartyManager.inst.parties)
        {
           if(item.Value.mapTileID == id && item.Value.members.Count > 0)
           {return true;}
        }

        foreach (var item in PartyManager.inst.benched)
        {
            if(item.Value.mapTileID == id)
            {return true;}
        }

        return false;
    }

    public List<Vector2> adjacentLocations(Vector2 id)
    {
        List<Vector2> vector2s = new List<Vector2>();

        Vector2 n1 = new Vector2(1,1);
        Vector2 s1 = new Vector2(2,1);
        Vector2 e1 = new Vector2(3,1);
        Vector2 w1 = new Vector2(4,1);
        if(id == new Vector2(0,0))
        {
            vector2s.Add(n1);
            vector2s.Add(s1);
            vector2s.Add(e1);
            vector2s.Add(w1);
        }
        else
        {
            float y = id.y;
            if(y == 1)
            {
                Debug.Log("First Tile");
                Vector2 forwardTile = new Vector2(id.x,id.y+1);
                Vector2 cityTile = Vector2.zero;
                
                if(MapTileManager.inst.ld.ContainsKey(forwardTile))
                {vector2s.Add(forwardTile);}

                if(MapTileManager.inst.ld.ContainsKey(cityTile))
                {vector2s.Add(cityTile);}
            }
            else
            {
                Vector2 backwardTile = new Vector2(id.x,y-1);
                Vector2 forwardTile = new Vector2(id.x,id.y+1);
                
                if(MapTileManager.inst.ld.ContainsKey(forwardTile))
                {vector2s.Add(forwardTile);}

                if(MapTileManager.inst.ld.ContainsKey(backwardTile))
                {vector2s.Add(backwardTile);}
            }
        }
        return vector2s;
    }

    public Location GetRelativeDirection(Vector2 A,Vector2 B)
    {
        if(A == Vector2.zero)
        {return MapTileManager.inst.ld[B].locationInfo.stage.loc;}
        else
        {
            float y = A.y;
            if(y == 1)
            {
                if(MapTileManager.inst.ld[B].locationInfo.stage.loc == Location.CITY)
                {
                    switch (MapTileManager.inst.ld[A].locationInfo.stage.loc )
                    {
                        case Location.NORTH:
                        return Location.SOUTH;

                        case Location.SOUTH:
                        return Location.NORTH;

                        case Location.EAST:
                        return Location.WEST;

                        case Location.WEST:
                        return Location.EAST;
                    }
                }
                return MapTileManager.inst.ld[B].locationInfo.stage.loc;
            }
            else
            {
                Vector2 backwardTile = new Vector2(A.x,A.y-1);
                Vector2 forwardTile = new Vector2(A.x,A.y+1);

                if(MapTileManager.inst.ld[B].locationInfo.stage.GetID() == forwardTile)
                {return MapTileManager.inst.ld[B].locationInfo.stage.loc;}
                else if(MapTileManager.inst.ld[B].locationInfo.stage.GetID() == backwardTile)
                {
                    switch (MapTileManager.inst.ld[B].locationInfo.stage.loc)
                    {
                        case Location.NORTH:
                        return Location.SOUTH;

                        case Location.SOUTH:
                        return Location.NORTH;

                        case Location.EAST:
                        return Location.WEST;

                        case Location.WEST:
                        return Location.EAST;
                    }
                }
            }
        }
        return Location.NORTH;
    }

    public bool TileHasCharacterAdjacentToIt(Vector2 id)
    {
        Vector2 n1 = new Vector2(1,1);
        Vector2 s1 = new Vector2(2,1);
        Vector2 e1 = new Vector2(3,1);
        Vector2 w1 = new Vector2(4,1);
        if(currentLocation == new Vector2(0,0))
        {
            if(id == n1 |id == s1|id == e1|id == w1 )
            {
            
                return true;
            }

        }
        else{
            Vector2 backwardTile = new Vector2(id.x,id.y-1);
            Vector2 forwardTile = new Vector2(id.x,id.y-1);
            if(LocationManager.inst.currentLocation == backwardTile | LocationManager.inst.currentLocation == forwardTile){
                return true;
            }
        }


        return false;
    }

}