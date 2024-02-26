using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
    public class Border:MonoBehaviour
    {
        public  int borderID;
        public Room roomA,roomB;
        public List<Slot> slots = new List<Slot>();
        public List<Wall> walls = new List<Wall>();
        public List<Door> doors = new List<Door>();
        public void GetBorderSlots()
        {
            foreach (var slot in roomA.slots)
            {
                List<Slot> neighbours = slot.func.GetNeighbouringSlots();
                foreach (var neighbouringSlot in neighbours)
                {
                    if(neighbouringSlot.room == roomB)
                    {
                        if(!slots.Contains(neighbouringSlot))
                        {slots.Add(neighbouringSlot);}
                    }
                }
            }
        }
    }
