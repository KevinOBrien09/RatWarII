
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using DG.Tweening;

public class SoloCharacterMapTab : MonoBehaviour
{
    public RawImage picture;
    public TextMeshProUGUI charName;
    public void Init(CharacterHolder ch){
        picture.texture =  IconGraphicHolder.inst.dict[ch. character.ID];
        charName.text = ch.character.characterName.fullName();
    }

}