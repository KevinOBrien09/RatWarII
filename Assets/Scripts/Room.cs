using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;

public class EnemySpawnData{
    public DefinedCharacter enemy;
    public Slot spawnSlot;
}

[System.Serializable]
public class Room : MonoBehaviour
{
    public enum RoomAnchors{BOT_LEFT,TOP_LEFT,MIDDLE,BOT_RIGHT,TOP_RIGHT}
    public enum Content{EMPTY, ENEMY}
    public int roomID;
    public GenerationRoomType roomType;
    public List<Slot> slots = new List<Slot>();
    public List<Border> borders = new List<Border>();
    public List<Room> borderRooms = new List<Room>();
    TextMeshPro tmp;
    public bool lockDown,roomClear;
    public Room.Content roomContent;
    public List<EnemySpawnData> enemySpawnData = new List<EnemySpawnData>();
    public GenericDictionary<Direction,List<Wall>> wallDict = new GenericDictionary<Direction, List<Wall>>();
    bool contentMade;
    public GenericDictionary<RoomAnchors,Slot> anchors = new GenericDictionary<RoomAnchors, Slot>();
    public void Init(int i)
    {
        wallDict.Add(Direction.UP,new List<Wall>());
        wallDict.Add(Direction.DOWN,new List<Wall>());
        wallDict.Add(Direction.RIGHT,new List<Wall>());
        wallDict.Add(Direction.LEFT,new List<Wall>());
      
        Transform t = transform.Find("Text");
        if(t != null){
            if(t.TryGetComponent<TextMeshPro>(out tmp)){
            tmp.text = i.ToString();
        }
        }
        
      
        roomID = i;
        transform.name =  "Room :" + i;
        transform.SetParent(MapManager.inst.map.transform);
        
    }

    public void SortAnchors(){
        if(slots.Count > 0){
        anchors.Add(RoomAnchors.BOT_LEFT,slots[0]);
        int truCount = slots.Count-1;
        anchors.Add(RoomAnchors.MIDDLE,slots[(int) truCount/2]);
        anchors.Add(RoomAnchors.TOP_RIGHT,slots[truCount]);
        }
        else{
            Debug.LogWarning("Room has no slots!");
        }
    
    }
    
    public void AddRoomContent()
    {
        if(roomType == GenerationRoomType.SIDEHALL | roomType == GenerationRoomType.VERTHALL)
        {roomContent = Room.Content.EMPTY;
        roomClear = true;}
        else
        {AddEnemies();}
    }


    void AddEnemies(){
        roomContent = Room.Content.ENEMY;
        int howManyEnemies = 0;
        switch (roomType)
        {
            case GenerationRoomType.NORMAL:
            howManyEnemies = 3;
            break;
            case GenerationRoomType.BIG:
            howManyEnemies = 9;
            break;
            case GenerationRoomType.HORIDOUBLE:
            howManyEnemies = 6;
            break;
            case GenerationRoomType.VERTDOUBLE:
            howManyEnemies = 6;
            break;
            
            default:
            howManyEnemies = 0;
            roomClear = true;
            Debug.LogAssertion("DEFAULT CASE!!!");
            break;
        }
        List<Slot> enemySlots = new List<Slot>();
        List<Slot> Xd = new List<Slot>(slots);
       
        System.Random rng = new System.Random();
        foreach (var item in Xd.OrderBy(_ => rng.Next()).ToList())
        {
            if(item.cont.specialSlot == null)
            {
                if(item.cont.walkable() && !item.marked)
                {enemySlots.Add(item); }
            }
        }

        for (int i = 0; i < howManyEnemies; i++)
        {
            EnemySpawnData esd = new EnemySpawnData();
            esd.enemy = UnitFactory.inst.enemies[Random.Range(0, UnitFactory.inst.enemies.Count)];
            if(enemySlots.ElementAtOrDefault(i) == null){
Debug.LogAssertion("THIS SHIT AGAIN JUST GONNA FUCKING RESET");
MapGenerator.inst.brain.Reset();
            }
            esd.spawnSlot = enemySlots[i];
            enemySpawnData.Add(esd);
        }
    }

    public void GetBorderRooms()
    {
        foreach (var item in slots)
        {
            foreach (var slot in item.func.GetNeighbouringSlots())
            {
                if(slot.room != this)
                {
                    if(!borderRooms.Contains(slot.room))
                    {
                        borderRooms.Add(slot.room);
                    }
                }
            }  
        }
    }


    public Slot RandomSlot()
    {
        List<Slot> Xd = new List<Slot>(slots);
       
        System.Random rng = new System.Random();
        
        foreach (var item in Xd.OrderBy(_ => rng.Next()).ToList())
        {
            if(item.cont.specialSlot == null){
                if(item.cont.walkable() && !item.marked)
                {
                    return item;
                }
            }
            
            
        }

        return null;

        
    }

   
  
}