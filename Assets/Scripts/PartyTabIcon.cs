using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections.Generic;
public class PartyTabIcon : StatSheetOpener 
{
    public RawImage rawImage;
 

    public void Wipe(){
        character = null;
       rawImage.enabled = false;
    }

    public override void Init(Character c){
        rawImage.texture =  IconGraphicHolder.inst.dict[c.ID];
        character = c;
        rawImage.enabled = true;
    }

}