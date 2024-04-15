using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Grid_ : MonoBehaviour
{
   
    public LayerMask blockage;
    public AStar aStar;
    public Vector2 vGridWorldSize;
    public float fNodeRadius;
    public float fDistanceBetweenNodes;
    public Node[,] NodeArray;
    public List<Node> FinalPath;
   [HideInInspector] public float fNodeDiameter;
    public int iGridSizeX, iGridSizeY;

    public void ResizeGrid (Vector2 size){
        vGridWorldSize  = size;
        CreateGrid();
    }

   
   public virtual void CreateGrid()
    {
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
                bool isBlocked = false;//Make the node a wall

                if (Physics.CheckBox(worldPoint,new Vector3(fNodeRadius,fNodeRadius,fNodeRadius) ,Quaternion.identity, blockage))
                {
                    isBlocked = true;
                }

                NodeArray[x, y] = new Node(isBlocked, worldPoint, x, y);
            }
        }
    }

    public Node CenterNode(){
        return  NodeArray[iGridSizeX/2,iGridSizeY/2];
    }

    public List<Node> ShuffledNodes()
    {
        List<Node> Xd = new List<Node>();
        foreach (var item in NodeArray)
        {
            Xd.Add(item);
        }
        System.Random rng = new System.Random();
        return Xd.OrderBy(_ => rng.Next()).ToList();

        
    }

  
    public void UpdateGrid()
    {
        foreach (var item in NodeArray)
        {
         
            item.isBlocked = false;
            if (Physics.CheckBox(item.vPosition,new Vector3(fNodeRadius,fNodeRadius,fNodeRadius) ,Quaternion.identity, blockage))
            {
                item.isBlocked = true;
            } 
           
           
            
        }
    }

    //Function that gets the neighboring nodes of the given node.
    public List<Node> GetNeighboringNodes(Node a_NeighborNode)
    {
        List<Node> NeighborList = new List<Node>();
        int icheckX;
        int icheckY;

        //Check the right side of the current node.
        icheckX = a_NeighborNode.iGridX + 1;
        icheckY = a_NeighborNode.iGridY;
        if (icheckX >= 0 && icheckX < iGridSizeX)
        {
            if (icheckY >= 0 && icheckY < iGridSizeY)
            {
                NeighborList.Add(NodeArray[icheckX, icheckY]);
            }
        }
       
        icheckX = a_NeighborNode.iGridX - 1;
        icheckY = a_NeighborNode.iGridY;
        if (icheckX >= 0 && icheckX < iGridSizeX)
        {
            if (icheckY >= 0 && icheckY < iGridSizeY)
            {
                NeighborList.Add(NodeArray[icheckX, icheckY]);
            }
        }
     
        icheckX = a_NeighborNode.iGridX;
        icheckY = a_NeighborNode.iGridY + 1;
        if (icheckX >= 0 && icheckX < iGridSizeX)
        {
            if (icheckY >= 0 && icheckY < iGridSizeY)
            {
                NeighborList.Add(NodeArray[icheckX, icheckY]);
            }
        }
     
        icheckX = a_NeighborNode.iGridX;
        icheckY = a_NeighborNode.iGridY - 1;
        if (icheckX >= 0 && icheckX < iGridSizeX)
        {
            if (icheckY >= 0 && icheckY < iGridSizeY)
            {
                NeighborList.Add(NodeArray[icheckX, icheckY]);
            }
        }

        return NeighborList;
    }
    public Node NodeFromWorldPoint(Vector3 a_vWorldPos)//BROKEN
    {
        float ixPos = ((a_vWorldPos.x + vGridWorldSize.x / 2) / vGridWorldSize.x);
        float iyPos = ((a_vWorldPos.z + vGridWorldSize.y / 2) / vGridWorldSize.y);

        ixPos = Mathf.Clamp01(ixPos);
        iyPos = Mathf.Clamp01(iyPos);

        int ix = Mathf.RoundToInt((iGridSizeX - 1) * ixPos);
        int iy = Mathf.RoundToInt((iGridSizeY - 1) * iyPos);

        return NodeArray[ix, iy];
    }


    //Function that draws the wireframe
    public virtual void OnDrawGizmos()
    {
        if(!enabled){
            return;
        }
        Gizmos.DrawWireCube(transform.position, new Vector3(vGridWorldSize.x, 1, vGridWorldSize.y));

        if (NodeArray != null)
        {
            foreach (Node n in NodeArray)
            {
                if (!n.isBlocked)
                {
                    Gizmos.color = Color.black;//Set the color of the node
                }
                else
                {
                    Gizmos.color = Color.red;//Set the color of the node
                }


                if (FinalPath != null)
                {
                    if (FinalPath.Contains(n))
                    {
                        Gizmos.color = Color.green;
                    }

                }

                if (n.isBlocked)
                {
                    Gizmos.DrawWireCube(n.vPosition, Vector3.one * (fNodeDiameter - fDistanceBetweenNodes));//Draw the node at the position of the node.
                }
                else
                {
                    Gizmos.DrawWireCube(n.vPosition, Vector3.one * (fNodeDiameter - fDistanceBetweenNodes));//Draw the node at the position of the node.

                }
                
            }
        }
    }
}