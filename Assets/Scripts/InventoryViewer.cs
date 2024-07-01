    using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using DG.Tweening;
using System;
public class InventoryViewer : MonoBehaviour
{
    public ItemBehaviour itemBehaviourPrefab;
    public Transform behaviourHolder;
    public Dictionary<string,ItemBehaviour> behaviourDict = new Dictionary<string, ItemBehaviour>();
    public Image sprite;
    public TextMeshProUGUI itemName,itemDesc,sellValue,catagory;
    public List<InventoryTab> tabs = new List<InventoryTab>();

    public void Load()
    {
        Exit();
        BuildAll();
       InventoryManager.inst.inventory.fakeEdit += XD;
    }

    public void Exit(){
        InventoryManager.inst.inventory.RemoveAllEvents();
    }

    public void XD(object sender,EventArgs args)
    {
        BuildAll();
    }

    
    public void BuildAll(){
        SetEmpty();
        Clear();
        foreach (var item in tabs)
        {
            if(item.isAll)
            {item.Up();}
            else
            {item.Down();}
           
        }
        catagory.text = "All";
        foreach (var i in InventoryManager.inst.inventory.allItems)
        {
            if(behaviourDict.ContainsKey(i.ID))
            {
                behaviourDict[i.ID].Stack();
            }
            else
            {
                ItemBehaviour ib = Instantiate(itemBehaviourPrefab,behaviourHolder);
                ib.Init(i,this);
                ib.Stack();
                behaviourDict.Add(i.ID,ib);
            }
        }

        if(behaviourDict.Count > 0){
            behaviourDict.ToList()[0].Value.Click();
        }
    }

    public void SetEmpty(){
        itemName.text =  string.Empty;
        itemDesc.text = string.Empty;
        sellValue.text =  string.Empty;
        sprite.enabled = false;
    }

    public void BuildCatagory(ItemCatagory catagory){
        SetEmpty();
        Clear();
        foreach (var item in tabs)
        {
            if(item.catagory == catagory && !item.isAll){
                item.Up();
            }
            else{
                item.Down();
            }
           
        }
        this.catagory.text = catagory.ToString();
        foreach (var iter in InventoryManager.inst.inventory.dict[catagory])
        {
            foreach (var i in iter.Value)
            {
                if(behaviourDict.ContainsKey(i.ID))
                {
                    behaviourDict[i.ID].Stack();
                }
                else
                {
                    ItemBehaviour ib = Instantiate(itemBehaviourPrefab,behaviourHolder);
                    ib.Init(i,this);
                    ib.Stack();
                    behaviourDict.Add(i.ID,ib);
                }
            }
        }

        if(behaviourDict.Count > 0)
        {
            behaviourDict.ToList()[0].Value.Click();
        }
    }

    public void ShowItem(Item i){
        itemName.text = i.GetName();
        itemDesc.text = i.GetDesc();
        sellValue.text = "Sells for:" + i.sellValue.ToString();
        sprite.enabled = true;
        sprite.sprite = i.icon;
        foreach(var item in behaviourDict){
            item.Value.text.color = Color.white;
        }
    }

    void Clear()
    {
        foreach (var item in behaviourDict)
        {Destroy(item.Value.gameObject);}
        behaviourDict.Clear();
    }

}