
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.EventSystems;
public class CharacterTab : MonoBehaviour
{
    public TextMeshProUGUI charName,charTitle;
    public RawImage charIcon,simpleIcon;
    public GameObject inPartySignifier;
    public Character character;
    public CharacterCell cell;
    public CharacterDragger dragger;


    public void Init(Character c)
    {
        character = c;
        charName.text = c.characterName.fullName();
        charTitle.text = c.species.ToString() +"/"+ c.job.ToString();

           
        charIcon.texture = IconGraphicHolder.inst.dict[character.ID];
        simpleIcon.texture = IconGraphicHolder.inst.dict[character.ID];
        
      
    }
    public void IsPartyMember(CharacterCell newCell)
    {
        inPartySignifier.SetActive(true);
        newCell.Take(dragger);
        
    
    }


}