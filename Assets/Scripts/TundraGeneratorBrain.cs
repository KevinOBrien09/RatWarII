using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using DG.Tweening;



public class TundraGeneratorBrain : PerlinGeneratorBrain
{
    public Slot slotPrefab;
    public Unit oneTileHigh,twoTileHigh;
   
    public GenericDictionary<int, Material> materialDictionary = new  GenericDictionary<int,Material>();
    public override void Generate(LocationInfo li = null)
    {
        locationInfo = li;
        StartCoroutine(q());
        IEnumerator q()
        {
            CreateFakeRoom();
            GeneratePerlin(li.mapSize);
            yield return   new WaitForSeconds(.1f);
            foreach (var item in MapManager.inst.map.NodeArray)
            {             
                Slot s = CreateSlot(item,perlinGrid[ new Vector2(item.iGridX,item.iGridY)]);
                GenerateTile(item,s);
            }
            WeatherManager.inst.Snowing(locationInfo.mapSize);
            BuildBounds();
            yield return   new WaitForSeconds(.1f);
            MapManager.inst.map.UpdateGrid();
            MapGenerator.inst.WrapUp();
        }
    }

    public void CreateFakeRoom(){
        MapManager.inst.map.startRoom  = MapManager.inst.gameObject.AddComponent<Room>();
    }

    public void GenerateTile(Node item,Slot s)
    {
        int p = perlinGrid[ new Vector2(item.iGridX,item.iGridY)] ;
        Unit w =  null;
        switch (p)
        {
            case 1:
            w=  Instantiate(oneTileHigh,item.vPosition,Quaternion.identity);
            break;

            case 2:
            w = Instantiate(twoTileHigh,item.vPosition,Quaternion.identity);
            break;

            case 3:
            w = Instantiate(twoTileHigh,item.vPosition,Quaternion.identity);
            break;
            
            default: return;
        }
        s.cont.unit = w;
        w.slot = s;
        w.GetComponent<Wall>(). ChangeMat(slotMat);

    }


  

    Slot CreateSlot(Node item,int i)
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
       Room r =  MapManager.inst.map.startRoom;
        r.slots.Add(s);
        s.gameObject.name = r.roomID +"|"+r.slots.Count;
        s.room = r;
        MapManager.inst.allSlots.Add(s);
        item.slot = s;
        s.node = item;
      //  s.transform.SetParent(r.transform);
        
        s.mesh.material = materialDictionary[i];

        return s;
    }
 
}