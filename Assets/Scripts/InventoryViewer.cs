    using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using DG.Tweening;

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
        //Inventory.inst.onEdit.AddListener((()=>{BuildAll();}));
    }

    public void Exit(){
        Inventory.inst.onEdit.RemoveAllListeners(); 
    }

    public void BuildAll(){

        Clear();
        foreach (var item in tabs)
        {
            if(item.isAll)
            {item.Up();}
            else
            {item.Down();}
           
        }
        catagory.text = "All";
        foreach (var i in Inventory.inst.alltems)
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

    public void BuildCatagory(ItemCatagory catagory){
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
        foreach (var iter in Inventory.inst.dict[catagory])
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
        itemName.text = i.itemName[GameManager.inst.language];
        itemDesc.text = i.itemDesc[GameManager.inst.language];
        sellValue.text = "Sells for:" + i.sellValue.ToString();
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