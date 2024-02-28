using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
public class WorldTile : MonoBehaviour,IPointerEnterHandler,IPointerExitHandler,IPointerClickHandler
{
    public SoundData SFX;

    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("Click");
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        AudioManager.inst.GetSoundEffect().Play(SFX);
        Debug.Log("In");
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Debug.Log("Out");
    }
}
