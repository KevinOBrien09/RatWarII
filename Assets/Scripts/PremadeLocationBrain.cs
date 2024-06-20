using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using DG.Tweening;



public class PremadeLocationBrain : MapGeneratorBrain
{
    public PremadeLocation level;
    public Slot slotPrefab;
    public Objective.ObjectiveEnum objective;
    public override void Generate(LocationInfo li = null)
    {
        // PremadeLocation pm = Instantiate(level,MapManager.inst.transform);
        GameManager.inst.LoadQuest(objective);
    
        MapGenerator.inst.WrapUp();
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
        s.mesh.gameObject.SetActive(false);
    }
}