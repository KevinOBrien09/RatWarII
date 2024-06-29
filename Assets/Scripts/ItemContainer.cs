using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.Events;


public class ItemContainer : MonoBehaviour
{
    public List<Item> possibleItems = new List<Item>();
    public int maxItems;
    public int nothingChance;
    public bool itemsGone;
    
    public (string,List<Item>)  RetrieveItems()
    {
        List<Item> it = new List<Item>();

        if(MiscFunctions.randomChance(nothingChance)){
            Debug.Log("Found Nothing");
                itemsGone = true;
            return (retrevialText(it), it);
        }
        else{
            int howMany = Random.Range(1,maxItems);
            for (int i = 0; i < howMany; i++)
            {
                it.Add(possibleItems[Random.Range(0,possibleItems.Count)]);
                itemsGone = true;
            }
        }
        return (retrevialText(it), it);
    }

    public string retrevialText(List<Item> items)
    {
        if(items.Count == 0){
            return "Found Nothing...";
        }
        // Dictionary<string,List<Item>> d = new Dictionary<string,List<Item>>();
        // foreach (var item in items)
        // {
        //     if(d.ContainsKey(item.ID)){
        //         d[item.ID].Add(item);
        //     }
        //     else{
        //         d.Add(item.ID,new List<Item>());
        //         d[item.ID].Add(item);
        //     }
        // }
        // int q = d.Count;
        // string f = "Found ";
        // foreach (var item in d)
        // {
            
        //     string s = item.Value.Count.ToString() +  " " + item.Value[0].itemName[GameManager.inst.language];
        //     if(item.Value.Count > 1 && d.Count > 1){
        //         s += "s";
        //     }
        //     f += s;
        //     q--;
        //     if(q < 1){
        //         f += ", ";
        //     }
        //     else if(q == 1){
        //         f += " and ";
        //     }

            
        // }
        string f = "";
        if(items.Count == 1){
            f = "Found a " + items[0].itemName[GameManager.inst.language];
        }
        else{
            f =  "Found " + items.Count + " " + items[0].itemName[GameManager.inst.language] +"s";
        }
       

        return f;
    }

}