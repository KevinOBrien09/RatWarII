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
     
    public override void CreateGrid()
    {
        ocean = gameObject.AddComponent<Room>();
        MapManager.inst.map.rooms.Add(ocean);
        ocean.Init(MapManager.inst.map.rooms.Count);

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
            }
            
        }

        SpawnBoat();
    }

    public void SpawnBoat(){
        Slot s = ocean.RandomSlot();
        Boat b = Instantiate(boatPrefab,s.node.vPosition,Quaternion.identity);
        b.Reposition(s);
    }

    // public override void OnDrawGizmos()
    // {
    //     if(!enabled){
    //         return;
    //     }
    //     Gizmos.DrawWireCube(transform.position, new Vector3(vGridWorldSize.x, 1, vGridWorldSize.y));

    //     if (NodeArray != null)
    //     {
    //         foreach (Node n in NodeArray)
    //         {
    //             if (!n.isBlocked)
    //             {
    //                 Gizmos.color = Color.magenta;//Set the color of the node
    //             }
    //             else
    //             {
    //                 Gizmos.color = Color.red;//Set the color of the node
    //             }


    //             if (FinalPath != null)
    //             {
    //                 if (FinalPath.Contains(n))
    //                 {
    //                     Gizmos.color = Color.green;
    //                 }

    //             }

    //             if (n.isBlocked)
    //             {
    //                 Gizmos.DrawWireCube(n.vPosition, Vector3.one * (fNodeDiameter - fDistanceBetweenNodes));//Draw the node at the position of the node.
    //             }
    //             else
    //             {
    //                 Gizmos.DrawWireCube(n.vPosition, Vector3.one * (fNodeDiameter - fDistanceBetweenNodes));//Draw the node at the position of the node.

    //             }
                
    //         }
    //     }
    // }

}