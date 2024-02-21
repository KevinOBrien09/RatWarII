using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour 
{
    public Grid_ grid;
    public Slot slotPrefab;
    public List<Slot> slots = new List<Slot>();

    public void Create(string roomName)
    {
        grid.CreateGrid();
        foreach(var item in grid.NodeArray)
        {
            Slot s = Instantiate(slotPrefab,item.vPosition,Quaternion.identity);
            item.slot = s;
            s.node = item;
            s.gameObject.name = roomName + "|" + item.iGridX + ":" + item.iGridY;
            slots.Add(s);
            s.transform.SetParent(transform);
            if(item.isBlocked)
            {s.IsWall(); }
        }
     
       
        grid.UpdateGrid();
    }
}