using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;
public class ResourceBar : MonoBehaviour
{
    public Unit unit;
    public GenericDictionary<SkillResource.Catagory,Sprite> dict = new GenericDictionary<SkillResource.Catagory, Sprite>();
    public GenericDictionary<SkillResource.Catagory,Color32> colourDict = new GenericDictionary<SkillResource.Catagory, Color32>();
    public Image fill;
    public TextMeshProUGUI resCount;
    public bool textJustShowsCurrent;
    public void Init(Unit u){
        unit = u;
        unit.skillResource.onChange.AddListener(()=>{
            Refresh();
        });
        fill.sprite = dict[u.skillResource.catagory];
        resCount.color = colourDict[u.skillResource.catagory];
        Refresh();
    }

    public void Refresh()
    {  
        fill.DOFillAmount((float)unit.skillResource.current/(float)unit.skillResource.max,.35f);
        if(unit.skillResource.current < 0)
        {unit.skillResource.current = 0;}
        
        if(resCount != null)
        {
            if(textJustShowsCurrent){
                resCount.text = unit.skillResource.current.ToString() +":";
            }
            else{
                resCount.text = "HP:"+ unit.skillResource.current +  "/" +   unit.skillResource.max;
            }
           
        }
    }

}