using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening;
using TMPro;
using System.Linq;
public class ItemBattleHander : Singleton<ItemBattleHander>
{
    public GenericDictionary<string,SkillBehaviour> currentItems = new GenericDictionary<string, SkillBehaviour>();
    public bool open;
    public ScrollRectAutoScroll scrollRectAutoScroll;
    public SkillBehaviour prefab;
    public Transform holder;
    public Item itemToRemove;

    void Start(){
        Close();
    }
    public void BuildInventory(){
        WipeInventory();
        foreach (var item in InventoryManager.inst.inventory.allItems)
        {
            if(item.canBeUsedInBattle)
            {
                if(currentItems.ContainsKey(item.ID)){
                    currentItems[item.ID].ItemStack(item);
                }
                else
                {
                    SkillBehaviour sb =  Instantiate(prefab,holder);
                    sb.Init(item);
                    currentItems.Add(item.ID,sb);
                    currentItems[item.ID].ItemStack(item);
                }
            }
        }
    }

    public void WipeInventory(){
        foreach (var item in currentItems)
        {
            Destroy(item.Value.gameObject);
        }
        currentItems.Clear();
    }


    public void Open()
    {
        gameObject.SetActive(true);
        if(currentItems.Count > 0)
        {
            StartCoroutine(q(currentItems.ToList()[0].Value.gameObject));
            IEnumerator q(GameObject g)
            {
                EventSystem.current.SetSelectedGameObject(null);
                yield return new WaitForEndOfFrame();
                EventSystem.current.SetSelectedGameObject(g);
                scrollRectAutoScroll.ScrollToSelected();
            }
        }
        itemToRemove = null;
        open = true;
    }

    void Update()
    {
        if(SkillHandler.inst.open)
        {
            return;
        }
        if(open && !ActionMenu.inst.FUCKOFF)
        {
            if(InputManager.inst.player.GetButtonDown("Cancel"))
            { 
                if(SkillAimer.inst.castDecided){
                    return;
                }
                if(!SkillAimer.inst.aiming )
                { Close(); }
                else
                { 
                    SkillAimer.inst.Leave(); 
                    itemToRemove = null;
                }

            }
            else if(InputManager.inst.player.GetButtonDown("Confirm"))
            {
                if(SkillHandler.inst.hoveredSkill != null)
                {
                    if(!SkillAimer.inst.aiming&& !ActionMenu.inst.FUCKOFF)
                    {
                        Item item = SkillHandler.inst.hoveredSkill as Item;
                        if(item != null)
                        {
                            BattleManager.inst.StartCoroutine(SkillHandler.inst.SetObject(null));
                            itemToRemove = item;
                            SkillAimer.inst.Go(item.cast);
                        
                            ActionMenu.inst.Hide();
                        }
                    }
                }
                else{
                    Debug.LogWarning("hover skill is null");
                }
            }
               
        }
    }

    public void Close(){
        SkillHandler.inst.hoveredSkill = null;
        SkillHandler.inst.hoveredBehaviour = null;
       
        gameObject.SetActive(false);
        ActionMenu.inst.ReturnFromItemMenu();
        itemToRemove = null;
        open = false;
        //ActionMenu.inst.ReturnFromItemMenu();
       
    }



   
}