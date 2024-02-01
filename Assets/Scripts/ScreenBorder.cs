using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenBorder : MonoBehaviour
{
    public Direction direction;
    void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.name == "CameraHolder"){
            CamFollow.inst.allowedCameraMovement[direction] = false;
        }
    }

     void OnTriggerExit(Collider other)
    {
        if(other.gameObject.name == "CameraHolder"){
           CamFollow.inst.allowedCameraMovement[direction] = true;
        }
    }
}
