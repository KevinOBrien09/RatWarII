using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using  UnityEngine.AI;
public class OverworldUnit: MonoBehaviour
{
    public Unit battleUnit;
    public   OverworldUnit followTarget;
    public NavMeshAgent agent;
    public bool facingRight;
    public Transform graphicHolder;
    public Animator animator;
    public void Move(Vector3 targetPos){
        agent.SetDestination(targetPos);
        Flip(targetPos);
        // Vector2 input = new Vector2(Input.GetAxisRaw("Horizontal"),Input.GetAxisRaw("Vertical"));
        // Vector3 move = transform.right * input.x + transform.forward * input.y;
        // move = move * 5 * Time.deltaTime;
        // move =  Vector3.Normalize(move);
        // controller.Move(move);
    }

    void Update(){
        animator.SetFloat("Walk",agent.velocity.magnitude);
    }

    public virtual void Flip(Vector3 v)
    {
        if( transform.position.x != v.x)
        {
            bool movingRight = transform.position.x <= v.x ;
            if(movingRight)
            {transform.localScale = Vector3.one; 
            
            facingRight = true;}
            else
            { transform.localScale = new Vector3(-1,1,1); 
         
            facingRight = false;}
        }
    }

}