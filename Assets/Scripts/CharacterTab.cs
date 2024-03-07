
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
    public Character c;
    public CharacterDragger dragger;
    public GameObject inPartySignifier;
    public EquipmentSlot equipmentSlot;
    public Character character;
    public CharacterGraphic iconGraphic;
    public RenderTexture rt;
    void Start(){
        // character =CharacterBuilder.inst.GenerateCharacter();
        // Init(character);
    }
    public void Init(Character c){
        character = c;
        charName.text = c.characterName.fullName();
        charTitle.text = c.species.ToString() +"/"+ c.job.ToString();
        dragger.Init(this);
        CharacterGraphic cg = CharacterBuilder.inst.GenerateGraphic(c,true);
        
        iconGraphic = cg.iconClone;
        rt = iconGraphic.cam.activeTexture;
        charIcon.texture = rt;
        simpleIcon.texture = rt;
        Destroy(cg.gameObject);
    }
    public void IsPartyMember(int i){
        inPartySignifier.SetActive(true);
        BackbenchHandler.inst.partyCells[i].Drop(dragger);
    }


}