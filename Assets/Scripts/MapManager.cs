using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;


public class MapManager : Singleton<MapManager>
{
    public BattleArenaManager map;
    public List<Slot> allSlots = new List<Slot>();
    public List<Slot> intrustiveSlotsCurrentlyMadeTransparent = new List<Slot>();
    public Room currentRoom;
    public SoundData enemySpawnHit;
    public bool doNotSpawnEnemies;
    public List<Slot> premadeSlots;
    public Queue<Slot> premadeSlotQ = new Queue<Slot>();
    public List<Wall> premadeWalls;
    public Queue<Wall> premadeWallQ = new Queue<Wall>();
    public void QPremadeSlots()
    {
        foreach (var item in premadeSlots)
        {premadeSlotQ.Enqueue(item);}

        foreach (var item in premadeWalls)
        {premadeWallQ.Enqueue(item);}
    }
    public bool canGivePremadeSlot(){
        return premadeSlotQ.Count > 0;
    }
      public bool canGivePremadeWall(){
        return premadeWallQ.Count > 0;
    }
    public Slot GivePremade()
    {
        return premadeSlotQ.Dequeue();
    }
    public Wall GiveWall()
    {
        return premadeWallQ.Dequeue();
    }

    public void InitStartRoom()
    {
        currentRoom = map.startRoom;
        SetDoors();
        currentRoom.lockDown = false;
        foreach (var item in currentRoom.borders)
        {
            foreach (var d in item.doors)
            {d.metalFence.gameObject.SetActive(false); }
        }
        currentRoom.roomClear = true;
        MusicManager.inst.ChangeMusic( MusicManager.inst.peace);
  
   
    }

    public void SetDoors(){
    foreach (var border in   currentRoom.borders)
        {
            foreach (var door in border.doors)
            {
                foreach (var doorSlot in door.occupiedSlots)
                {
                    Slot s = doorSlot;
                    s.room.slots.Remove(s);
                    currentRoom.slots.Add(s);
                    doorSlot.room = currentRoom;
                }
            }
        }
    }

   


    public void CheckForIntrusions()
    {
        foreach (var item in allSlots)
        {
            item.DealWithSurrondingIntrusions();
        }

    }

    public void OpenRoomsFromLockdown(){
        foreach (var item in currentRoom.borders)
        {
            foreach (var dr in item.doors)
            {
                dr.metalFence.gameObject.SetActive(true);
                dr.OpenFromLockDown();
            }
        }
    }

  

    public bool slotBelongsToGrid(Slot s) //
    {
       
        return true;
        
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
        Collider[] c = Physics.OverlapSphere(MapManager.inst.map.startRoom.RandomSlot().transform.position,3);
        Slot s = null;
        foreach (var item in c)
        {
            if(item.TryGetComponent<Slot>(out s))
            {
                if(!s.isWater){
                    break;
                }
              
            }
        }
        if(s == null)
        {s = MapManager.inst.map.startRoom.RandomSlot();}
    
        List<Slot> radius = s.func.GetRadiusSlots(2,CharacterBuilder.inst.mandatorySkills[0],false);
        if(!s.isWater){
             radius.Add(s);
        }
       
        foreach (var item in MapManager.inst.map.startRoom.slots)
        {
            if(!radius.Contains(item)){
                radius.Add(item);
            }
        }

        List<Slot> filter = new List<Slot>(radius);
        foreach (var item in filter)
        {
            BoatSlot bs = item as BoatSlot;
            if(item.isWater|item.isBoat | bs != null){
                radius.Remove(item);
            }
            
        }

        return radius;

    }

    public List<Slot> HostageSlots()
    {
      
        Collider[] c = Physics.OverlapSphere(MapManager.inst.map.startRoom.transform.position,3);
        Slot s = null;
        foreach (var item in c)
        {
            if(item.TryGetComponent<Slot>(out s))
            {break;}
        }
        if(s == null)
        {s = MapManager.inst.map.startRoom.RandomSlot();}
    
        List<Slot> radius = s.func.GetRadiusSlots(2,CharacterBuilder.inst.mandatorySkills[0],false);
        radius.Add(s);
        System.Random rng = new System.Random();
        return radius.OrderBy(_ => rng.Next()).ToList();

    }

    public Slot RandomSlot()
    {
        List<Slot> Xd = new List<Slot>(allSlots);
       
        System.Random rng = new System.Random();
        
        foreach (var item in Xd.OrderBy(_ => rng.Next()).ToList())
        {
            if(item.cont.specialSlot == null)
            {
                if(item.cont.walkable() && item.cont.unit == null && !item.marked){
                    return item;
                }
            }
        }
        return null;
    }
}