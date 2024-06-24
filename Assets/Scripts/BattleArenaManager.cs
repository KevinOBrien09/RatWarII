    using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using DG.Tweening;



public class BattleArenaManager : Map
{
    public Slot slotPrefab;

    public void Generate()
    {
      
        // MapManager.inst.map.aStar = pm.aStar;
     //   MapManager.inst.star
        CreateGrid();
        foreach (var item in  MapManager.inst.map.NodeArray)
        {
            CreateSlot(item,  MapManager.inst.map.startRoom);
        }
    }

    void CreateSlot(Node item, Room r)
    {
        Slot s = Instantiate(slotPrefab,item.vPosition,Quaternion.identity);
        r.slots.Add(s);
        s.gameObject.name = r.roomID +"|"+r.slots.Count;
        s.room = r;
        MapManager.inst.allSlots.Add(s);
        item.slot = s;
        s.node = item;
        s.transform.SetParent(r.transform);
     //   s.mesh.gameObject.SetActive(false);
    }

    public Dictionary<Vector2,Slot> GetStartingSlots()
    {
        Dictionary<Vector2,Slot> d = new Dictionary<Vector2, Slot>();
        int X = iGridSizeX/2;
        
        d.Add(new Vector2(1,0),NodeArray[X-1,2].slot);
        d.Add(new Vector2(2,0),NodeArray[X,2].slot);
        d.Add(new Vector2(3,0),NodeArray[X+1,2].slot);
      
        d.Add(new Vector2(0,1),NodeArray[X-2,1].slot);
        d.Add(new Vector2(1,1),NodeArray[X-1,1].slot);
        d.Add(new Vector2(2,1),NodeArray[X,1].slot);
        d.Add(new Vector2(3,1),NodeArray[X+1,1].slot);
        d.Add(new Vector2(4,1),NodeArray[X+2,1].slot);

     
        d.Add(new Vector2(0,2),NodeArray[X-2,0].slot);
        d.Add(new Vector2(1,2),NodeArray[X-1,0].slot);
        d.Add(new Vector2(2,2),NodeArray[X,0].slot);
        d.Add(new Vector2(3,2),NodeArray[X+1,0].slot);
        d.Add(new Vector2(4,2),NodeArray[X+2,0].slot);

        
        return d;
    }
}