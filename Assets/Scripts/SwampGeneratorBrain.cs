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
    public Slot slotPrefab,waterSlot;
    public Material mud;
    public SpecialSlot mushrooms;
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
                Slot s = CreateSlot(item);
              //  GenerateTile(item,s);
            }

            WeatherManager.inst.Raining(locationInfo.mapSize);
            BuildBounds();
            AddContent();
            yield return   new WaitForSeconds(.1f);
            MapManager.inst.map.UpdateGrid();
            MapGenerator.inst.WrapUp();
        }
    }

    public void CreateFakeRoom(){
        MapManager.inst.map.startRoom  = MapManager.inst.gameObject.AddComponent<Room>();
    }

    public Slot CreateSlot(Node item)
    {
        int p = perlinGrid[ new Vector2(item.iGridX,item.iGridY)] ;
        Slot w =  null;
        switch (p)
        {
            case 1:
            w=  Instantiate(waterSlot,item.vPosition,Quaternion.identity);
            w.mesh.material = mud;
            break;

            case 2:
            w=  Instantiate(waterSlot,item.vPosition,Quaternion.identity);
            w.mesh.material = mud;
            break;

            case 3:
            w=  Instantiate(waterSlot,item.vPosition,Quaternion.identity);
            w.mesh.material = mud;
            break;
            
            default:

            if(MapManager.inst.canGivePremadeSlot())
            {
                w = MapManager.inst.GivePremade();
                w.gameObject.SetActive(true);
                w.transform.SetParent(null);
                w.transform.position = item.vPosition;
                w.transform.rotation = Quaternion.identity;
            }
                else{
                w = Instantiate(slotPrefab,item.vPosition,Quaternion.identity);
            }
            w.mesh.material = slotMat;
            break;
        }
        Room r =  MapManager.inst.map.startRoom;
        r.slots.Add(w);
        w.gameObject.name = r.roomID +"|"+r.slots.Count;
        w.room = r;
        MapManager.inst.allSlots.Add(w);
        item.slot = w;
        w.node = item;


        return w;
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
}