using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class WorldMapCamera : Singleton<WorldMapCamera>
{
    public Vector3 offset;
    public Camera camera_;
void Start(){
    offset = transform.position;
}
    public void Focus(Transform t){
        transform.DOMove(new Vector3(t.position.x+1,t.position.y,transform.position.z) ,.2f);
        camera_.DOFieldOfView(20,.2f);
      
    }

    public void Reset(bool tween){
          LocationDetailHandler.inst.Hide();
        float t = 0;
        if(tween){
            t = .2f;
        }
        transform.DOMove(offset ,t);
        camera_.DOFieldOfView(60,t);

    }

    // void OnDisable()
    // {Reset(false);}


}