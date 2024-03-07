using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using DG.Tweening;
public class Door : MonoBehaviour 
{
    public Border border;
    public Room roomA,roomB;
    public List<Slot> occupiedSlots = new List<Slot>();
    public SpecialSlot doorInteractablePrefab;
    public GameObject placeHolder;
   public List<DoorInteractable> doorInteractables = new List<DoorInteractable>();
    public  List<Slot> triggerSlots = new List<Slot>();

    public Dictionary<Room,List<Slot>> landingSlots = new Dictionary<Room, List<Slot>>();
    public GameObject metalFence;
    public SpecialSlot currentSpecialSlot;
    public bool spawnVisual;

    public void LockDown(){
        float y = metalFence.transform.localPosition.y;
        metalFence.gameObject.SetActive(true);
        metalFence.transform.localPosition = new Vector3(  metalFence.transform.localPosition.x,15,  metalFence.transform.localPosition.z);
        metalFence.transform.DOLocalMoveY(y,.25f);
    }
    public void OpenFromLockDown(){
        
        metalFence.transform.DOLocalMoveY(15,.25f).OnComplete((()=>{
        metalFence.gameObject.SetActive(false);
        }) );
    }

    public void Init(Border b,Slot s1,Slot s2)
    {
        border = b;
        roomA = border.roomA;
        roomB = border.roomB;
        occupiedSlots.Add(s1);
        occupiedSlots.Add(s2);
   
        foreach (var item in occupiedSlots)
        {
            if(item.cont.specialSlot == null){
                item.cont.wall = true;
                SpecialSlot ss =  Instantiate(doorInteractablePrefab,item.transform.position,Quaternion.identity);
                currentSpecialSlot = ss;
                DoorInteractable di = ss.interactable as DoorInteractable;
                doorInteractables.Add(di);
                di.door = this;
                item. cont.specialSlot = ss;
                if(item. cont.specialSlot.interactable != null)
                {item.cont.specialSlot.interactable.slot = item;}
                item.cont.specialSlot.slot = item;
                item.cont.specialSlot.Init();
           
            }
            else{
                Debug.LogWarning("Already has a door interactable");
            }
          
        }
        foreach (var item in occupiedSlots)
        {
            foreach (var s in  item.func.GetNeighbouringSlots())
            {
                if(!s.cont.wall)
                {triggerSlots.Add(s);}
            }
        }
        
        landingSlots.Add(roomA,new List<Slot>());
        landingSlots.Add(roomB,new List<Slot>());
        
        foreach (var item in triggerSlots)
        {landingSlots[item.room].Add(item);}
        if(s1.node.iGridX != s2.node.iGridX)
        {RotateDoor(0);}
        else
        {RotateDoor(90);}
        InitLandingSlots();

        foreach (var item in landingSlots)
        {
            foreach(var os in occupiedSlots)
            {
                if(item.Value.Contains(os))
                {item.Value.Remove(os); }
            }
        }
    }


    void InitLandingSlots(){

  
        foreach (var item in landingSlots[roomA] [0].func.GetNeighbouringSlots())
        {
            if(item.room == landingSlots[roomA] [0].room && !landingSlots[roomA].Contains(item))
            {landingSlots[roomA].Add(item);}
        }

        foreach (var item in landingSlots[roomA] [1].func.GetNeighbouringSlots())
        {
            if(item.room == landingSlots[roomA] [1].room && !landingSlots[roomA].Contains(item))
            {landingSlots[roomA].Add(item);}
        }
        
        foreach (var item in landingSlots[roomB] [0].func.GetNeighbouringSlots())
        {
            if(item.room == landingSlots[roomB] [0].room && !landingSlots[roomB].Contains(item))
            {landingSlots[roomB].Add(item);}
        }

        foreach (var item in landingSlots[roomB] [1].func.GetNeighbouringSlots())
        {
            if(item.room == landingSlots[roomB] [1].room && !landingSlots[roomB].Contains(item))
            { landingSlots[roomB].Add(item);}
        }
        
        foreach (var item in landingSlots[roomA])
        { 
            item.marked = true;
            if(spawnVisual){Instantiate(placeHolder,item.transform.position,Quaternion.identity);}
        }
        foreach (var item in landingSlots[roomB])
        { 
            item.marked = true;
            if(spawnVisual){ Instantiate(placeHolder,item.transform.position,Quaternion.identity).GetComponent<MeshRenderer>().material.color = Color.blue;}
        }
    }

    public void RotateDoor(float y){
        foreach (var item in doorInteractables)
        {
           item.transform. localRotation = Quaternion.Euler(new Vector3(0,y,0));
        }
        metalFence.transform.localRotation = Quaternion.Euler(new Vector3(0,y,0));
       
    }

    

}