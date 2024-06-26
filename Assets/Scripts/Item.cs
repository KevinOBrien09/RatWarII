using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Item/Base")]
public class Item : GUIDScriptableObject
{
    public GenericDictionary<Language,string> itemName = new GenericDictionary<Language, string>();
    public GenericDictionary<Language,string> itemDesc = new GenericDictionary<Language, string>();
    public int sellValue;
    public Sprite icon;
    public ItemCatagory catagory;
    public int buyValue()
    {
        int third = MiscFunctions.GetPercentage(33,sellValue);
        return sellValue + third;
    }
}
