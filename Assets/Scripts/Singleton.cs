using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Singleton<T> : MonoBehaviour where T:MonoBehaviour
{
    public static T inst;
    protected virtual void Awake()
    {
        if(inst == null)
        {inst = this as T;}
        else
        {Debug.LogAssertion("There are two " + typeof(T) + "'s! " + "One on " + gameObject.name + " and another on " + inst.gameObject.name);}
    }
}
