using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
public class Door : MonoBehaviour 
{
    public Grid_ grid_;
    public GameObject doorSlot;
    public GameObject wall;
    public void MakeDoor(Grid_ g)
    {
        grid_ = g;
        List<Vector3> v = new List<Vector3>();
        for (int i = 0; i < grid_.iGridSizeY; i++)
        {
            v.Add(new Vector3(transform.position.x,doorSlot.transform.position.y,grid_.NodeArray[0,i].vPosition.z));
        }

        System.Random rng = new System.Random();
        v =   v.OrderBy(_ => rng.Next()).ToList();
        bool doorSpawned = false;

        foreach (var item in v)
        {
            if(!doorSpawned){
                Instantiate(doorSlot,item,Quaternion.identity);
                doorSpawned = true;
            }
            else{
  Instantiate(wall,new Vector3(item.x,wall.transform.position.y,item.z),Quaternion.identity);
            }
        }

    }

}