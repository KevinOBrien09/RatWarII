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
   public float baseFOV;
    void Start(){
        baseFOV = cam.fieldOfView;
    }
    void Update()
    {
        LockOn();
        if(!BattleManager.inst.inBattle && !Menu.inst.open){
      Zoom();
        }
  
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



     void Zoom(){
        var fov  = Camera.main.fieldOfView;
 
        float i =  InputManager.inst.player.GetAxis("ScrollWheel")/10;
        fov -=  i * 55;
        fov = Mathf.Clamp(fov, 16, baseFOV);
        Camera.main.fieldOfView = fov;
    }

}