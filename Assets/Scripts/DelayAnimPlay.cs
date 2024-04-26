using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.Events;


public class DelayAnimPlay : MonoBehaviour
{
    public Animator anim;
    public string startState;
    public Vector2 range;
    IEnumerator Start()
    {   
        yield return new WaitForSeconds(Random.Range(range.x,range.y));
        anim.Play(startState);
    }

}