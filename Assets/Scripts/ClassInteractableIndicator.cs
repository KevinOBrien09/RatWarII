using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class ClassInteractableIndicator : MonoBehaviour
{   
    public Job job;
    public GenericDictionary<Job,Sprite> dict = new GenericDictionary<Job, Sprite>();
    public SpriteRenderer icon,bg,frame;
    public SoundData sfx;
    bool inTween;
    public void Start(){
        icon.sprite = dict[job];
        icon.DOFade(0,0);
        bg.DOFade(0,0);
        frame.DOFade(0,0);
    }

    public void Show()
    {
        if(!inTween){
            inTween = true;
            AudioManager.inst.GetSoundEffect().Play(sfx);
            icon.DOFade(1,.2f);
            frame.DOFade(1,.2f);
            bg.DOFade(.5f,.2f).OnComplete(()=>{
                StartCoroutine(q());
                IEnumerator q()
                {    
                    yield return new WaitForSeconds(1f);
                    icon.DOFade(0,.2f);
                    frame.DOFade(0,.2f);
                    bg.DOFade(0,.2f).OnComplete(()=>
                    {
                    inTween = false;
                    });
                }

            });

        }
       
    }
}