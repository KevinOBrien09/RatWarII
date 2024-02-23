using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using UnityEngine.Events;
using DG.Tweening;

public enum GenerationRoomType{NORMAL,BIG,SIDEHALL,VERTHALL}
public class MapGenerator : Singleton<MapGenerator>
{
    public AStar generatorAstar;
    public Grid_ mapGrid;
    public int howManyPartyMembers,howManyEnemies;
    public SpikeSlot spikeSlotPrefab;
    public GameObject placeholder,blockage,plane,offshootTarget;
    public DungeonNode dungeonNodePrefab;
    public Vector2 blockagePercentRange;
    public bool showVisual;
    public bool generating;
   public List<DungeonNode> dungeonNodes = new List<DungeonNode>();
    protected override void Awake(){
        base.Awake();
       
        mapGrid.CreateGrid();
      
    }

    public void Generate()
    {
        (List<Node> path,Node startNode,Node endNode) layout = DrawLayout();
        ParseLayout(layout.path,layout.startNode,layout.endNode);
    }

    public (List<Node>,Node startNode,Node endNode) DrawLayout()
    {
        generating = true;
        List<Node>  rngNodes =  mapGrid.ShuffledNodes();
        List<Node> ogList = new List<Node>(rngNodes);
        List<GameObject> gos = new List<GameObject>();
        List<Node> startEndCandidates = new List<Node>();
        List<Node> bin = new List<Node>();

        int blockCount = Random.Range(MiscFunctions.GetPercentage(rngNodes.Count,blockagePercentRange.x),
        MiscFunctions.GetPercentage(rngNodes.Count,blockagePercentRange.y));
        for (int i = 0; i < blockCount; i++)
        {
            Node n = rngNodes[i];
            Spawn(blockage,rngNodes[i]);
            bin.Add(n);
        }
        foreach (var item in bin)
        { rngNodes.Remove(item); }
        mapGrid.UpdateGrid();
        Dictionary<Node,List<Node>> dict = new Dictionary<Node, List<Node>>();
        foreach (var item in rngNodes)
        {
            List<Node>  filteredRNGNodes = item.FilterUnadjacents(rngNodes,new List<Node>(),mapGrid);
            dict.Add(item,filteredRNGNodes);
        }
        List<Node> filtered = new List<Node>();
        foreach (var item in dict)
        {
            if(item.Value.Count > filtered.Count)
            {filtered = item.Value;}
        }

        (Node n1,Node n2) startEnd = FurthestTwoNodes(filtered);
        if(showVisual)
        {
            Spawn(placeholder,startEnd.n1);
            Spawn(placeholder,startEnd.n2);
        }
        List<Node>   mainPath  = generatorAstar.FindPath(startEnd.n1,startEnd.n2);
        bool failed = !mainPath.Contains(startEnd.n2);
        if(failed)
        {
            Debug.LogWarning("Invalid Critical Path");
            StartCoroutine(g());
            IEnumerator g()
            {
                Clear();
                yield return new WaitForSeconds(.1f);
                yield return  DrawLayout();
            }
        }
        else
        {
            foreach (var item in filtered)
            { 
                if(showVisual)
                {Spawn(plane,item);}
            }
            foreach (var item in ogList)
            {
                if(!item.isBlocked && !filtered.Contains(item)){
                    Spawn(blockage,item);
                }
            }
            return (filtered,startEnd.n1,startEnd.n2);
            
        }
        Debug.LogAssertion("LIST IS NULL!!!");
        return (null,null,null);

        void Spawn(GameObject prefab,Node n)
        {gos.Add(Instantiate(prefab,n.vPosition,Quaternion.identity));}
        void Clear()
        {
            foreach (var item in gos)
            {Destroy(item.gameObject);}
            mapGrid.UpdateGrid();
        }
    }

