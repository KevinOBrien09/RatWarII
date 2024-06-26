    using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class InventoryTab : MonoBehaviour
{
    public InventoryViewer viewer;
    public ItemCatagory catagory;
    public float ogY,targetY;
    public bool isAll;
    public RectTransform rt;
    public Color32 red;
    public Image icon,button;
    void Start(){
        ogY = rt.anchoredPosition.y;
        Down();

    }
    public void Open(){
       
        if(!isAll){
            viewer.BuildCatagory(catagory);
        }
        else{
            viewer.BuildAll();
        }
    
        
    }

    public void Up(){
        icon.color = Color.black;
        button.color = red;
        rt.DOAnchorPosY(targetY,.1f);
    }

    public void Down(){
        button.color = Color.black;
        icon.color = red;
        rt.DOAnchorPosY(ogY,.1f);
    }
}