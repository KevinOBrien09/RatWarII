
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.EventSystems;
public class CharacterTab : MonoBehaviour
{
    public TextMeshProUGUI charName,charTitle,level,healthTxt;
    public RawImage charIcon,simpleIcon;
    public Image expFill,healthFill;
    public GameObject inPartySignifier;
    public Character character;
    public CharacterCell cell;
    public CharacterDragger dragger;
    public GameObject dismissButton;
    public List<StatSheetOpener> openers = new List<StatSheetOpener>();

    public void Init(Character c)
    {
        character = c;
        charName.text = c.characterName.fullName();
        charTitle.text = c.species.ToString() +"/"+ c.job.ToString();
        level.text = c.exp.level.ToString();
        expFill.DOFillAmount((float) c.exp.currentExp/ (float)c.exp.targetExp,.3f);
        healthTxt.text = "HP:"+ c.battleData.currentHP +"/" + c.stats().hp;
        healthFill.DOFillAmount((float) c.battleData.currentHP/ (float)c.stats().hp,0);
        charIcon.texture = IconGraphicHolder.inst.dict[character.ID];
        simpleIcon.texture = IconGraphicHolder.inst.dict[character.ID];
        foreach (var item in openers)
        {
            item.Init(c);
        }
        
      
    }

    public void ToggleDismissButton(bool state){
        dismissButton.SetActive(state);
    }

    public void Dismiss(){
        if(CharacterStatSheet.inst.open){
            return;
        }

    
            BackbenchHandler.inst.characterToBeDismissed = character;
            BackbenchHandler.inst.DismissCharacterPopup();
      
    }
    public void IsPartyMember(CharacterCell newCell)
    {
        inPartySignifier.SetActive(true);
        newCell.Take(dragger);
        
    
    }


}