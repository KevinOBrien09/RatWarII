using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class MapManager : Singleton<MapManager>
{
    public AStar aStar;
    public Grid_ grid;
    public Slot slotPrefab;
    public List<Slot> slots = new List<Slot>();
    

    public List<Slot> fuckYouSlots = new List<Slot>();
  

   public void SpawnSlots()
    {
        GameObject g = new GameObject("grid");
        g.transform.SetParent(grid.transform);
        foreach(var item in grid.NodeArray)
        {
            Slot s = Instantiate(slotPrefab,item.vPosition,Quaternion.identity);
            item.slot = s;
            s.node = item;
            s.gameObject.name = item.iGridX + ":" + item.iGridY;
            slots.Add(s);
            s.transform.SetParent(g.transform);
            if(item.isBlocked)
            {s.IsWall(); }
        }
     
       
        grid.UpdateGrid();
      
       
    }

    public bool nodeIsValid(Vector2 v)
    {
        int maxX = MapManager.inst.grid.iGridSizeX-1;
        int maxY = MapManager.inst.grid.iGridSizeY-1;

        bool inXRange = v.x <= maxX && v.x >= 0;
        bool inYRange = v.y <= maxY && v.y >= 0;
       bool valid = inXRange && inYRange;
       return valid;
    }

    public List<Slot> StartingRadius()
    {
        Slot center = grid.NodeArray[2,2].slot;
        List<Slot> radius =  center.func.GetRadiusSlots(1,null,true);
       
        System.Random rng = new System.Random();
        return radius.OrderBy(_ => rng.Next()).ToList();

    }

    public List<Slot> HostageSlots()
    {
        Slot center = grid.NodeArray[2,2].slot;
        List<Slot> radius =  center.func.GetRadiusSlots(1,CharacterBuilder.inst.mandatorySkills[0],false);
        radius.Add(center);
        System.Random rng = new System.Random();
        return radius.OrderBy(_ => rng.Next()).ToList();

    }

    public Slot RandomSlot()
    {
        List<Slot> Xd = new List<Slot>(slots);
       
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