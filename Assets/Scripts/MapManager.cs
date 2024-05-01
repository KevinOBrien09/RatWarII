using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public enum MapQuirk{NONE,ROOMS,ISLANDS}
public class MapManager : Singleton<MapManager>
{
    public Map map;
    public List<Slot> allSlots = new List<Slot>();
    public List<Slot> intrustiveSlotsCurrentlyMadeTransparent = new List<Slot>();
    public Room currentRoom;
    public SoundData enemySpawnHit;
    public bool doNotSpawnEnemies;
    public MapQuirk mapQuirk;
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
  
        HideInactiveRooms();
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

    public void  ChangeRoom(Room newRoom,Door d)
    {
        currentRoom = newRoom;
       SetDoors();
       
        for (int i = 0; i < BattleManager.inst.playerUnits.Count; i++)
        {
            Slot s = d.landingSlots[newRoom][i];
            Unit u = BattleManager.inst.playerUnits[i];
            u.Reposition(s);
            Vector3 v =  new Vector3(s.transform.position.x,u. transform.position.y ,s.transform.position.z);
            u.transform.position = v;

        }
        HideInactiveRooms();
        StartCoroutine(q());
        IEnumerator q()
        {
            if(ObjectiveManager.inst.objective.objectiveEnum == Objective.ObjectiveEnum.HOSTAGE)
            {
                if(ObjectiveManager.inst.hostageInPlayerPossession())
                {
                    if(currentRoom == map.startRoom)
                    {
                        foreach (var item in HostageSlots())
                        {
                            item.ActivateAreaIndicator( new Color32(0,255,255,55));
                        }
                    }
                 
                }
         
            }

            yield return new WaitForSeconds(.65f);
            CamFollow.inst.enabled = true;
            DoorInteractable di = d.currentSpecialSlot.interactable as DoorInteractable;
            AudioManager.inst.GetSoundEffect().Play(di.close);
            for (int i = 0; i < BattleManager.inst.playerUnits.Count; i++)
            {BattleManager.inst.playerUnits[i].graphic.gameObject.SetActive(true);}
            
            yield return new WaitForSeconds(.3f);
           

            
            if(currentRoom.roomContent == Room.Content.ENEMY)
            {
                if(!currentRoom.roomClear && !doNotSpawnEnemies)
                {
                    CamFollow.inst.Focus(currentRoom.transform,(()=>
                    {
                    
                        StartCoroutine(spawnEnemies());
                        IEnumerator spawnEnemies()
                        {  
                        
                            LockDownRooms();
                            BattleManager.inst.roomLockDown = true;
                            BattleTicker.inst.Type("Enemies have appeared!");
                            foreach (var item in currentRoom.enemySpawnData)
                            {UnitFactory.inst. CreateEnemyUnit(item.spawnSlot,item.enemy);}
                            
                            AudioManager.inst.GetSoundEffect().Play(enemySpawnHit);
                            MusicManager.inst.ChangeMusic( MusicManager.inst.battle);
                            yield return new WaitForSeconds(1f);
                            BattleManager.inst.ResetTurnOrder();
                            BattleManager.inst.EndTurn();
                        }
                            
                        
                    }));
                }
                else{
                    currentRoom.roomClear = true;
                    BattleManager.inst.EndTurn();
                }
            }
            else if(currentRoom.roomContent == Room.Content.EMPTY)
            {
                BattleManager.inst.EndTurn();
            }
        }
    }

    public void LockDownRooms(){
         if(mapQuirk == MapQuirk.ROOMS){
        foreach (var item in currentRoom.borders)
        {
            foreach (var dr in item.doors)
            {
                dr.metalFence.gameObject.SetActive(true);
                dr.LockDown();
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

    public void HideInactiveRooms()
    {
        if(mapQuirk == MapQuirk.ROOMS){
foreach (var item in map.rooms)
        {
            foreach (var s in item.slots)
            {  s.ActivateAreaIndicator(new Color32(0,0,0,200));
                s.border.gameObject.SetActive(false);
                    s.DisableHover();
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
        
    }

    public bool slotBelongsToGrid(Slot s)
    {
        if(mapQuirk == MapQuirk.ROOMS){
        return currentRoom.slots.Contains(s); 
        }
        else{
            return true;
        }
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