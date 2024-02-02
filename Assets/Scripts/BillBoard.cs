using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BillBoard : MonoBehaviour
{   
    public bool billboard = true;
    public bool clampX;
    void LateUpdate()
    {
        if(billboard)
        {
          
            if(!clampX)
            {
                transform.eulerAngles = new Vector3(transform.eulerAngles.x,transform.eulerAngles.y,transform.eulerAngles.z);
            }
            else
            {
                transform.eulerAngles = new Vector3(0,transform.eulerAngles.y,0);
            }
        }
    }
}
