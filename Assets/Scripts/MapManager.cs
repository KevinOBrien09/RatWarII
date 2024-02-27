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
    public SoundData enemySpawnHit;
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

    public void InitStartRoom(){
        currentRoom = map.startRoom;
        currentRoom.lockDown = false;
        foreach (var item in currentRoom.borders)
        {
            foreach (var d in item.doors)
            {
                d.metalFence.gameObject.SetActive(false);
            }
        }
        MusicManager.inst.ChangeMusic( MusicManager.inst.peace);
        HideInactiveRooms();
    }

    public void  ChangeRoom(Room newRoom,Door d)
    {
        currentRoom = newRoom;
       
        
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

            yield return new WaitForSeconds(.65f);
            CamFollow.inst.enabled = true;
            DoorInteractable di = d.currentSpecialSlot.interactable as DoorInteractable;
            AudioManager.inst.GetSoundEffect().Play(di.close);
            for (int i = 0; i < BattleManager.inst.playerUnits.Count; i++)
            {BattleManager.inst.playerUnits[i].graphic.gameObject.SetActive(true);}
            
            yield return new WaitForSeconds(.3f);

            if(currentRoom.roomContent == Room.Content.ENEMY)
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
            else if(currentRoom.roomContent == Room.Content.EMPTY)
            {
                BattleManager.inst.EndTurn();
            }
        }
    }

    public void LockDownRooms(){
        foreach (var item in currentRoom.borders)
        {
            foreach (var dr in item.doors)
            {
                dr.metalFence.gameObject.SetActive(true);
                dr.LockDown();
            }
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

    public void HideInactiveRooms(){
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