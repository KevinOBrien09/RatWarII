using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using DG.Tweening;



public class CastleGeneratorBrain : MapGeneratorBrain
{
    public  GenericDictionary<int,Border> borderDict = new   GenericDictionary<int,Border>();
    public GenericDictionary<GenerationRoomType,Vector2> roomSizeDict = new GenericDictionary<GenerationRoomType, Vector2>();
   
    public GameObject blockage,roomSpacerPrefab;
    public Wall wall;
    public Door doorPrefab;
    public Border borderPrefab;
    public DungeonNode dungeonNodePrefab;
    public Vector2 blockagePercentRange;
    public LayerMask reAlignMask;
    Dictionary<Vector2,DungeonNode> dungDict = new Dictionary<Vector2, DungeonNode>();
    public List<DungeonNode> dungeonNodes = new List<DungeonNode>();
    public Slot slotPrefab;
    public bool debug;
    List<GameObject> gos = new List<GameObject>();
    
    
    public override void Generate(LocationInfo li = null)
    {
        MapManager.inst.mapQuirk = MapQuirk.ROOMS;
        (List<Node> path,Node startNode,Node endNode) layout = DrawLayout();
        SpawnDungeonNodes(layout.path);
        ParseLayout(layout.startNode,layout.endNode);
        SpawnSpacers();
        
    }

