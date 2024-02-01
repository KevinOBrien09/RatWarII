using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
            if(!item.isBlocked){
                Slot s = Instantiate(slotPrefab,item.vPosition,Quaternion.identity);
                item.slot = s;
                s.node = item;
                s.gameObject.name = item.iGridX + ":" + item.iGridY;
                slots.Add(s);
                s.transform.SetParent(g.transform);
            }
           
        }
     
       
        grid.UpdateGrid();
      
       
    }
}