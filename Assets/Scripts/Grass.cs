using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.EventSystems;
using System.Linq;
using UnityEngine.Rendering;
public class Grass : SpecialSlot
{
    public List<SortingGroup> frontfacingGrass = new List<SortingGroup>();
    public override void Init(){
        base.Init();
        foreach (var item in frontfacingGrass)
        {
            item.sortingOrder =   -slot.node.iGridY;
        }
        float y = Random.Range(.7f,1f);
        transform.localScale = new Vector3(transform.localScale.x,y,transform.localScale.z);
    }
   
}