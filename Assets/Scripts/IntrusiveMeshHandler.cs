using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;
public class IntrusiveMeshHandler : MonoBehaviour
{
    public Material m;
    public MeshRenderer mr;
    public SortingGroup sortingGroup;
    public BoxCollider bc;
    public Color defaultColour,transColour;
    public float alphaValue;
    void Start()
    {
        m = Instantiate(mr.material);
        mr.material = m;
        defaultColour = m.color;
        transColour = new Color(m.color.r,m.color.g,m.color.b,alphaValue);


    }

    public void Reset(){
        m.color = defaultColour;
    }

    public void MakeTrans(){
        
        m.color = transColour;
    }

 

   
}