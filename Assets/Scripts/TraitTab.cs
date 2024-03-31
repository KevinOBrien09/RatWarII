using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;


public class TraitTab : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    
    public Image bg;
    public TextMeshProUGUI traitName;

    public void Init(Trait t,Sprite s)
    {
        bg.sprite = s;
        traitName.text = t.traitName;
        
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
       Debug.Log("Pointer Enter");
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Debug.Log("Pointer Exit");
    }
}