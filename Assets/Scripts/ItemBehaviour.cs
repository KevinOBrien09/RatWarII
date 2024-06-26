using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ItemBehaviour : MonoBehaviour
{
    public TextMeshProUGUI text;
    public Button button;
    public Item item;
    public int amount;
    InventoryViewer viewer;
    public void Init(Item i,InventoryViewer v){
        item = i;
        viewer = v;
        text.text = item.itemName[GameManager.inst.language] + "<size=50%> X</size><size=100%>" + amount.ToString();
    }

    public void Stack(){
        amount++;
        text.text = item.itemName[GameManager.inst.language] + "<size=50%> X</size><size=100%>" + amount.ToString();
    }

    public void Click(){
        viewer.ShowItem(item);
        text.color = Color.red;
    }
}