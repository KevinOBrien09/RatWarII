using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;
public class HealthBar : MonoBehaviour
{
    public Health health;
    public TextMeshProUGUI hpCount;
    public Image fill;
    Tween tween;
    public void Refresh()
    {
        tween =  fill.DOFillAmount((float)health.currentHealth/(float)health.maxHealth,.35f);
        if(health.currentHealth < 0){
            health.currentHealth = 0;
        }
        hpCount.text = "HP:"+ health.currentHealth +  "/" +  health.maxHealth ;
    }

    public void KillTween(){
       tween.Kill(true);
    }
}
