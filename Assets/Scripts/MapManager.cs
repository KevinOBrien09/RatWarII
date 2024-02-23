using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class MapManager : Singleton<MapManager>
{

    public Room currentRoom;
    public Transform gridHolder;
    public List<Room> rooms = new List<Room>();
    public Room roomPrefab;
    public Door doorPrefab;
    float padding =5;
    
    public Vector3 SpawnRoom(DungeonNode dungeonNode)
    {
        string roomName = "Room:" + rooms.Count;
        Room room = Instantiate(roomPrefab);
        room.transform.position = dungeonNode.transform.position;
        room.transform.SetParent(gridHolder);
        room.Create(roomName,dungeonNode.roomType);
        float yC = room.grid.CenterNode().vPosition.z-5;
        float xC = room.grid.NodeArray[room.grid.iGridSizeX-1,0].vPosition.x+5;
        Vector3 d = new Vector3(xC,room.transform.position.y,yC );
        // Door door = Instantiate(doorPrefab,d,Quaternion.identity);
        // door.MakeDoor(room.grid);
        rooms.Add(room);

        return new Vector3(xC,room.transform.position.y,room.transform.position.z);
     
      
       
    }

    public void ChangeRoom(Room newRoom){
       
        currentRoom = newRoom;
       
    }

    public bool slotBelongsToGrid(Slot s)
    {
        return currentRoom.slots.Contains(s);
    }

    public bool nodeIsValid(Vector2 v)
    {
        int maxX = MapManager.inst.currentRoom.grid.iGridSizeX-1;
        int maxY = MapManager.inst.currentRoom.grid.iGridSizeY-1;

        bool inXRange = v.x <= maxX && v.x >= 0;
        bool inYRange = v.y <= maxY && v.y >= 0;
       bool valid = inXRange && inYRange;
       return valid;
    }

    public List<Slot> StartingRadius()
    {
        Slot center = currentRoom.grid.NodeArray[2,2].slot;
        List<Slot> radius =  center.func.GetRadiusSlots(1,null,true);
       
        System.Random rng = new System.Random();
        return radius.OrderBy(_ => rng.Next()).ToList();

    }

    public List<Slot> HostageSlots()
    {
        Slot center = currentRoom.grid.NodeArray[2,2].slot;
        List<Slot> radius =  center.func.GetRadiusSlots(1,CharacterBuilder.inst.mandatorySkills[0],false);
        radius.Add(center);
        System.Random rng = new System.Random();
        return radius.OrderBy(_ => rng.Next()).ToList();

    }

    public Slot RandomSlot()
    {
        List<Slot> Xd = new List<Slot>(currentRoom.slots);
       
        System.Random rng = new System.Random();
        
        foreach (var item in Xd.OrderBy(_ => rng.Next()).ToList())
        {
            if(item.cont.specialSlot == null){
if(item.cont.walkable()){
                return item;
            }
            }
            
            
        }

        return null;

        
    }
}