using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;

public class BattleNumber : PoolableObject
{
    public TextMeshPro textMeshProUGUI;
    public void Go(string value,Color32 color,Vector3 v){
        textMeshProUGUI.color = color;
        textMeshProUGUI.text = value;
        transform.position = new Vector3(v.x,v.y,v.z+1);
        transform.DOMoveY(v.y+2,.5f).OnComplete(()=>{gameObject.SetActive(false);});
        gameObject.SetActive(true);
    }

}