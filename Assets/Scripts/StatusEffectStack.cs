using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StatusEffectStack : MonoBehaviour
{ 
    public TextMeshProUGUI stackNum;
    public Image pic;
    int stacks;
    public void Init(Sprite sprite){
        pic.sprite = sprite;
        
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


}