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

    public void FadeOut(float t = 2,UnityAction a = null){
        fade.DOFade(1,0);
        fade.DOFade(0,t);
        if(a != null){
            a.Invoke();
        }

        
    }

    public void FadeInEvent(UnityAction a){
        fade.DOFade(1,.25f).OnComplete(()=>{
            a.Invoke();
        });
    }

    public void toggleRaycast(bool b){
        fade.raycastTarget = b;
    }
}