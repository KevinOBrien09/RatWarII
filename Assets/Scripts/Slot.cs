using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System.Linq;
public class Slot : MonoBehaviour,IPointerEnterHandler,IPointerClickHandler,IPointerExitHandler
{
    public Node node;
    public Unit unit;
    public SpriteRenderer border,hoverBorder;
    public TempTerrain tempTerrain;
    public Transform rayShooter;
    public SpecialSlot specialSlot;
    public List<SlotContents> slotContents = new List<SlotContents>();
    public SlotFunctions func = new SlotFunctions();
    void Start()
    {
        func.slot = this;
    }

    public void ChangeColour(Color32 color)
    {border.color = color;}
    
    public void MakeSpecial(SpecialSlot specialSlotPrefab)
    {
        specialSlot = Instantiate(specialSlotPrefab,transform);
        specialSlot.slot = this;
    }

    public void AddContent(SlotContents slotc){
        slotContents.Add(slotc);
    }

    
    public void OnPointerClick(PointerEventData eventData)
    {
        if(eventData.button == PointerEventData.InputButton.Left)
        { SlotSelect(); }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {SlotHover();}

    public void SlotHover()
    {
        if(!node.isBlocked || unit != null)
        {
            if(GameManager.inst.checkGameState(GameState.PLAYERSELECT))
            {
                
                if(UnitMover.inst.validSlots.Contains(this))
                {
                   hoverBorderOn();
                    List<Node> path = MapManager.inst.aStar.FindPath(UnitMover.inst.selectedSlot.transform.position,this.transform.position);
                    UnitMover.inst.NewHover(path);
                }
                else if(SkillAimer.inst.validSlots.Contains(this)){
                      hoverBorderOn();
                }
            }
            // else if(GameManager.inst.checkGameState(GameState.PLAYERHOVER)){
            // hoverBorder.gameObject.SetActive(true);
            // }
           
        }

       
    }

    public void hoverBorderOn(){
 hoverBorder.gameObject.SetActive(true);
            border.gameObject.SetActive(false);
    }

    public void SlotSelect()
    {
        if(SkillAimer.inst.castDecided)
        {return;}

        switch(GameManager.inst.currentGameState)
        {
            case GameState.PLAYERHOVER:
            foreach (var item in MapManager.inst.slots)
            { item. DisableHover();}
            SlotInfoDisplay.inst.Apply(this);
            hoverBorderOn();
            break;
            case GameState.PLAYERSELECT:
            if(SkillAimer.inst.aiming)
            {
                if(this.unit != null)
                { SkillAimer.inst.RecieveSlot(this);
                  DisableHover(); }
                else if(SkillAimer.inst._skill.doesntNeedUnitInSlot)
                { SkillAimer.inst.RecieveSlot(this); 
                  DisableHover();}
            }
            else //moving
            {
                if(GameManager.inst.checkGameState(GameState.PLAYERSELECT)  && !UnitMover.inst.inCoro)
                {
                    if(UnitMover.inst.validSlots.Contains(this))
                    {UnitMover.inst.InitializeMove(this); 
                      DisableHover();}
                }
                else
                {SlotInfoDisplay.inst.Apply(this);
                  DisableHover();}
            }
            break;
         
            case GameState.INTERACT:
            // if(interactable != null)
            // {  DisableHover();
            //     interactable.Go(BattleManager.inst.currentUnit);
            // }
            break;

            case GameState.ENEMYTURN:
            return;
            case GameState.UNITMOVE:
            return;
            case GameState.PLAYERUI:
            return;
            default:
            Debug.LogAssertion("default case!!!");
            break;

        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if(!GameManager.inst.checkGameState(GameState.PLAYERHOVER))
        {
            DisableHover();
        }
   
    }

   public void DisableHover(){
   hoverBorder.gameObject.SetActive(false);
     border.gameObject.SetActive(true);
    }
}
