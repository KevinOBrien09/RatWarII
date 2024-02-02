using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "Effect/BaseEffect")]
public class Effect : ScriptableObject

{
    public virtual void Go(){
        Debug.Log(name);
    }
}