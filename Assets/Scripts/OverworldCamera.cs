using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.Events;


public class OverworldCamera : Singleton<OverworldCamera>
{  

    public Camera cam;
    public Transform target;
    public float smoothspeed = 0.125f;
    public Vector3 offset;
    void Update()
    {
        LockOn();
    }

    void LockOn()
    {
        if(target != null)
        {
            Vector3 desiredposition = target.position + offset;
            Vector3 smoothedposition = Vector3.Lerp(transform.position, desiredposition, smoothspeed*Time.deltaTime);
            transform.position = smoothedposition;
        }
       
    }
    public void FOVChange(float target, UnityAction a){
        cam.DOFieldOfView(target,.3f).OnComplete(()=>{a.Invoke();});
    }

}