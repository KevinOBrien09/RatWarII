using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using DG.Tweening;



public class SwampGeneratorBrain : PerlinGeneratorBrain
{
    //ramp up scale non inverted water = mountain
    public BoatGrid waterGrid;
    public Slot slotPrefab,waterSlot;
    public Material mud;
    public SpecialSlot mushrooms;
    public List<Slot> landSlots = new List<Slot>();
    public List<Slot> waterSlots = new List<Slot>();
    public Material red,blue;
    List<List<Slot>> landMasses = new List<List<Slot>>();
    List<Node> waterNodes = new List<Node>();
    public bool invertWater,randomIslandColour;
    public int islandSizeThreshHold;
   
    public override void Generate(LocationInfo li = null)
    {
        locationInfo = li;
        MapManager.inst.mapQuirk = MapQuirk.ISLANDS;
        StartCoroutine(q());
        IEnumerator q()
        {
            
            CreateFakeRoom();
            GeneratePerlin(li.mapSize);
            yield return   new WaitForSeconds(.1f);
            foreach (var item in MapManager.inst.map.NodeArray)
            {             
                Slot s = CreateSlot(item);
              //  GenerateTile(item,s);
            }

            WeatherManager.inst.Raining(locationInfo.mapSize);
            ParseIslands();
            RemoveSmallIslands();
            SpawnWaterSlots();
            KillProblematicWaterTiles();
            yield return   new WaitForSeconds(.1f);
            ReparseIslands();
            InitRooms();
         
            MakeWaterGrid();
            
            AddContent();
            
            BuildBounds();
            SetStartRoom();
            Island i = MapManager.inst.map.startRoom as Island;
           

            UnitMover.inst.boatMover = new BoatMover();
            
            UnitMover.inst.boatMover.Init(this);
            UnitMover.inst.boatMover.boat = waterGrid.SpawnBoat(i);
            yield return   new WaitForSeconds(.1f);
            waterGrid.UpdateGrid();
            MapManager.inst.map.UpdateGrid();
            MapGenerator.inst.WrapUp();
        }
    }

    public void CreateFakeRoom(){
        MapManager.inst.map.startRoom  = MapManager.inst.gameObject.AddComponent<Room>();
    }

    public void SetStartRoom()
    {
        Destroy(MapManager.inst.map.startRoom);
        int count = 0;
        foreach (var item in MapManager.inst.map.rooms)
        {
            if(item == waterGrid.ocean){
                continue;
            }
            if(item.slots.Count > count){
                MapManager.inst.map.startRoom = item;
                count = item.slots.Count;
            }
        }
    }

    public Slot CreateSlot(Node item)
    {
        int p = perlinGrid[ new Vector2(item.iGridX,item.iGridY)] ;
        Slot w =  null;
        switch (p)
        {
            case 1:  
            if(invertWater){
            SlotSetUp(MakeLandSlot(w,item),item) ;
            }
            else{
                waterNodes.Add(item);
            }
         
              // 
            break;

            case 2: 
             if(invertWater){
          SlotSetUp(MakeLandSlot(w,item),item) ;
            }
            else{
            waterNodes.Add(item);
            }
            break;

            case 3:  
             if(invertWater){
           SlotSetUp(MakeLandSlot(w,item),item) ;
            }
            else{
             waterNodes.Add(item);
            }
            break;
            
            default:
            if(invertWater){
            waterNodes.Add(item);
            }
            else{
            SlotSetUp(MakeLandSlot(w,item),item) ;
            }
            break;
        }
        return w;
    }

    public void SlotSetUp(Slot s,Node n)
    {
        Room r =  MapManager.inst.map.startRoom;  //might be bug
        r.slots.Add(s);
        s.gameObject.name = r.roomID +"|"+r.slots.Count;
        s.room = r;
        MapManager.inst.allSlots.Add(s);
        n.slot = s;
        s.node = n;
    }

    void SpawnWaterSlots()
    {
        foreach (var item in waterNodes)
        {
            MakeWaterSlot(item);
        }
    }

    Slot MakeLandSlot(Slot w,Node item)
    {
        if(MapManager.inst.canGivePremadeSlot())
        {
            w = MapManager.inst.GivePremade();
            w.gameObject.SetActive(true);
            w.transform.SetParent(null);
            w.transform.position = item.vPosition;
            w.transform.rotation = Quaternion.identity;
        }
        else
        {
            w = Instantiate(slotPrefab,item.vPosition,Quaternion.identity);
        }
        w.mesh.material = slotMat;
        landSlots.Add(w);
        return w;
    }
    
