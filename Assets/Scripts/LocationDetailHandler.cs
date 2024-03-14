using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;
using UnityEngine.UI;
public class LocationDetailHandler : Singleton<LocationDetailHandler>
{
    public RectTransform rt;
    public float shown,hidden;
    public Image locationPicture;
    public TextMeshProUGUI locationName,stage,description;
    public bool open;
    void Start(){
           rt.DOAnchorPosX(hidden,0).OnComplete(()=>{

            gameObject.SetActive(false);
           });
    }
    public void Show(LocationInfo i){
        if(!open){
        gameObject.SetActive(true);
        rt.DOAnchorPosX(hidden,0);
        rt.DOAnchorPosX(shown,.25f);
        open = true;
        }
        EditDetails(i);
    
    }

    public void EditDetails(LocationInfo info)
    {
        locationName.text = info.locationName;
     //   stage.text = info.stage. GetStage();
        description.text = info.desc;

    }

    public void Hide(){
        rt.DOAnchorPosX(hidden,.25f).OnComplete(()=>{
        open = false;
            gameObject.SetActive(false);
           });
    }
}