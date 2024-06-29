    using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using DG.Tweening;
using System;

public enum ItemCatagory{MISC,MAGIC,CONSUMABLE,QUEST}
[System.Serializable]
public class Inventory 
{

    [System.Serializable]
    public class  InventorySave
    {

        public List<string> items = new List<string>();
    }
    
    public  Dictionary<ItemCatagory,GenericDictionary<string,List<Item>>> dict = new Dictionary<ItemCatagory, GenericDictionary<string, List<Item>>>();
    public List<Item> allItems = new List<Item>();
    List<EventHandler> editSubs = new List<EventHandler>();
    public event EventHandler onEdit;
    public event EventHandler fakeEdit
    {
        add
        {
            onEdit += value;
            editSubs.Add(value);
        }

        remove
        {
            onEdit -= value;
            editSubs.Remove(value);
        }
    }

   

    public void RemoveAllEvents()
    {
        foreach(EventHandler eh in editSubs)
        {
            onEdit -= eh;
        }
        editSubs.Clear();
    }

    public void Init()
    {
        foreach (ItemCatagory i in System.Enum.GetValues(typeof(ItemCatagory)))
        {
            dict.Add(i,new GenericDictionary<string, List<Item>>());
        }
    }

    public void AddItem(Item i)
    {
        var key = i.catagory;
        if(!dict.ContainsKey(key))
        {
            Debug.LogAssertion(key + ": WAS NOT IN CATAGORY DICTIONARY!!");
            return;
        }
        if(dict[key].ContainsKey(i.ID))
        {
            dict[key][i.ID].Add(i);
        }
        else
        {
            dict[key].Add(i.ID, new List<Item>());
            dict[key][i.ID].Add(i);
        }
        allItems.Add(i);
        if(onEdit != null){
            onEdit.Invoke(this,null);
        }
        
    }

    public void RemoveItem(Item i)
    {
        var key = i.catagory;
        dict[key][i.ID] .Remove(i);
        allItems.Remove(i);
        if(onEdit != null){
        onEdit.Invoke(this,null);
        }
    }

   

    public InventorySave Save(){
        InventorySave s = new InventorySave();
       // s.gold = gold;
        foreach (var item in allItems)
        {
           s.items.Add(item.ID);
        }
        return s;
    }

}