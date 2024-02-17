using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StackInfo : MonoBehaviour
{
    public TextMeshProUGUI stackInfo;
    public RectTransform frame,main;
    public string t1,t2;
    public void Apply(string _stackName,string _stackInfo)
    {
        t1 = _stackName;
        t2 = _stackInfo;
        
       
    }

    public void ToggleOn(){
        gameObject.SetActive(true);
        stackInfo.text ="<u>"+t1+"</u><br>"+  t2;
        StartCoroutine(Q());
        IEnumerator Q(){
            yield return new WaitForEndOfFrame();
            frame.sizeDelta = new Vector2(main.sizeDelta.x,main.sizeDelta.y) ;
        }
        
    }
}