    public void ParseLayout(List<Node> layout,Node start,Node end)
    {   
        
        Dictionary<Vector2,DungeonNode> dungDict = new Dictionary<Vector2, DungeonNode>();
        foreach (var item in layout)
        {
            DungeonNode dn = Instantiate(dungeonNodePrefab,item.vPosition,Quaternion.identity);
            dn.originalNode = item;
            dungDict.Add(new Vector2( dn.originalNode.iGridX, dn.originalNode.iGridY),dn );
            dungeonNodes.Add(dn);
        }

        foreach (var item in dungeonNodes)
        {item.GetNeighbours();}
        
        List<List<Node>> XDD = new List<List<Node>>();
        List<DungeonNode> dinner = new List<DungeonNode>();
if(sideHallway()){
            Debug.Log("made hallways");
        }
        else{
            Debug.Log("no hallways");
        }
        if(bigRoom())
        {Debug.Log("Created BigRoom");}
        else{Debug.Log("No big room");}
        Clear();
        

        bool sideHallway()
        {
            bool foundHallway = false;
            foreach(var item in dungeonNodes)
            {
                Vector2 up = new Vector2( item.originalNode.iGridX, item.originalNode.iGridY+1);
                Vector2 down =  new Vector2( item.originalNode.iGridX, item.originalNode.iGridY-1);
                Vector2 left = new Vector2( item.originalNode.iGridX-1, item.originalNode.iGridY);
                Vector2 right = new Vector2( item.originalNode.iGridX+1, item.originalNode.iGridY);
                Vector2 og = new Vector2( item.originalNode.iGridX, item.originalNode.iGridY);
                bool canMakeSideHallway = dungDict.ContainsKey(og) && 
                dungDict.ContainsKey(left) && dungDict.ContainsKey(right) && !dungDict.ContainsKey(up) && !dungDict.ContainsKey(down);

                bool canMakeVertHallway = dungDict.ContainsKey(og) && 
                !dungDict.ContainsKey(left) &&!dungDict.ContainsKey(right) && dungDict.ContainsKey(up) && dungDict.ContainsKey(down);

                if(canMakeSideHallway)
                {
                    if(dungDict[left].roomType != GenerationRoomType.VERTHALL && dungDict[right].roomType != GenerationRoomType.VERTHALL)
                    {
                        dungDict[og].MarkAsSideHallway();

                        dungDict.Remove(og);
                        foundHallway = true;
                    }
                
                }

                if(canMakeVertHallway)
                {
                    if(dungDict[up].roomType != GenerationRoomType.SIDEHALL && dungDict[down].roomType != GenerationRoomType.SIDEHALL)
                    {
                        dungDict[og].MarkAsVertHallway();

                        dungDict.Remove(og);
                        foundHallway = true;
                    }
                }


            }

            return foundHallway;
        }

        bool bigRoom()
        {
            foreach(var item in dungeonNodes)
            {
                Vector2 og = new Vector2( item.originalNode.iGridX, item.originalNode.iGridY);
                Vector2 up = new Vector2( item.originalNode.iGridX, item.originalNode.iGridY+1);
                Vector2 down =  new Vector2( item.originalNode.iGridX, item.originalNode.iGridY-1);
                Vector2 upRight = new Vector2( item.originalNode.iGridX+1, item.originalNode.iGridY+1);
                Vector2 right = new Vector2( item.originalNode.iGridX+1, item.originalNode.iGridY);
                Vector2 upLeft = new Vector2( item.originalNode.iGridX-1, item.originalNode.iGridY+1);
                Vector2 left = new Vector2( item.originalNode.iGridX-1, item.originalNode.iGridY);
                Vector2 downLeft = new Vector2( item.originalNode.iGridX-1, item.originalNode.iGridY-1);
                Vector2 downRight = new Vector2( item.originalNode.iGridX+1, item.originalNode.iGridY-1);
                bool botLeft = dungDict.ContainsKey(og) && 
                dungDict.ContainsKey(up) 
                &&  dungDict.ContainsKey(upRight)
                && dungDict.ContainsKey(right);

                bool botRight = dungDict.ContainsKey(og) && 
                dungDict.ContainsKey(up) 
                &&  dungDict.ContainsKey(upLeft) 
                && dungDict.ContainsKey(left);

                bool topRight = dungDict.ContainsKey(og) &&
                dungDict.ContainsKey(down) 
                &&  dungDict.ContainsKey(left) 
                && dungDict.ContainsKey(downLeft);

                
                bool topLeft = dungDict.ContainsKey(og) &&
                dungDict.ContainsKey(down)
                &&  dungDict.ContainsKey(right)
                && dungDict.ContainsKey(downRight);
                bool possibleBigRoom = botLeft | botRight | topRight | topLeft;
                if(possibleBigRoom)
                {
                
                    Debug.Log("LEBIGROOM X^D");
                    if(botLeft)
                    {
                        dinner.Add(dungDict[up]);
                        dinner.Add(dungDict[upRight]);
                        dinner.Add(dungDict[right]);
                        dungDict[og].MarkAsBigRoom(dinner);

                        dungDict.Remove(og);
                        dungDict.Remove(up);
                        dungDict.Remove(upRight);
                        dungDict.Remove(right);
                        
                    }
                    else if(botRight)
                    {
                        dinner.Add(dungDict[up]);
                        dinner.Add(dungDict[upLeft]);
                        dinner.Add(dungDict[left]);
                        dungDict[og].MarkAsBigRoom(dinner);

                        dungDict.Remove(og);
                        dungDict.Remove(up);
                        dungDict.Remove(left);
                        dungDict.Remove(upLeft); 

                    }
                    else if(topRight)
                    {   
                        dinner.Add(dungDict[down]);
                        dinner.Add(dungDict[left]);
                        dinner.Add(dungDict[downLeft]);
                        dungDict[og].MarkAsBigRoom(dinner);
                        
                        dungDict.Remove(og);
                        dungDict.Remove(down);
                        dungDict.Remove(left);
                        dungDict.Remove(downLeft); 
                    }
                    else if(topLeft)
                    {   
                        dinner.Add(dungDict[down]);
                        dinner.Add(dungDict[right]);
                        dinner.Add(dungDict[downRight]);
                        dungDict[og].MarkAsBigRoom(dinner);
                        
                        dungDict.Remove(og);
                        dungDict.Remove(down);
                        dungDict.Remove(right);
                        dungDict.Remove(downRight); 
                    }
                    return true;
                }
            }  
            return false;
        }
       
        void Clear()
        {
            foreach (var item in dinner)
            {
                dungeonNodes.Remove(item);
                Destroy(item. gameObject);
            }
        }
       
           

        MakeMap();
    }

