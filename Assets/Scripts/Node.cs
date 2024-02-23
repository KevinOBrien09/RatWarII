using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Node 
{
    public int iGridX;
    public int iGridY;

    public bool isBlocked;
    public Vector3 vPosition;

    public Node ParentNode;

    public int igCost;
    public int ihCost;

    public int FCost { get { return igCost + ihCost; } }
    public bool offShootStart;
    public Slot slot;

    public Node(bool a_bIsWall, Vector3 a_vPos, int a_igridX, int a_igridY)
    {
        isBlocked = a_bIsWall;
        vPosition = a_vPos;
        iGridX = a_igridX;
        iGridY = a_igridY;
    }


   public List<Node> FilterUnadjacents(List<Node> candidates,List<Node> slots,Grid_ g)
    {
        foreach (var item in g.GetNeighboringNodes(this))
        {
            if(candidates.Contains(item))
            {
                if(!slots.Contains(item))
                {
                    
                    if(!item.isBlocked)
                    {
                        slots.Add(item);
                        item.FilterUnadjacents(candidates,slots,g);
                    }
                    
                }
            }
        }
        return slots;
    }

    
   
}
