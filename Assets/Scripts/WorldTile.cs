using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
public class WorldTile : MonoBehaviour,IPointerEnterHandler,IPointerExitHandler,IPointerClickHandler
{
    public SoundData SFX;
    
    public virtual void Click()
    {}

    public virtual void Enter()
    {AudioManager.inst.GetSoundEffect().Play(SFX);}

    public virtual void Exit()
    {}

    public void OnPointerClick(PointerEventData eventData)
    {
        if(eventData.button == PointerEventData.InputButton.Left)
        {Click();}
    }
    
    public void OnPointerEnter(PointerEventData eventData)
    {Enter();}

    public void OnPointerExit(PointerEventData eventData)
    {Exit();}
}
