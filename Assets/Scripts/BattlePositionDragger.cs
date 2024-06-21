
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEngine.UI;
using DG.Tweening;

public class BattlePositionDragger : Draggable
{
    public RectTransform rectTransform;
    public Image main,female;
    public Character character;
  [HideInInspector] public  BattlePositionEditor editor;
  [HideInInspector] public BattlePositionSlot slot;
    public void Init(Character c,BattlePositionEditor e,BattlePositionSlot s){
        character = c;
       
        editor = e;
        main.rectTransform.sizeDelta = new Vector2(65,65);
        female.rectTransform.sizeDelta = new Vector2(65,65);
        rectTransform.sizeDelta = new Vector2(25,25);
        s.Take(this);
        SetCharSprites(c);
    }

    public void SetY(){
        rectTransform.anchoredPosition = Vector2.zero;
        main.rectTransform.anchoredPosition = new Vector2(0,5);
        female.rectTransform.anchoredPosition = new Vector2(0,5);
    }
    


     public void SetCharSprites(Character c){
        if(c.gender == Gender.FEMALE)
        {
            if(c.job == Job.KNIGHT)
            {
                if(c.species != Species.FROG)
                {
                    if(c.spriteVarient == 3){
                       female.gameObject.SetActive(false);   //Female BucketHelm Knights do not have lashes"
             
                        goto spriteSetUp;
                    }
                }

            }
           female.gameObject.SetActive(true);
            female.sprite = CharacterBuilder.inst.female[c.species];
        }
        else
        {    female.gameObject.SetActive(false);}

        spriteSetUp:
    
      main.sprite = CharacterBuilder.inst.classVarients[c.species][c.job][c.spriteVarient];
      
    }

    public override void DragEnd( List<RaycastResult> results)
    {
        BattlePositionSlot cell = null;
	    foreach (var result in results)
		{
            cell = result.gameObject.GetComponent<BattlePositionSlot>();
            if (cell != null)
			{
             
                if(cell.dragger == null)
                {
                    slot.Remove();
                    cell.Take(this);
                }
                else{ 
                    Debug.Log("QWERTY");
                    BattlePositionSlot slotA = slot;
                    BattlePositionSlot slotB = cell;
                    BattlePositionDragger other = cell.dragger;
                    slotA.Remove();
                    slotB.Remove();
                    
                    slotB.Take(this);
                    slotA.Take(other);
                }
               
                return;
              
			}
        }

        if(cell == null)
        { NoCell(); }
    }

    public override void NoCell(){
        transform.SetParent(startParent);
        SetY();
    }
    

}