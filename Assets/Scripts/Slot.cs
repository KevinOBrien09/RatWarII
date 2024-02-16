using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System.Linq;
public class Slot : MonoBehaviour,IPointerEnterHandler,IPointerClickHandler
{
    public SpriteRenderer border;
    public Unit unit;
    public Node node;
    public TempTerrain tempTerrain;
    public GameObject indicator;
    public Transform rayShooter;
    public SpecialSlot specialSlot;
    public Interactable interactable;
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

    public void MakeInteractable(Interactable _interactable)
    {
        interactable = Instantiate(_interactable,transform);
        interactable.Init(this);
    }


    public void OnPointerClick(PointerEventData eventData)
    {
        if(GameManager.inst.checkGameState(GameState.UNITMOVE)|GameManager.inst.checkGameState(GameState.ENEMYTURN))
        {return;}
        if(SkillAimer.inst.castDecided)
        {return;}

        if(eventData.button == PointerEventData.InputButton.Left)
        {
            if(GameManager.inst.checkGameState(GameState.INTERACT))
            {
                if(interactable != null)
                {
                    interactable.Go(BattleManager.inst.currentUnit);
                    return;
                }
                else
                {return;}
            }
          
            if(SkillAimer.inst.aiming)
            {
                if(SkillAimer.inst._skill.doesntNeedUnitInSlot)
                {SkillAimer.inst.RecieveSlot(this);}
                else
                {
                    if(this.unit != null)
                    {SkillAimer.inst.RecieveSlot(this); }
                }
            }
            else
            {
                if(unit != null)
                {
                    if(!GameManager.inst.checkGameState(GameState.PLAYERUI) )
                    {
                        if(!GameManager.inst.checkGameState(GameState.PLAYERSELECT))   
                        {
                            if(unit == BattleManager.inst.currentUnit)
                            {
                                if(ActionMenu.inst.currentState == ActionMenu.ActionMenuState.ROAM){
                                    return;
                                }
                                if(!ActionMenu.inst.open)
                                {ActionMenu.inst.Show(this); }
                            }
                            else
                            {SlotInfoDisplay.inst.Apply(this);}
                        }
                    }
                }
                else
                {
                    if(GameManager.inst.checkGameState(GameState.PLAYERSELECT)  && !UnitMover.inst.inCoro)
                    {
                        if(UnitMover.inst.validSlots.Contains(this))
                        {UnitMover.inst.InitializeMove(this); }
                    }
                    else
                    {SlotInfoDisplay.inst.Apply(this);}
                }
            }
           
        }
      
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if(!node.isBlocked || unit != null)
        {
            if(GameManager.inst.checkGameState(GameState.PLAYERSELECT))
            {
                if(UnitMover.inst.validSlots.Contains(this))
                {
                    SlotSelector.inst.Attach(this);
                    List<Node> path = MapManager.inst.aStar.FindPath(UnitMover.inst.selectedSlot.transform.position,
                    SlotSelector.inst.transform.position);
                    UnitMover.inst.NewHover(path);
                }
            }
            SlotSelector.inst.Attach(this); 
        }

        if(interactable != null)
        {SlotSelector.inst.Attach(this);}


    }


}
