using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolableObject : MonoBehaviour
{

    public virtual void Go(float v)
    {Debug.Log("Base function v: " + v);}

    public  virtual void Reposition(Vector3 p,Quaternion r)
    {
        transform.position = p;
        transform.rotation = r;
        //Quaternion.Euler(r.x,r.y,r.z);
    }

    public virtual void Reset()
    {
        Debug.Log("Reset");
    }

}