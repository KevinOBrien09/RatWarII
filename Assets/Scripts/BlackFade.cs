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

    public void FadeOut(){
        fade.DOFade(1,0);
        fade.DOFade(0,2);

        
    }
}