    public (List<Node>,Node startNode,Node endNode) DrawLayout()
    {
        
        List<Node>  rngNodes =  MapGenerator.inst.genGrid.ShuffledNodes();
        List<Node> ogList = new List<Node>(rngNodes);
        
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
        MapGenerator.inst.genGrid.UpdateGrid();
        Dictionary<Node,List<Node>> dict = new Dictionary<Node, List<Node>>();
        foreach (var item in rngNodes)
        {
            List<Node>  filteredRNGNodes = item.FilterUnadjacents(rngNodes,new List<Node>(),MapGenerator.inst.genGrid);
            dict.Add(item,filteredRNGNodes);
        }
        List<Node> filtered = new List<Node>();
        foreach (var item in dict)
        {
            if(item.Value.Count > filtered.Count)
            {filtered = item.Value;}
        }

        (Node n1,Node n2) startEnd = FurthestTwoNodes(filtered);
        List<Node>   mainPath  = MapGenerator.inst. generatorAstar.FindPath(startEnd.n1,startEnd.n2);
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
            
          
            MapGenerator.inst.genGrid.UpdateGrid();
        }
    }


    public void SpawnDungeonNodes(List<Node> layout){
   foreach (var item in layout)
        {
            DungeonNode dn = Instantiate(dungeonNodePrefab,item.vPosition,Quaternion.identity);
            dn.originalNode = item;
            dungDict.Add(new Vector2( dn.originalNode.iGridX, dn.originalNode.iGridY),dn );
            dungeonNodes.Add(dn);
        }
    }

    public void ParseLayout(Node start,Node end)
    {   
        
        
     

        foreach (var item in dungeonNodes)
        {item.GetNeighbours();}
        
        List<List<Node>> XDD = new List<List<Node>>();
        List<DungeonNode> dinner = new List<DungeonNode>();
        List<Vector2> lunch = new List<Vector2>();
        
        if(sideHallway())
        {Debug.Log("made hallways");}
        else
        {Debug.Log("no hallways");}
        foreach (var item in lunch)
        {dungDict.Remove(item);}
        if(bigRoom())
        {Debug.Log("Created BigRoom");}
        else{Debug.Log("No big room");}
        Clear();
        vertDoubleRoom();
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
                if(dungDict.ContainsKey(left))
                {
                    if(dungDict[left].isHall)
                    {continue;}
                }
                if(dungDict.ContainsKey(up))
                {
                    if(dungDict[up].isHall)
                    {continue;}
                }
                if(dungDict.ContainsKey(down))
                {
                    if(dungDict[down].isHall)
                    {continue;}
                }
                if(dungDict.ContainsKey(right))
                {
                    if(dungDict[right].isHall)
                    {continue;}
                }

                bool canMakeSideHallway = dungDict.ContainsKey(og) && 
                dungDict.ContainsKey(left) && dungDict.ContainsKey(right) && !dungDict.ContainsKey(up) && !dungDict.ContainsKey(down);

                bool canMakeVertHallway = dungDict.ContainsKey(og) && 
                !dungDict.ContainsKey(left) &&!dungDict.ContainsKey(right) && dungDict.ContainsKey(up) && dungDict.ContainsKey(down);

                if(canMakeSideHallway)
                {
                    dungDict[og].MarkAsSideHallway();
                    lunch.Add(og);
                    foundHallway = true;
                }

                if(canMakeVertHallway)
                {
                    dungDict[og].MarkAsVertHallway();
                    lunch.Add(og);
                    foundHallway = true;
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
                && dungDict.ContainsKey(upRight)
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


        bool vertDoubleRoom(){
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

               
                if(dungDict.ContainsKey(left) && dungDict.ContainsKey(og)) 
                {
                    if(item.roomType == GenerationRoomType.NORMAL)
                    {
                        if(dungDict[og].roomType == GenerationRoomType.NORMAL && dungDict[left].roomType == GenerationRoomType.NORMAL)
                        {
                            if(Random.Range(0,3 ) ==1)
                            {
                                dungDict[og].MarkAsHORIDouble(dungDict[left]);
                                dinner.Add(dungDict[left]);
                                dungDict.Remove(og);
                                dungDict.Remove(left);
                            }
                        }
                    }
                }

                if(dungDict.ContainsKey(right) && dungDict.ContainsKey(og)) 
                {
                    if(item.roomType == GenerationRoomType.NORMAL)
                    {
                        if(dungDict[og].roomType == GenerationRoomType.NORMAL && dungDict[right].roomType == GenerationRoomType.NORMAL)
                        {
                            if(Random.Range(0,3 ) ==1){
                            dungDict[og].MarkAsHORIDouble(dungDict[right]);
                            dinner.Add(dungDict[right]);
                            dungDict.Remove(og);
                            dungDict.Remove(right);
                            }
                        }
                    }
                }
                
                if(dungDict.ContainsKey(up) && dungDict.ContainsKey(og)) 
                {
                    if(item.roomType == GenerationRoomType.NORMAL)
                    {
                        if(dungDict[og].roomType == GenerationRoomType.NORMAL && dungDict[up].roomType == GenerationRoomType.NORMAL)
                        {
                            if(Random.Range(0,3 ) ==1){
                            dungDict[og].MarkAsVERTDouble(dungDict[up]);
                            dinner.Add(dungDict[up]);
                            dungDict.Remove(og);
                            dungDict.Remove(up);
                            }
                        }
                    }

                }

                if(dungDict.ContainsKey(down) && dungDict.ContainsKey(og)) 
                {
                    if(item.roomType == GenerationRoomType.NORMAL)
                    {
                        if(dungDict[og].roomType == GenerationRoomType.NORMAL && dungDict[down].roomType == GenerationRoomType.NORMAL)
                        {
                            if(Random.Range(0,3 ) ==1){
                            dungDict[og].MarkAsVERTDouble(dungDict[down]);
                            dinner.Add(dungDict[down]);
                            dungDict.Remove(og);
                            dungDict.Remove(down);
                            }
                        }
                    }

                }
            }

            return false;
        }
       
        void Clear()
        {
            foreach (var item in dinner)
            {
                dungeonNodes.
                Remove(item);
                Destroy(item. gameObject);
            }
        }
    }

    public void SpawnSpacers()
    {
        List<GameObject> spacers = new List<GameObject>();
        Dictionary<GameObject,Room> dict = new Dictionary<GameObject, Room>();
        int i = 1;
        foreach (var item in dungeonNodes)
        {
            GameObject spacer = Instantiate(roomSpacerPrefab,item.spawnPoint.position,Quaternion.identity);
            Room r = item.gameObject.AddComponent<Room>();
            r.Init(i);
            r.roomType = item.roomType;
            dict.Add(spacer,r);
            spacers.Add(spacer);
            Vector2 v = roomSizeDict[item.roomType];
            spacer.transform.localScale = new Vector3(v.x,1,v.y);
            i++;
        }
        MapGenerator.inst.genGrid.enabled = false;
        MapManager.inst.map.vGridWorldSize = MapGenerator.inst.genGrid.vGridWorldSize;
        MapManager.inst.map.CreateGrid();
        List<Node> n = new List<Node>();
        StartCoroutine(q());
        IEnumerator q()
        {
            yield return new WaitForSeconds(.1f);
            SpawnSlots(dict);
           if(CheckIfValid()){
                PerimeterSlots();
                AssignStartEndRooms();
                yield return new WaitForEndOfFrame();
                BordersAndDoors();
                RoomContent();
                foreach (var item in spacers)
                {  
                    Destroy(item.gameObject);
                }
                foreach (var item in gos)
                {
                    Destroy(item.gameObject);
                }
                yield return new WaitForEndOfFrame();
                MapManager.inst.map.UpdateGrid();
               MapGenerator.inst.WrapUp();
               
           }
            
        }
    }

    void SpawnSlots(  Dictionary<GameObject,Room> dict)
    {
        foreach (var item in  MapManager.inst.map.NodeArray)
        {
            var g = Physics.OverlapSphere(item.vPosition,.1f,layerMask:reAlignMask);
            
            if(g.Length == 1)
            {
                Room r = dict[g[0].gameObject];
                CreateSlot(item, r);
            }
            else if(g.Length > 1)
            {Debug.LogAssertion("TWO COLLIDERS FOUND, ITS OVER!!!");}
            else
            {//Instantiate(wall,item.vPosition,Quaternion.identity);
            }


          
        }

        foreach (var item in dict)
        {MapManager.inst.map.rooms. Add(item.Value);}
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
        s.ChangeMaterial(slotMat);
    }


    void PerimeterSlots()
    {
        List<Slot> perimeterSlots = new List<Slot>();
        foreach (var item in  MapManager.inst.allSlots)
        {
            List<Direction> dirs = item.func.CheckIfSideSlot();
            if(dirs.Count > 0)
            {
                if(!perimeterSlots.Contains(item))
                {
                    if(dirs.Count == 1){
                        perimeterSlots.Add(item);
                        item.IsWall();
                        Wall w = Instantiate(wall,item.transform.position,Quaternion.identity);
                        item.room.wallDict[dirs[0]].Add(w);
                        w.transform.SetParent(item.room.transform);
                        item.gameObject.SetActive(false);
                        
                    }
                }
            }
        }
        
        foreach (var item in  MapManager.inst.allSlots)
        {
            List<Direction> dirs = item.func.CheckIfSideSlot();
            if(dirs.Count > 0)
            {   item.IsWall();
                Wall w = Instantiate(wall,item.transform.position,Quaternion.identity);
                perimeterSlots.Add(item); w.transform.SetParent(item.room.transform);
                item.gameObject.SetActive(false);
            }
        }
    }

    public bool CheckIfValid()
    {
        Slot s = MapManager.inst.map.rooms[0].RandomSlot();
        List<bool> l = new List<bool>();
        foreach (var item in MapManager.inst.map.rooms)
        {
            Node targetNode = item.RandomSlot().node;
            List<Node> n = MapManager.inst.map.aStar.FindPath(s.node,targetNode);
            if(!n.Contains(targetNode))
            {
                Debug.LogAssertion("ROOMS ARE NOT CONNECTED");
                Reset();
                return false;
            }
        }
        bool playerSelectedRetrevial = GameManager.inst.chosenObjective == Objective.ObjectiveEnum.RETRIEVAL && GameManager.inst.chosenQuest;
        bool debugSelectedRetrevial = ObjectiveManager.inst.predecideObjective && ObjectiveManager.inst.predecidedObjective == Objective.ObjectiveEnum.RETRIEVAL;
        if(playerSelectedRetrevial |debugSelectedRetrevial)
        {
            if(MapManager.inst.map.rooms.Count <= 4)
            {
                Debug.LogAssertion("NOT ENOUGH ROOMS FOR RETRIEVAL MISSION");
                Reset();
                return false;
            }

        }

        return true;
    }

    public override void Reset(){
        Debug.Log("RESET");
        if(!debug){
            StopAllCoroutines();
            BattleManager.inst.StopAllCoroutines();
            SceneManager.LoadScene("Arena");
        }

    }


    void AssignStartEndRooms()
    {
        Dictionary<Room,Slot>randomSlotFromEachRoom = new  Dictionary<Room,Slot>();
        Room startRoom = null;
        Room endRoom = null;
        List<Node> n = new List<Node>();
        foreach (var item in MapManager.inst.map.rooms)
        {randomSlotFromEachRoom.Add(item,item.slots[Random.Range(0,item.slots.Count)]) ;}

        foreach (var aRoom in randomSlotFromEachRoom)
        {
            foreach (var bRoom in randomSlotFromEachRoom)
            {
                if(aRoom.Value != bRoom.Value)
                {
                    List<Node> a = MapManager.inst.map.aStar.FindPath(aRoom.Value.node,bRoom.Value.node);
                    if(a.Count > n.Count){
                        n = new List<Node>(a);
                        startRoom = aRoom.Value.room;
                        endRoom = bRoom.Value.room;
                    }
                }
            }
        }

        MapManager.inst.map.AssignStartEnd(startRoom,endRoom);
      
    }


    void BordersAndDoors()
    {
        List<Slot> borderSlots = new List<Slot>();
        foreach (var item in MapManager.inst.map.rooms)
        {item.GetBorderRooms();}

        foreach (var room in MapManager.inst.map.rooms)
        {
            foreach (var bRoom in room.borderRooms)
            {
                int a =  Mathf.Min(room.roomID,bRoom.roomID);
                int b = Mathf.Max(room.roomID,bRoom.roomID);
                int id = int.Parse(a.ToString()+b.ToString());
                if(!borderDict.ContainsKey(id))
                {
                    Border border =  Instantiate(borderPrefab);
                    border.borderID = id;
                    border.roomA = room;
                    border.roomB = bRoom;
                    room.borders.Add(border);
                    bRoom.borders.Add(border);
                    border.name = "Border of Rooms " + room.roomID +" and " + bRoom.roomID;
                    borderDict.Add(border.borderID,border);
                    border.GetBorderSlots();
                }
            }
        }
        
        foreach (var item in borderDict)
        {
            Border border = item.Value;
            List<Transform> t = new List<Transform>();
            foreach (var st in border.slots)
            {
                t.Add(st.transform);
            }
            border.transform.position = MiscFunctions.FindCenterOfTransforms(t);
            
            List<Slot> s = new List<Slot>(border.slots);
            int i = s.Count/2;
            Slot doorSlotA = s[ i];
            Slot doorSlotB = s[i-1 ];
            t.Clear();
            t.Add(doorSlotA.transform);
            t.Add(doorSlotB.transform);
          
            s.Remove(doorSlotA);
            s.Remove(doorSlotB);
            foreach (var slot in s)
            {
                Wall w = Instantiate(wall,slot.transform.position,Quaternion.identity);
                border.walls.Add(w);
                slot.IsWall();
            }
            Door door =  Instantiate(doorPrefab,MiscFunctions.FindCenterOfTransforms(t),Quaternion.identity);
            door.Init(border,doorSlotA,doorSlotB);
            border.doors.Add(door);
        }
    }

    public void RoomContent()
    {
        foreach (var item in MapManager.inst.map.rooms)
        {item.AddRoomContent();}

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
}