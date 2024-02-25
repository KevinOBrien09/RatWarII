using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class MapManager : Singleton<MapManager>
{

    public Map map;
    public List<Slot> allSlots = new List<Slot>();
    public Transform gridHolder;
    public Room currentRoom;
    public Room roomPrefab;
    public Door doorPrefab;
    float padding =5;
    
    // public void SpawnRoom(DungeonNode dungeonNode)
    // {
    //     string roomName = "Room:" + rooms.Count;
    //     Room room = Instantiate(roomPrefab);
    //     room.transform.name = roomName;
    //     room.transform.position = dungeonNode.spawnPoint.position;
    //     room.transform.SetParent(gridHolder);
    //     //room.Create(roomName,dungeonNode.roomType);
    //     rooms.Add(room);

       
      
       
    // }

    public void ChangeRoom(Room newRoom)
    {
       
        currentRoom = newRoom;
        foreach (var item in map.rooms)
        {
            foreach (var s in item.slots)
            {  s.ActivateAreaIndicator(new Color32(0,0,0,200));
                s.border.gameObject.SetActive(false);
                s.dormant = true;
            }
              
        }
        foreach (var s in currentRoom.slots)
        {
            s.DectivateAreaIndicator();
            s.border.gameObject.SetActive(true);
            s.dormant = false;
        }
       
    }

    public bool slotBelongsToGrid(Slot s)
    {
        return currentRoom.slots.Contains(s);
    }

    public bool nodeIsValid(Vector2 v)
    {
        int maxX =  MapManager.inst.map.iGridSizeX-1;
        int maxY = MapManager.inst.map.iGridSizeY-1;

        bool inXRange = v.x <= maxX && v.x >= 0;
        bool inYRange = v.y <= maxY && v.y >= 0;
       bool valid = inXRange && inYRange;
       return valid;
    }

    public List<Slot> StartingRadius()
    {
        Slot center = MapManager.inst.map.NodeArray[2,2].slot;
        List<Slot> radius =  new List<Slot>();
        for (int i = 0; i < 5; i++)
        {
            radius.Add(RandomSlot());
        }
        //center.func.GetRadiusSlots(1,null,true);
       
        System.Random rng = new System.Random();
        return radius.OrderBy(_ => rng.Next()).ToList();

    }

    public List<Slot> HostageSlots()
    {
        Slot center =MapManager.inst.map.NodeArray[2,2].slot;
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