using UnityEngine;
public class MapGeneratorBrain : MonoBehaviour
{
    public AudioClip overrideLocationInfoMusic;
    public Material slotMat;
    public LocationInfo locationInfo;
    public MapBounds mapBoundPrefab;
    public virtual void Generate(LocationInfo li = null)
    { Debug.Log("Begin Generating Dungeon"); }

    public virtual void Reset(){
        Debug.Log("RESET");
      

    }

    public virtual void BuildBounds(){
        Vector2 v = locationInfo.mapSize;
        float y = mapBoundPrefab.transform.position.y;
        MapBounds l = Instantiate(mapBoundPrefab,new Vector3(v.x,y,0),Quaternion.identity);
        MapBounds r = Instantiate(mapBoundPrefab,new Vector3(-v.x,y,0),Quaternion.identity);
        MapBounds t = Instantiate(mapBoundPrefab,new Vector3(0,y,v.y),Quaternion.identity);
        MapBounds b = Instantiate(mapBoundPrefab,new Vector3(0,y,-v.y),Quaternion.identity);

        MapBounds bL = Instantiate(mapBoundPrefab,new Vector3(-v.x,y,-v.y),Quaternion.identity);
        MapBounds bR = Instantiate(mapBoundPrefab,new Vector3(v.x,y,-v.y),Quaternion.identity);
        MapBounds tR = Instantiate(mapBoundPrefab,new Vector3(v.x,y,v.y),Quaternion.identity);
        MapBounds tL = Instantiate(mapBoundPrefab,new Vector3(-v.x,y,v.y),Quaternion.identity);
        l.Init(v,slotMat);
        r.Init(v,slotMat);
        t.Init(v,slotMat);
        b.Init(v,slotMat);

        bL.Init(v,slotMat);
        bR.Init(v,slotMat);
        tR.Init(v,slotMat);
        tL.Init(v,slotMat);

        CameraBoundManager.inst.Init(v);
    }

}