using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using DG.Tweening;
using UnityEngine.Rendering;


public class ForestGenerationBrain : MapGeneratorBrain
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
    public SpecialSlot mushrooms,grass;
    public MeshRenderer oobSlot;
    public float xOffset,yOffset;
    public GenericDictionary<int,Material> materialDictionary = new GenericDictionary<int, Material>();
    List<Slot> perimeterSlots = new List<Slot>();
    int perlinLastX,perlinLastY;
    public int[] perlinGrid;
    public GameObject backDrop;
    public GameObject test;
    List<Node> walledGenSlots = new List<Node>();
    public SideTerrain sideTerrainPrefab;
    public Material wallMat;
    public GameObject tree;
    public GameObject ruins;
    public override void Generate(LocationInfo li = null)
    {
        MapGenerator.inst.genGrid.fNodeRadius = roomSizeDict[GenerationRoomType.NORMAL].x/2;
        MapGenerator.inst.genGrid.CreateGrid();
        locationInfo = li;
        GeneratePerlin();
        MapManager.inst.QPremadeSlots();
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
        sideHallway();
        foreach (var item in lunch)
        {dungDict.Remove(item);}
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

    public void SpawnSpacers() /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    { 

        StartCoroutine(q());
        IEnumerator q()
        {
            Dictionary< GameObject, Vector2> spacers = new   Dictionary<GameObject,Vector2>();
            Dictionary<GameObject,Room> dict = new Dictionary<GameObject, Room>();
            int i = 1;
            foreach (var item in dungeonNodes)
            {
                GameObject spacer = Instantiate(roomSpacerPrefab,item.spawnPoint.position,Quaternion.identity);
                Room r = item.gameObject.AddComponent<Room>();
                r.Init(i);
                r.roomType = item.roomType;
                dict.Add(spacer,r);
                Vector2 v = roomSizeDict[item.roomType];
                spacers.Add(spacer,v );
                i++;
            }

            MapGenerator.inst.genGrid.UpdateGrid();
            GetWalledSlots();
            yield return new WaitForSeconds(.1f);
            foreach (var item in spacers)
            {
                Vector2 v = item.Value;
                item.Key.transform.localScale = new Vector3(v.x,1,v.y);
            }

            MapGenerator.inst.genGrid.enabled = false;
            yield return new WaitForSeconds(.1f);
            SpawnSlots(dict);
            MapManager.inst.map.UpdateGrid();
            yield return new WaitForSeconds(.1f);
            foreach (var item in MapManager.inst.map.rooms)
            {
                item.SortAnchors();
                
            }
            PerimeterSlots();
            MapManager.inst.map.UpdateGrid();
            yield return new WaitForSeconds(.1f);
        
            if(CheckIfValid()){
                yield return new WaitForSeconds(.1f);
            
                AssignStartEndRooms();
                yield return new WaitForEndOfFrame();
                //BordersAndDoors();
                RoomContent();
                foreach (var item in spacers)
                {  
                    Destroy(item.Key);
                }
                foreach (var item in gos)
                {
                    Destroy(item.gameObject);
                }
                BuildBounds();
                BattleManager.inst.hasAmbush = true;
                BattleManager.inst. iterationsTilAmbush = 2;
                UnitFactory.inst.enemies =new List<DefinedCharacter>(locationInfo.enemies) ;
                //Random.Range(5,10);
                foreach (var item in walledGenSlots)
                {
                    SideTerrain st =   Instantiate(sideTerrainPrefab,item.vPosition,Quaternion.identity);
                    st.Init();
                }
                yield return new WaitForEndOfFrame();
                MapManager.inst.map.UpdateGrid();
                MapGenerator.inst.WrapUp();
            }
            else
            {
                Debug.LogAssertion("NOT ALL SLOTS ARE ACCESSIBILE!! RESTTING!!");
                Reset();
            }
            
        }
    }

    public void GetWalledSlots(){
        foreach (var item in  MapGenerator.inst.genGrid.NodeArray)
        {
            if(item.isBlocked){
                walledGenSlots .Add(item);
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
                if(r == null){
                    Debug.LogAssertion("THE ROOM IS NULL!!!");
                
                }
                int i =  GetIdUsingPerlin(item.iGridX, item.iGridY);
                CreateSlot(item, r,i);
            
              
            }
            else if(g.Length > 1)
            {Debug.LogAssertion("TWO OVERLAPPING SLOT COLLIDERS!!!");}
            else
            {    
                CreateWall(item);
             
            }
        }

        foreach (var item in dict)
        {MapManager.inst.map.rooms. Add(item.Value);}
    }

    void CreateWall(Node item){
           Wall w = null;
             
            
                // if(MapManager.inst.canGivePremadeWall()){
                //     w = MapManager.inst.GiveWall();
                //     w.gameObject.SetActive(true);
                //     w.transform.SetParent(null);
                //     w.transform.position = item.vPosition;
                //     w.transform.rotation = Quaternion.identity;
                // }
                // else{
                  w=     Instantiate(wall,item.vPosition,Quaternion.identity);
             // }
                ForestWall fw = w as ForestWall;
                fw.ApplyDeco();
                w.ChangeMat(wallMat); 
    }

 

    void CreateSlot(Node item, Room r,int i)
    {
        Slot s = null;
        if(MapManager.inst.canGivePremadeSlot()){
            s = MapManager.inst.GivePremade();
            s.gameObject.SetActive(true);
            s.transform.SetParent(null);
            s.transform.position = item.vPosition;
            s.transform.rotation = Quaternion.identity;
        }
        else{
            s = Instantiate(slotPrefab,item.vPosition,Quaternion.identity);
      }
       
        r.slots.Add(s);
        s.gameObject.name = r.roomID +"|"+r.slots.Count;
        s.room = r;
        MapManager.inst.allSlots.Add(s);
        item.slot = s;
        s.node = item;
        s.transform.SetParent(r.transform);
        
        s.mesh.material = materialDictionary[i];
    }




    void PerimeterSlots()
    {
     
        foreach (var item in  MapManager.inst.allSlots)
        {
            List<Direction> dirs = item.func.CheckIfSideSlot();
            if(dirs.Count > 0)
            {
                if(!perimeterSlots.Contains(item))
                {
                    if(item.gameObject.activeSelf){
                        if(dirs.Count == 1){
                        perimeterSlots.Add(item);
                        if(!dirs.Contains(Direction.DOWN)){
                            GameObject g = Instantiate(tree,item.transform.position,Quaternion.identity);
                            g.transform.rotation = Quaternion.Euler(  g.transform.rotation.eulerAngles.x,Random.Range(0,360), g.transform.rotation.eulerAngles.z);
                            int i = Random.Range(0,g.transform.childCount);
                            GameObject bush =  g.transform.GetChild(i).gameObject;
                            bush.SetActive(true);
                            
                            bush.GetComponent<SortingGroup>().sortingOrder = -item.node.iGridX;
                        }
                        item.IsWall();
                        Wall w = Instantiate(wall,item.transform.position,Quaternion.identity);
                        ForestWall fw = w as ForestWall;
                        fw.ApplyDeco();
                        w.ChangeMat(wallMat);
                        item.room.wallDict[dirs[0]].Add(w);
                        w.transform.SetParent(item.room.transform);
                        item.gameObject.SetActive(false);
                        
                    }
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
                 w.ChangeMat(slotMat);
                perimeterSlots.Add(item); w.transform.SetParent(item.room.transform);
                item.gameObject.SetActive(false);
            }
        }
    }

    public bool CheckIfValid()
    {
        // Slot s = MapManager.inst.map.rooms[0].RandomSlot();
        // bool b = (bool)MapGenerator.inst.genGrid;
        // Debug.Log("b:" + b);
        // List<Slot>  filteredSlots = s.func .FilterUnadjacents(MapManager.inst.allSlots,new List<Slot>(),true);
        // foreach (var item in MapManager.inst.allSlots )
        // {
        //     if(!filteredSlots.Contains(item) && !perimeterSlots.Contains(item))
        //     {
                
               
        //         return false;
        //     }
            
        // }
      
        // bool playerSelectedRetrevial = GameManager.inst.chosenObjective == Objective.ObjectiveEnum.RETRIEVAL && GameManager.inst.chosenQuest;
        // bool debugSelectedRetrevial = ObjectiveManager.inst.predecideObjective && ObjectiveManager.inst.predecidedObjective == Objective.ObjectiveEnum.RETRIEVAL;
        // if(playerSelectedRetrevial |debugSelectedRetrevial)
        // {
        //     if(MapManager.inst.map.rooms.Count <= 4)
        //     {
        //         Debug.LogAssertion("NOT ENOUGH ROOMS FOR RETRIEVAL MISSION");
        //         Reset();
        //         return false;
        //     }

        // }

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
        (Room,Room) r = FurthestTwoRooms(MapManager.inst.map.rooms);
        MapManager.inst.map.AssignStartEnd(r.Item1,r.Item2);
      
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

        // List<Slot> rand = new List<Slot>();
        // foreach (var item in MapManager.inst.allSlots)
        // {
        //    rand.Add(item);
        // }
        // System.Random rng = new System.Random();
        // rand = rand.OrderBy(_ => rng.Next()).ToList();
        // int howManyEncounterSlots = MiscFunctions.GetPercentage(MapManager.inst.allSlots.Count,5);
    
        // for (int i = 0; i < howManyEncounterSlots; i++)
        // {
        //     if(rand[i].cont.walkable()){    Debug.Log(howManyEncounterSlots +"HMES");
        //         Instantiate(test,rand[i].transform.position,Quaternion.identity);
        //     }
        // }


        foreach (var item in MapManager.inst.allSlots)
        {
            int i = Random.Range(0,10);
            if(i == 5){
               Slot s =   MapManager.inst.RandomSlot();
                s.MakeSpecial(grass);
            }
        }

      

        foreach (var item in MapManager.inst.map.rooms)
        {
            if(MiscFunctions.FiftyFifty()){
                item.AddRoomContent();
        
                Slot s = item.RandomSlot();
                s.cont.wall = true;
                s.MakeSpecial(mushrooms);
                Transform t = s.cont.specialSlot .transform.GetChild(0);
                
                MiscFunctions.RandomYRotation(t);
            }

            if(item.roomID % 2 == 0 && item.roomType == GenerationRoomType.NORMAL)
            {
                if(10 >= Random.Range(0,100))
                {
                    Instantiate(ruins,MapManager.inst.map.RandomRoom().anchors[Room.RoomAnchors.MIDDLE].transform.position,Quaternion.identity); 
                }
            }
        }
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

    public  (Room,Room) FurthestTwoRooms(List<Room> nodeList)
    {
        float FurthestDistance = 0;
        Room n1 = null;
         Room  n2 = null;
        foreach(Room g in nodeList)
        {
            for (int i = 0; i < nodeList.Count; i++)
            {
                float ObjectDistance = Vector3.Distance(nodeList[i].transform.position, g.transform.position);
                if (ObjectDistance > FurthestDistance)
                {
                    n1 = g;
                    n2 = nodeList[i];
                    FurthestDistance = ObjectDistance;
                }
            }
        }
        return (n1,n2);
    }

 
 

    int GetIdUsingPerlin(int x, int y)
    {
       
        float mag = 20;
        int newNoise = 5;
        float raw_perlin = Mathf.PerlinNoise((x -newNoise) / mag , (y -newNoise) / mag);
        float clamp_perlin = Mathf.Clamp01(raw_perlin); 
        float scaled_perlin = clamp_perlin * materialDictionary.Count;
 
        // Replaced 4 with tileset.Count to make adding tiles easier
        if(scaled_perlin == materialDictionary.Count)
        {
            scaled_perlin = (materialDictionary.Count - 1);
        }
        return Mathf.FloorToInt(scaled_perlin);
    }


    void GeneratePerlin()
    {
 
        // Set up the texture and a Color array to hold pixels during processing.
      
        float randomorg = Random.Range(0, 100);
        float Y =  locationInfo.mapSize.y;
        float X = locationInfo.mapSize.x;
 
        // For each pixel in the texture...
        float y = 0.0F;
        float scale = 1;
        perlinGrid = new int[(int) X*(int)Y];
        while (y < Y)
        {
            float x = 0.0F;
            while (x < X)
            {
 
                float xCoord = randomorg + x / X * scale;
                float yCoord = randomorg + y / Y * scale;
                float sample = Mathf.PerlinNoise(xCoord, yCoord);
 
                if (sample == Mathf.Clamp(sample, 0, 0.5f))
                  perlinGrid[(int)y * (int) X + (int)x] = 0;
                else if (sample == Mathf.Clamp(sample, 0.5f, 0.6f))
                   perlinGrid[(int)y * (int) X + (int)x] = 1;
                else if (sample == Mathf.Clamp(sample, 0.6f, 0.7f))
                    perlinGrid[(int)y * (int) X + (int)x] = 2;
                else if (sample == Mathf.Clamp(sample, 0.7f, 0.8f))
                    perlinGrid[(int)y * (int) X + (int)x] = 3;
                else if (sample == Mathf.Clamp(sample, 0.8f, 1f))
                     perlinGrid[(int)y * (int) X + (int)x] = 4;
                else
                     perlinGrid[(int)y * (int) X + (int)x] = 5;
 
 
                x++;
            }
            y++;
        }
 
 
        // // Copy the pixel data to the texture and load it into the GPU.
        // noiseTex.SetPixels(pix);
        // noiseTex.Apply();
 
        // mappreview.texture = noiseTex;
        // worldmap = noiseTex;
    }
 

    public override void BuildBounds(){
        Vector2 v = locationInfo.mapSize;
        float y = mapBoundPrefab.transform.position.y;
        MapBounds l = Instantiate(mapBoundPrefab,new Vector3(v.x,y,0),Quaternion.identity);
        MapBounds r = Instantiate(mapBoundPrefab,new Vector3(-v.x,y,0),Quaternion.identity);
        MapBounds t = Instantiate(mapBoundPrefab,new Vector3(0,y,v.y),Quaternion.identity);
        MapBounds b = Instantiate(mapBoundPrefab,new Vector3(0,y,-v.y),Quaternion.identity);
        GameObject  tbd =   Instantiate(backDrop,t.backdropAnchors[3]);
        GameObject  rbd =   Instantiate(backDrop,r.backdropAnchors[1]);
        GameObject  lbd =   Instantiate(backDrop,l.backdropAnchors[2]);
        GameObject  brbd =   Instantiate(backDrop,b.backdropAnchors[1]);
        GameObject  blbd =   Instantiate(backDrop,b.backdropAnchors[2]);
        float yScale = 500;
        tbd.transform.localScale = new Vector3(tbd.transform.localScale.x,yScale,tbd.transform.localScale.z);
        rbd.transform.localScale = new Vector3(rbd.transform.localScale.x,yScale,rbd.transform.localScale.z);
        rbd.transform.localRotation = Quaternion.Euler(0,90,0);
        lbd.transform.localScale = new Vector3(lbd.transform.localScale.x,yScale,lbd.transform.localScale.z);
        lbd.transform.localRotation = Quaternion.Euler(0,-90,0);

        brbd.transform.localScale = new Vector3( brbd.transform.localScale.x,yScale, brbd.transform.localScale.z);
        brbd.transform.localRotation = Quaternion.Euler(0,90,0);
        blbd.transform.localScale = new Vector3(blbd.transform.localScale.x,yScale,blbd.transform.localScale.z);
        blbd.transform.localRotation = Quaternion.Euler(0,-90,0);


      
        l.Init(v,slotMat);
        r.Init(v,slotMat);
        t.Init(v,slotMat);
        b.Init(v,slotMat);
        tbd.transform.SetParent(null);
        rbd .transform.SetParent(null);
        lbd .transform.SetParent(null);
        brbd .transform.SetParent(null);
        blbd.transform.SetParent(null);

        l.transform.localScale = new Vector3(l.transform.localScale.x,50,l.transform.localScale.z);
        r.transform.localScale = new Vector3(r.transform.localScale.x,50,r.transform.localScale.z);
        t.transform.localScale = new Vector3(t.transform.localScale.x,50,t.transform.localScale.z);

        // l.gameObject.SetActive(false);
        // r.gameObject.SetActive(false);
        // t.gameObject.SetActive(false);
        //b.transform.localScale = new Vector3(b.transform.localScale.x,50,b.transform.localScale.z);
       
        CameraBoundManager.inst.Init(v);
    }
 

   
}