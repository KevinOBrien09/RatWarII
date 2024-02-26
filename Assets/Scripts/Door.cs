using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
public class Door : MonoBehaviour 
{
    public Border border;
    public Room roomA,roomB;
    public List<Slot> occupiedSlots = new List<Slot>();
    public SpecialSlot doorInteractablePrefab;
    public Transform doorHolder;
    public  List<Slot> triggerSlots = new List<Slot>();
    public void Init(Border b,Slot s1,Slot s2)
    {
        border = b;
        roomA = border.roomA;
        roomB = border.roomB;
        occupiedSlots.Add(s1);
        occupiedSlots.Add(s2);
   
        foreach (var item in occupiedSlots)
        {item.IsWall();}
        foreach (var item in occupiedSlots)
        {
            foreach (var s in  item.func.GetNeighbouringSlots())
            {
                if(!s.cont.wall)
                {
                    triggerSlots.Add(s);
                    SpecialSlot ss =  Instantiate(doorInteractablePrefab,s.transform.position,Quaternion.identity);
                    DoorInteractable di = ss.interactable as DoorInteractable;
                    di.door = this;
                    s.MakeSpecial(ss);
                    
                }
            }
        }
        List<Slot> XD = new List<Slot>();
        XD.Add(triggerSlots[0]);

        foreach (var item in triggerSlots)
        {
            if(item != triggerSlots[0] && item.room == triggerSlots[0].room)
            {XD.Add(item);}
        }

        if(XD[0].node.iGridX != XD[1].node.iGridX)
        {RotateDoor(0);}
        else
        {RotateDoor(90);}

    }

    public void RotateDoor(float y){
        doorHolder.localRotation = Quaternion.Euler(new Vector3(0,y,0));
    }

    

}