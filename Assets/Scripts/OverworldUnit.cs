using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using  UnityEngine.AI;
using DG.Tweening;
public class OverworldUnit: MonoBehaviour
{
    public Unit battleUnit;
    public   OverworldUnit followTarget;
    public NavMeshAgent agent;
    public bool facingRight;
    public Transform graphicHolder;
    public Animator animator;
    public CharacterGraphic graphic;
    public SpriteRenderer selectedSignifier;
    public  SoundData selectClick;
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

    public void ToggleSelected(){
        PortraitManager.inst.dict[battleUnit.character.ID].interactor.gameObject.SetActive(true);
        selectedSignifier.gameObject.  SetActive(true);
        selectedSignifier.DOFade(1,.1f).OnComplete(()=>{
            AudioManager.inst.GetSoundEffect().Play(selectClick);
            StartCoroutine(q());
            IEnumerator q(){
                yield return new WaitForSeconds(.1f);
                selectedSignifier.DOFade(.3f,.1f);
            }
        });
    }

    public virtual void Flip(Vector3 v)
    {
        if( transform.position.x != v.x)
        {
            bool movingRight = transform.position.x <= v.x ;
            if(movingRight)
            { LookRight(); }
            else
            { LookLeft(); }
        }
    }

    public void LookRight()
    {
        transform.localScale = Vector3.one; 
        selectedSignifier.flipX = false;
        facingRight = true;
    }

    public void LookLeft()
    {
        transform.localScale = new Vector3(-1,1,1); 
      selectedSignifier.flipX = true;
        facingRight = false;
    }

}