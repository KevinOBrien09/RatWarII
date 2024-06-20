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
}