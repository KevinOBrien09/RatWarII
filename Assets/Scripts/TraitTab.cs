using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;


public class TraitTab : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    
    public Image bg;
    public TextMeshProUGUI traitName;
    public GameObject toolTip;
    public TextMeshProUGUI toolTipDesc;

    public void Init(Trait t,Sprite s)
    {
        bg.sprite = s;
        traitName.text = t.traitName;
        toolTipDesc.text = t.traitDesc;
        toolTip.SetActive(false);
    }
    
    public void OnPointerEnter(PointerEventData eventData)
    { toolTip.SetActive(true); }

    public void OnPointerExit(PointerEventData eventData)
    { toolTip.SetActive(false); }
}