using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public enum GenerationRoomType{NORMAL,BIG,SIDEHALL,VERTHALL,HORIDOUBLE,VERTDOUBLE}
public class DungeonNode : MonoBehaviour
{  
    public GenerationRoomType roomType;
    public Node originalNode;
    public List<DungeonNode> neighbours = new List<DungeonNode>();
    public Transform rayShooter;
    public Transform spawnPoint;
    public bool isHall;
    public float rayDist = 25;

    void Start(){
        spawnPoint = transform;
    }
    public void GetNeighbours()
    {
        List<Vector3> directions = new List<Vector3>();
        directions.Add(rayShooter.forward);
        directions.Add(-rayShooter.forward);
        directions.Add(rayShooter.right);
        directions.Add(-rayShooter.right);

        foreach (var item in directions)
        {
            RaycastHit hit ;
            if(Physics.Raycast(rayShooter.position,item * rayDist,maxDistance:rayDist,hitInfo: out hit))  
            {
                DungeonNode s = null;
                if(hit.collider.gameObject !=null)
                {
                    if(hit.collider.gameObject.TryGetComponent<DungeonNode>(out s))
                    { neighbours.Add(s); }
                }
            }
        }
    }

    public void MarkAsSideHallway(){
        roomType = GenerationRoomType.SIDEHALL;
         isHall = true;
         spawnPoint = transform;
    }

    public void MarkAsVertHallway(){
        roomType = GenerationRoomType.VERTHALL;
        isHall = true;
           spawnPoint = transform;
    }

    public void MarkAsBigRoom(List<DungeonNode> yum)
    {
        List<Transform> tran = new List<Transform>();
        foreach (var item in yum)
        {tran.Add(item.transform);}
        tran.Add(transform);
       
        //spawnPoint = new GameObject("spawn").transform;
        spawnPoint.position = MiscFunctions.FindCenterOfTransforms(tran);
        roomType = GenerationRoomType.BIG;

        
    }

    public void MarkAsHORIDouble(DungeonNode partner)
    {
        List<Transform> tran = new List<Transform>();
        tran.Add(transform);
        tran.Add(partner.transform);
        transform.position = MiscFunctions.FindCenterOfTransforms(tran);
        roomType = GenerationRoomType.HORIDOUBLE;
           spawnPoint = transform;
    }

    public void MarkAsVERTDouble(DungeonNode partner)
    {
        List<Transform> tran = new List<Transform>();
        tran.Add(transform);
        tran.Add(partner.transform);
        transform.position = MiscFunctions.FindCenterOfTransforms(tran);
        roomType = GenerationRoomType.VERTDOUBLE;
           spawnPoint = transform;
    }

  
}