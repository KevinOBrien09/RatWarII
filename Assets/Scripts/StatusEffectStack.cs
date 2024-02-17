using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class StatusEffectStack : MonoBehaviour,IPointerEnterHandler,IPointerExitHandler
{ 
    public GameObject stackGO;
    public TextMeshProUGUI stackNum;
    public RawImage pic;
    int stacks;

    public StackInfo stackInfo;


    public void Init(Texture2D sprite,SlotContents sc = null,string seName = null,string seDesc = null){
        pic.texture = sprite;
        stackInfo.gameObject.SetActive(false);
        stackGO.SetActive(true);
        if(sc != null)
        {
            if(sc.unStackable)
            {stackGO.SetActive(false);}
            stackInfo.Apply(sc.contentName,sc.contentDesc);
        }   
        else if(seName != null)
        {stackInfo.Apply(seName,seDesc);}
 
          
        Stack();
    }


    public void Stack()
    {
        stacks ++;
        stackNum.text = stacks.ToString();
    }

    public void RemoveStack(){
        stacks --;
        stackNum.text = stacks.ToString();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
       stackInfo.ToggleOn();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
         stackInfo.gameObject.SetActive(false);
    }
}