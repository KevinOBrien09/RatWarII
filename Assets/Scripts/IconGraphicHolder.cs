using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
public class IconGraphicHolder : Singleton<IconGraphicHolder>
{ 
    float currentX = 0;
    public List<CharacterGraphic> graphics = new List<CharacterGraphic>();
    public void Take(CharacterGraphic cg)
    {
        cg.transform.SetParent(transform);
        cg.transform.localPosition = new Vector3(currentX,0,0);
        currentX +=15;
        graphics.Add(cg);
    }

    public void Wipe(){
        foreach (var item in graphics)
        {
            Destroy(item.gameObject);
        }
    }
}