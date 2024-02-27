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
    public Transform doorHolder;
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
        {item.IsWall();}
        foreach (var item in occupiedSlots)
        {
            foreach (var s in  item.func.GetNeighbouringSlots())
            {
                if(!s.cont.wall)
                {
                    triggerSlots.Add(s);
                    SpecialSlot ss =  Instantiate(doorInteractablePrefab,s.transform.position,Quaternion.identity);
                    currentSpecialSlot = ss;
                    DoorInteractable di = ss.interactable as DoorInteractable;
                    di.door = this;
                    s.MakeSpecial(ss);
                    
                }
            }
        }

        landingSlots.Add(roomA,new List<Slot>());
        landingSlots.Add(roomB,new List<Slot>());

      

        foreach (var item in triggerSlots)
        {landingSlots[item.room].Add(item);}
        if(landingSlots[roomA] [0].node.iGridX != landingSlots[roomA][1].node.iGridX)
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
        doorHolder.localRotation = Quaternion.Euler(new Vector3(0,y,0));
    }

    

}