    public void MakeMap()
    {
        //List<Vector3> v = new List<Vector3>();
        // Dictionary<int,int> xPaddingDict = new Dictionary<int, int>();
        // Dictionary<int,int> zPaddingDict = new Dictionary<int, int>();
        // int zPadding = 0;
        // int xPadding = 0;
        // for (int i = 0; i < mapGrid.iGridSizeX; i++)
        // {
        //     xPaddingDict.Add(i,xPadding);
        //     xPadding += 5;
        // }

   
        // for (int i = 0; i < mapGrid.iGridSizeY; i++)
        // {
        //     zPaddingDict.Add(i,zPadding);
        //     zPadding += 5;
        // }
        
        // for (int i = 0; i < layout.Count; i++)
        // {
        //     Vector3 v3 = new Vector3(
        //     layout[i].vPosition.x + xPaddingDict[layout[i].iGridX],
        //     layout[i].vPosition.y,
        //     layout[i].vPosition.z + zPaddingDict[layout[i].iGridY]);  
        //     v.Add(v3);
        //     //padding += 5;
        // }
  
        

        foreach (var item in dungeonNodes)
        {
            MapManager.inst.SpawnRoom(item);
        }
        generating = false;
  
    }   

    

    public  (Node,Node) FurthestTwoNodes(List<Node> nodeList)
    {
        float FurthestDistance = 0;
        Node n1 = null;
        Node  n2 = null;
        foreach(Node node in nodeList)
        {
            for (int i = 0; i < nodeList.Count; i++)
            {
                float ObjectDistance = Vector3.Distance(nodeList[i].vPosition, node.vPosition);
                if (ObjectDistance > FurthestDistance)
                {
                    n1 = node;
                    n2 = nodeList[i];
                    FurthestDistance = ObjectDistance;
                }
            }
        }
        return (n1,n2);
    }


    



  

   public void CreateStartingUnits(){
        List<Slot> shuffle = MapManager.inst.StartingRadius();
        for (int i = 0; i < howManyPartyMembers; i++)
        { UnitFactory.inst. CreatePlayerUnit(shuffle[i]);}

        for (int i = 0; i < howManyEnemies; i++)
        {UnitFactory.inst. CreateEnemyUnit(MapManager.inst.RandomSlot());}
    }
   
}