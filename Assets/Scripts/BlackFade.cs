using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using UnityEngine.Events;
using DG.Tweening;

public class BlackFade : Singleton<BlackFade>
{
    public Image fade;

    public void FadeOut(float t = 2){
        fade.DOFade(1,0);
        fade.DOFade(0,t);

        
    }

    public void FadeInEvent(UnityAction a){
        fade.DOFade(1,.25f).OnComplete(()=>{
            a.Invoke();
        });
    }
}