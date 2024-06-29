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
    public Image fill,shieldFill;
    Tween tween;
    public bool textJustShowsCurrent;
void OnEnable(){
    if(health != null){
 shieldFill.fillAmount = (float)health.shield()/(float)health.maxHealth;
    }
    else{
      //  Debug.Log("XD HEALTH IS HECKIN NULL" + gameObject.name);
    }
  
   
}
  
    public void Refresh()
    {  
        shieldFill.DOFillAmount((float)health.shield()/(float)health.maxHealth,.35f);
        fill.DOFillAmount((float)health.currentHealth/(float)health.maxHealth,.35f);
        if(health.currentHealth < 0){
            health.currentHealth = 0;
        }

        if(hpCount != null)
        {
            if(health.shield() > 0)
            {   
                  int i = health.currentHealth + health.shield();
                if(textJustShowsCurrent){
                    hpCount.text = i.ToString();
                }
                else{
                  
                    hpCount.text = "HP:<b>"+ i +  "</b>/" +  health.maxHealth ;
                }
            }
            else
            {
                if(textJustShowsCurrent)
                { hpCount.text =  health.currentHealth.ToString()+":";;
                }
                else{
                    hpCount.text = "HP:"+ health.currentHealth +  "/" +  health.maxHealth ;
                }
               
            }
        }
    }

   
}
