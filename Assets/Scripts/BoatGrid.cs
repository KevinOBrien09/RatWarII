using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class BoatGrid : Grid_
{
    public BoatSlot boatSlotPrefab;
    public GenericDictionary<Vector3,Node> nodes = new GenericDictionary<Vector3, Node>();
    public Room ocean;
    public Boat boatPrefab;
    public GenericDictionary<int,List<BoatSlot>> seas = new GenericDictionary<int, List<BoatSlot>>();
    public List<BoatSlot> mainSea;
    public override void CreateGrid()
    {
        ocean = gameObject.AddComponent<Room>();
        MapManager.inst.map.rooms.Add(ocean);
        ocean.Init(MapManager.inst.map.rooms.Count);
        gameObject.name = "Room:Ocean";

       fNodeDiameter = fNodeRadius * 2;
        iGridSizeX = Mathf.RoundToInt(vGridWorldSize.x / fNodeDiameter);
        iGridSizeY = Mathf.RoundToInt(vGridWorldSize.y / fNodeDiameter);

        NodeArray = new Node[iGridSizeX, iGridSizeY];
        Vector3 bottomLeft = transform.position - Vector3.right * vGridWorldSize.x / 2 - Vector3.forward * vGridWorldSize.y / 2;//Get the real world position of the bottom left of the grid.
        for (int x = 0; x < iGridSizeX; x++)
        {
            for (int y = 0; y < iGridSizeY; y++)
            {
                Vector3 worldPoint = bottomLeft + Vector3.right * (x * fNodeDiameter + fNodeRadius) + Vector3.forward * (y * fNodeDiameter + fNodeRadius);//Get the world co ordinates of the bottom left of the graph
                worldPoint = new Vector3(worldPoint.x+2.5f,worldPoint.y,worldPoint.z+2.5f);
                bool isBlocked = false;//Make the node a wall

                if (Physics.CheckBox(worldPoint,new Vector3(fNodeRadius,fNodeRadius,fNodeRadius) ,Quaternion.identity, blockage))
                {
                    isBlocked = true;
                }

                NodeArray[x, y] = new Node(isBlocked, worldPoint, x, y);
            }
        }
    }

    public void SpawnSlots(){
        foreach (var item in NodeArray)
        {
            if (!Physics.CheckBox(item.vPosition,new Vector3(fNodeRadius,fNodeRadius,fNodeRadius) ,Quaternion.identity, blockage))
            {
                BoatSlot bs = Instantiate(boatSlotPrefab,item.vPosition,Quaternion.identity);
                bs.node = item;
                item.slot = bs;
                bs.transform.name =  "WATER:" + bs.node.iGridX + "|"+bs.node.iGridY;
                ocean.slots.Add(bs);
                bs.CheckIfCostal();
            }
            
        }
    }

    public void ParseOcean(){
        List<  BoatSlot> unaccBoatSlots = new List<BoatSlot>();
        foreach(var o in ocean.slots){
            BoatSlot bs = o as BoatSlot;
            unaccBoatSlots.Add(bs);
        }

        loop();
        void loop()
        {
            System.Random rng = new System.Random();
            List<  BoatSlot> shuff =  unaccBoatSlots.OrderBy(_ => rng.Next()).ToList();
            if(shuff.Count != 0)
            {
                BoatSlot s = null;
                for (int i = 0; i < shuff.Count; i++)
                {
                    BoatSlot q = shuff[i];
                    if(q.cont.walkable()){
                        s = q;
                        break;
                    }
                }
                if(s != null)
                {
                    List<BoatSlot> sea =  s.FilterUnadjacentsBOAT(unaccBoatSlots,new List<BoatSlot>(),true);
                    if(sea.Count > 0)
                    {
                        seas.Add(seas.Count+1,sea);
                        foreach (var item in sea)
                        {unaccBoatSlots.Remove(item);}
                    }
                    else
                    {
                        //This is for 1x1 isolated tiles.
                       sea.Add(s);
                       unaccBoatSlots.Remove(s);
                        seas.Add(seas.Count+1,sea);
                    }
                    
                    if(unaccBoatSlots.Count != 0)
                    {loop();}
                }
               
            }
        }
        
        int count = 0;
        foreach (var item in seas)
        {
            if(item.Value.Count > count){
                mainSea = item.Value;
                count = item.Value.Count;
            }
        }
    }

    public Boat SpawnBoat(Island startingIsland){
        BoatSlot bs = startingIsland.RandomBoatSlot();
        Boat b = Instantiate(boatPrefab,bs.node.vPosition,Quaternion.identity);
        b.Reposition(bs);
      
        b.Dock();
        return b;
       
    }

}