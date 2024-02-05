using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StatusEffectStack : MonoBehaviour
{ 
    public TextMeshProUGUI stackNum;
    public Image pic;
    Skill s;
    int stacks;
    public void Init(Skill skill){
        pic.sprite = skill.statusEffectIcon; 
        s = skill;
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