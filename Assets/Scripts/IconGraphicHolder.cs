using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
public class IconGraphicHolder : Singleton<IconGraphicHolder>
{ 
    float currentX = 0;
    public void Take(CharacterGraphic cg)
    {
        cg.transform.SetParent(transform);
        cg.transform.localPosition = new Vector3(currentX,0,0);
        currentX +=15;
    }
}