    using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using DG.Tweening;


public class InventoryManager : Singleton<InventoryManager>
{
    public int gold;
    public Inventory inventory;
    public List<Item> everyItem = new List<Item>();
    public GenericDictionary<string,Item> itemDict = new GenericDictionary<string, Item>();

    protected override void Awake(){
        base.Awake();
        foreach (var item in everyItem)
        {
            itemDict.Add(item.ID,item);
        }
    }
    public void SetCurrentInventory(Inventory newInventory)
    {
        inventory = newInventory;
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

    public int BattleItemCount(){
        int i = 0;
        foreach (var item in inventory.allItems)
        {
            if(item.canBeUsedInBattle){
                i++;
            }
        }
        return i;
    }

    public Inventory Load(Inventory.InventorySave inventorySave)
    {
        Inventory i = new Inventory();
        i.Init();
        foreach (var id in inventorySave.items)
        {i.AddItem(itemDict[id]);}
        return i;
    }
}