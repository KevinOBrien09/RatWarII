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
    public void Refresh()
    {
        fill.DOFillAmount((float)health.currentHealth/(float)health.maxHealth,.35f);
        hpCount.text = "HP:"+ health.currentHealth +  "/" +  health.maxHealth ;
    }
}
