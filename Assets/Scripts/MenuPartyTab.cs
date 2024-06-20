using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class MenuPartyTab : MonoBehaviour
{
    public TextMeshProUGUI charName,title,level,hp,res;
    public RawImage rawImage;
    public Image hpFill,manaFill,expFill;
    Unit u;
    public void Init(Unit _u){
        u = _u;
        Character c = u.character;
        rawImage.texture = IconGraphicHolder.inst.dict[c.ID];
        charName.text = c.characterName.fullName();
        title.text = c.job.ToString()  +" " +c.species.ToString();
        hp.text = u.health.currentHealth +"/"+ u.health.maxHealth;
        res.text = u.skillResource.current + "/"+ u.skillResource.max;
        level.text = c.exp.level.ToString();
        expFill.DOFillAmount((float) c.exp.currentExp/(float)c.exp.targetExp,.1f);
        hpFill.DOFillAmount((float)u.health.currentHealth/(float)u.health.maxHealth,.1f);
        manaFill.DOFillAmount((float)u.skillResource.current/u.skillResource.max,.1f);
    }

    public void Click(){
        Menu.inst.ShowStatScreen(u);
    }

}