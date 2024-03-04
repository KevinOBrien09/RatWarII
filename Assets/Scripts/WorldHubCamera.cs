using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using DG.Tweening;

public class WorldHubCamera : Singleton<WorldHubCamera>
{
    public Camera cam;
    Vector3 offset;
    float ogFOV;
    public bool fuckOff;
    void Start(){
        ogFOV = cam.fieldOfView;
        offset = transform.position;
    }
    public void Zoom(UnityAction a)
    {
        cam.DOFieldOfView(0,.15f).OnComplete(()=>
        {
            a.Invoke();
            cam.DOFieldOfView(ogFOV,.15f);
            //cam.fieldOfView = ogFOV;
        });
    }

    public void ZoomOut(UnityAction a)
    {
        cam.DOFieldOfView(179,.15f).OnComplete(()=>
        {
            a.Invoke();
            cam.DOFieldOfView(ogFOV,.15f);
            //cam.fieldOfView = ogFOV;
        });
    }

    public void Move(Transform t,UnityAction a)
    {
        float tm = .4f;
        transform.DOMove(t.position,tm );
        transform.DORotate(t.rotation.eulerAngles,tm ).OnComplete(()=>{
            a.Invoke();
        });
    }
}