    void MakeWaterSlot(Node item)
    {
        Slot w =  Instantiate(waterSlot,item.vPosition,Quaternion.identity);
        w.mesh.material = mud;
        w.isWater = true;
        waterSlots.Add(w);
        SlotSetUp(w,item);
       // w.transform.name = "WATER:" + w.transform.name;
    }



    public void AddContent(){

        int p = MiscFunctions.GetPercentage(MapManager.inst.allSlots.Count,1);
        for (int i = 0; i < p; i++)
        {
            Slot s =   MapManager.inst.RandomSlot();
            s.cont.wall = true;
            s.MakeSpecial(mushrooms);
        }
       
    }

    public void InitRooms(){
        int i =0;
        foreach (var mass in landMasses)
        {
            GameObject G = new GameObject();
            Island r =   G.AddComponent<Island>();
            MapManager.inst.map.rooms.Add(r);
            r.Init(i);
            List<Transform> t = new List<Transform>();
            foreach (var tile in mass)
            {t.Add(tile.transform);}
            G.transform.position = MiscFunctions.FindCenterOfTransforms(t);
           
            foreach (var tile in mass)
            {
                r.slots.Add(tile);
                tile.transform.SetParent(G.transform);
                tile.room = r;
                MapManager.inst.map.startRoom.slots.Remove(tile);
            }
            i++;
        }
    }


    public void ParseIslands()
    {
        List<Slot> unaccLandSlots = new List<Slot>(landSlots);
        loop();
        void loop()
        {
            System.Random rng = new System.Random();
            List<Slot> shuff =  unaccLandSlots.OrderBy(_ => rng.Next()).ToList();
            if(shuff.Count != 0)
            {
                Slot s = null;
                for (int i = 0; i < shuff.Count; i++)
                {
                    Slot q = shuff[i];
                    if(q.cont.walkable()){
                        s = q;
                        break;
                    }
                }
                if(s != null)
                {
                    List<Slot> island =  s.func.FilterUnadjacents(unaccLandSlots,new List<Slot>(),true);
                    if(island.Count > 0)
                    {
                        landMasses.Add(island);
                        foreach (var item in island)
                        {unaccLandSlots.Remove(item);}
                    }
                    else
                    {
                        //This is for 1x1 isolated tiles.
                        island.Add(s);
                        unaccLandSlots.Remove(s);
                        landMasses.Add(island);
                    }
                    
                    if(unaccLandSlots.Count != 0)
                    {loop();}
                    else
                    { RandomColourIslands(); }
                }
                else
                { RandomColourIslands(); }

                void RandomColourIslands()
                {
                    if(randomIslandColour){
                        foreach (var mass in landMasses)
                        {
                            var color1 = Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f);
                            foreach (var tile in mass)
                            {
                                Material m = Instantiate(red);
                                m.color = color1;;
                                tile.mesh.material = m;
                            } 
                        }
                    }
                }
            }
        }
       
    }


    void ReparseIslands(){
        landMasses.Clear();
        ParseIslands();
    }


    void KillProblematicWaterTiles()
    {
        List<Slot> beachSlots = new List<Slot>();
        foreach (var item in landSlots)
        {
            List<Slot> q = item.func.GetNeighbouringSlots();
            foreach (var neigh in q)
            {
                if(neigh.isWater){
                    beachSlots.Add(item);
                    break;
                }
            }
        }

        foreach(var d in beachSlots)
        {
            foreach (var item in landMasses)
            {
                if(item.Contains(d))
                {item.Remove(d);}
            }
            Node n = d.node;
            landSlots.Remove(d);
            KillSlot(d);
            MakeWaterSlot(n);
        }
    }


    void MakeWaterGrid(){
       
        waterGrid.ResizeGrid( locationInfo.mapSize);
        waterGrid.CreateGrid();
        waterGrid.SpawnSlots();
        waterGrid.ParseOcean();

    }
    
    void RemoveSmallIslands()
    {
        List<List<Slot>> clone = new List<List<Slot>>(landMasses);
        foreach (var island in clone)
        {
            if(island.Count <= islandSizeThreshHold)
            {
                foreach (var slot in island)
                {
                    
                    waterNodes.Add(slot.node);
                    landSlots.Remove(slot);
                    KillSlot(slot);
                    
                }
                landMasses.Remove(island);
            }
        }
    }

    void KillSlot(Slot slot){
        slot.room.slots.Remove(slot);
        slot.room = null;
        MapManager.inst.allSlots.Remove(slot);

        slot.node.slot = null;
            slot.node = null;
        Destroy(slot.gameObject);

    }
}