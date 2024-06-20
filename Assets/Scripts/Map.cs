using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using NavMeshPlus.Components;
public class Map : Grid_
{
    public List<Room> rooms = new List<Room>();
    public Room startRoom,endRoom;
    public NavMeshSurface surface;
    public void AssignStartEnd(Room a, Room b){
        startRoom = a;
        endRoom = b;
    }

    public Room RandomRoom(){
        return rooms[UnityEngine. Random.Range(0,rooms.Count)];
    }
}