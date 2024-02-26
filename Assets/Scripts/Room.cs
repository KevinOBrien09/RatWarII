using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[System.Serializable]
public class Room : MonoBehaviour
{
    public int roomID;
    public GenerationRoomType roomType;
    public List<Slot> slots = new List<Slot>();
    public List<Border> borders = new List<Border>();
    public List<Room> borderRooms = new List<Room>();
    TextMeshPro tmp;
    public GenericDictionary<Direction,List<Wall>> wallDict = new GenericDictionary<Direction, List<Wall>>();
    public void Init(int i)
    {
        wallDict.Add(Direction.UP,new List<Wall>());
        wallDict.Add(Direction.DOWN,new List<Wall>());
        wallDict.Add(Direction.RIGHT,new List<Wall>());
        wallDict.Add(Direction.LEFT,new List<Wall>());
        tmp = transform.Find("Text").GetComponent<TextMeshPro>();
        tmp.text = i.ToString();
        roomID = i;
        transform.name =  "Room :" + i;
        transform.SetParent(MapManager.inst.map.transform);

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

    // public void GetBorderRooms()
    // {
    //     foreach (var item in slots)
    //     {
    //         foreach (var slot in item.func.GetNeighbouringSlots())
    //         {
    //             if(slot.room != this)
    //             {
    //                 if(!borderRooms.Contains(slot.room))
    //                 {
    //                     borderRooms.Add(slot.room);
    //                 }
    //             }
    //         }  
    //     }
    // }
  
}