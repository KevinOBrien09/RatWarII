using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System.Linq;
public class Slot : MonoBehaviour,IPointerEnterHandler,IPointerClickHandler,IPointerExitHandler
{
    public SlotContainer cont = new SlotContainer();
    public SlotFunctions func = new SlotFunctions();
    public Node node;
    public SpriteRenderer border,hoverBorder,areaIndicator;
    public Transform rayShooter;
    public Color32 normalColour,moveColour,interactColour,skillColour;

    void Awake()
    {
        cont.slot = this;
        func.slot = this;
    }

    public void ChangeColour(Color32 color)
    {border.color = color;}

   
    
    public SpecialSlot MakeSpecial(SpecialSlot specialSlotPrefab)
    {
        cont.specialSlot = Instantiate(specialSlotPrefab,transform);
        if(cont.specialSlot.interactable != null){
            cont.specialSlot.interactable.slot = this;
        }
        cont.specialSlot.slot = this;
        return cont.specialSlot;
    }

    public void IsWall(){
        cont.wall = true;
        cont.invisible = true;
        border.gameObject.SetActive(false);
        hoverBorder.gameObject.SetActive(false);
    }
    
    public void SlotHover()
    {
        if(cont.invisible)
        { return; }
       
        if(GameManager.inst.checkGameState(GameState.PLAYERSELECT))
        {
            if(UnitMover.inst.validSlots.Contains(this))
            {
                if(cont.walkable())
                {
                    hoverBorderOn();
                    List<Node> path = MapManager.inst.currentRoom.grid. aStar.FindPath(UnitMover.inst.selectedSlot.node,node);
                    UnitMover.inst.NewHover(path);
                }
              
            }
           
            else if(SkillAimer.inst.validSlots.Contains(this))
            { hoverBorderOn(); }
        }
        else if(GameManager.inst.checkGameState(GameState.INTERACT))
        {
            if(InteractHandler.inst.slots.Contains(this))
            { hoverBorderOn();
            SlotInfoDisplay.inst.Apply(this); }
        }
    }
    
    public void SlotSelect()
    {
        if(cont.invisible)
        { return; }
        if(SkillAimer.inst.castDecided)
        {return;}

        switch(GameManager.inst.currentGameState)
        {
            case GameState.PLAYERHOVER:
            foreach (var item in MapManager.inst.currentRoom.slots)
            { item. DisableHover();}
            SlotInfoDisplay.inst.Apply(this);
            hoverBorderOn();
            break;
            case GameState.PLAYERSELECT:
            if(SkillAimer.inst.aiming)
            {
                if(this.cont.unit != null)
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
            if(InteractHandler.inst.slots.Contains(this)){
                InteractHandler.inst.TakeInteraction(this);
            }
     
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
    
    public void hoverBorderOn()
    {
        if(cont.invisible)
        { return; }
        if(cont.unit == null)
        { hoverBorder.color = Color.white; }
       
        else if(cont.unit != null )
        {
            if(cont.unit.side == Side.ENEMY)
            {hoverBorder.color = Color.red;}
            else
            {hoverBorder.color = Color.blue;}
        }
        else if(cont.specialSlot != null && cont.unit == null)
        {
            if(cont.specialSlot.interactable != null){
                hoverBorder.color = Color.magenta;
            }
        }

       
        hoverBorder.gameObject.SetActive(true);
        border.gameObject.SetActive(false);
    }

    public void DisableHover()
    {
        if(cont.invisible)
        { return; }
        hoverBorder.gameObject.SetActive(false);
        border.gameObject.SetActive(true);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if(eventData.button == PointerEventData.InputButton.Left)
        { SlotSelect();}
    }

    public void OnPointerEnter(PointerEventData eventData)
    {SlotHover();}

    public void OnPointerExit(PointerEventData eventData)
    {
        if(!GameManager.inst.checkGameState(GameState.PLAYERHOVER))
        { DisableHover(); }
    }

   
}
