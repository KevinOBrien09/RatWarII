using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using TMPro;

public class Minimap : Singleton<Minimap>
{
    public RectTransform rt;
    public Transform minimapCam;
    public Camera cam;
    public Vector2 shown,hidden;
    public CanvasGroup cg;
    void Start(){
       // rt.DOAnchorPosX(hidden.x,0);
cg.DOFade(0,0);
    }
    public void Show(){
        Vector3 uP = BattleManager.inst.currentUnit.transform.position;
        minimapCam.transform.position = new Vector3(uP.x,minimapCam.transform.position.y,uP.z);
        //rt.DOAnchorPosX(shown.x,.2f);
        cg.DOFade(1,.2f);
    }

    public void Hide(){
        //rt.DOAnchorPosX(hidden.x,.2f);
        cg.DOFade(0,.2f);
    }

    public void ResizeFOV(int radius){
        int i = radius * 5;
        i += 10;
        cam.orthographicSize = i;
    }
}