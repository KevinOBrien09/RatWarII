using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System.Linq;
using UnityEngine.Events;
using DG.Tweening;
using UnityEngine.SceneManagement;

public class HPBoxResizer : MonoBehaviour
{
    public GenericDictionary<int,float> dicty = new GenericDictionary<int, float>();
    public RectTransform rt;
    public void Resize(int hp,int res){
        float x = 0;
        int larger = res;
        if(hp > res){
            larger = hp;
        }
        if(dicty.ContainsKey(larger)){
            x = dicty[larger];
        }
        else{
            x = dicty[4];
            Debug.LogWarning("Larger Value that expected encountered!");
        }
       
        rt.sizeDelta = new Vector2(x,rt.sizeDelta.y);
    }
}