using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Item/Base")]
public class Item : Castable
{
    
    public int sellValue;
    public Sprite icon;
    public ItemCatagory catagory;
    public bool canBeUsedInBattle;
    public Skill cast;
    public int buyValue()
    {
        int third = MiscFunctions.GetPercentage(33,sellValue);
        return sellValue + third;
    }
}
