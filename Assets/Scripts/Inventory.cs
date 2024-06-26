    using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using DG.Tweening;

public enum ItemCatagory{MISC,MAGIC,CONSUMABLE}
public class Inventory : Singleton<Inventory>
{
    public int gold;
    public List<Item> alltems = new List<Item>();
    public  GenericDictionary<ItemCatagory,GenericDictionary<string,List<Item>>> dict = new GenericDictionary<ItemCatagory, GenericDictionary<string, List<Item>>>();
    public List<Item> itemOfEveryType = new List<Item>();
    public List<Item> allItems = new List<Item>();
    public UnityEvent onEdit;
    IEnumerator Start()
    {
        foreach (ItemCatagory i in System.Enum.GetValues(typeof(ItemCatagory)))
        {
        
            dict.Add(i,new GenericDictionary<string, List<Item>>());
        }
        gold = 900;
        Refresh();
        foreach (var item in allItems)
        {
            AddItem(item);
        }
   
        yield return new WaitForSeconds(10);
    }

    public void AddItem(Item i)
    {
        var key = i.catagory;
        if(!dict.ContainsKey(key))
        {
            Debug.LogAssertion(key + ": WAS NOT IN DICTIONARY, PUT OBJECT OF THAT TYPE INTO THE LIST!!!");
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
        alltems.Add(i);
        onEdit.Invoke();
    }

    public void RemoveItem(Item i)
    {
        var key = i.catagory;
        dict[key][i.ID] .Remove(i);
        alltems.Remove(i);
        onEdit.Invoke();
    }

    public bool canAfford(int i)
    {return gold >= i;}

    public void AddGold(int i)
    {
        gold += i;
       Refresh();
        
    }
    
    void Refresh()
    {
        if(GoldText.inst != null)
        { GoldText.inst.Refresh(); }
    }

    public void RemoveGold(int i)
    {
        gold -= i;
        Refresh();
    }

}