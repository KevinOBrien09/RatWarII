using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using TMPro;

 public enum ActionMenuState{SKILL,MOVE,ITEM,ROAM,SKIP}
public class ActionMenu : Singleton<ActionMenu>
{
   
    public enum Formation{DEFAULT,NOMOVE,PEACE}
    public GenericDictionary<Formation,ActionMenuFormation> formationDict = new GenericDictionary<Formation, ActionMenuFormation>();
    public ActionMenuFormation currentFormation;
    public RectTransform rt;
    public Vector2 hidden,shown;
    public bool FUCKOFF;
    public Image moveIcon,moveIconFade,skillIcon,skillIconFade,itemIcon,itemIconFade;
    public SoundData error;
    public TextMeshProUGUI center;
    public ActionMenuState currentState;
    public Skill wait;
    Slot slot;
    public bool open;
    void Start()
    {  
        Cursor.lockState = CursorLockMode.Locked;
       
        shown = rt.anchoredPosition;
        rt.DOAnchorPos(hidden,0);
        Reset();
    }

    public void Show(Slot s)
    {
        if(GameManager.inst.checkGameState(GameState.UNITMOVE))
        {return;}


        if(!FUCKOFF)
        {
          //  Minimap.inst.Hide();
            slot = s;

            
            if(slot.cont.unit.battleTokens.canMove())
            {
                ResetMoveOption();
            }   
            else{
                RemoveMoveOption();
            }

            if(slot.cont.unit.battleTokens.canAct())
            {
                ResetSkillOption();
            }   
            else{
                RemoveSkillOption();
            }

            if(slot.cont.unit.battleTokens.canAct() && InventoryManager.inst.BattleItemCount() > 0)
            {ResetItemOption();}
            else
            {RemoveItemOption();}


            CamFollow.inst.Focus(s.cont.unit.transform,()=>
            { 
                if(ActionMenu.inst.currentState != ActionMenuState.SKILL)
                {BattleTicker.inst.Type(BattleManager.inst. TurnState());}
                CamFollow.inst.ChangeCameraState(CameraState.LOCK);
                GameManager.inst.ChangeGameState(GameState.PLAYERUI);
            });
            gameObject.SetActive(true);
            SlotInfoDisplay.inst.Apply(s);
            MoveOnScreen();
        }
    }

  

    public void RemoveMoveOption(){
        moveIcon.color = new Color(0,0,0,.75f);
        moveIconFade.enabled = true;
    }

    public void ResetMoveOption(){
        moveIcon.color =  new Color(1,1,1,.75f);
        moveIconFade.enabled = false;
    }

    public void RemoveSkillOption(){
        skillIcon.color = new Color(0,0,0,.75f);
        skillIconFade.enabled = true;
    }

    public void ResetSkillOption(){
        skillIcon.color =  new Color(1,1,1,.75f);
        skillIconFade.enabled = false;
    }

     public void RemoveItemOption(){
        itemIcon.color = new Color(0,0,0,.75f);
        itemIconFade.enabled = true;
    }

    public void ResetItemOption(){
        itemIcon.color =  new Color(1,1,1,.75f);
        itemIconFade.enabled = false;
    }

    IEnumerator q()
    {
        yield return new WaitForSeconds(.35f);
        FUCKOFF = false;
        open = true;
    }

    public void MoveOnScreen(){
        Cursor.lockState = CursorLockMode.Locked;
        FUCKOFF = true;
        SlotInfoDisplay.inst.Apply(slot);
        rt.DOAnchorPos(hidden,0);
        rt.DOAnchorPos(shown,.2f);
        BattleManager.inst.StartCoroutine(q());
    }
    
    void Update()
    {
        if(GameManager.inst.checkGameState(GameState.ENEMYTURN) ||MapGenerator.inst.generating)
        {return;}
        if(!FUCKOFF)
        {
            if(open)
            {
                if(!SkillHandler.inst.open && !ItemBattleHander.inst.open)
                {
                    if(InputManager.inst.player.GetButtonDown("Right"))
                    {currentFormation.MoveRight();}

                    else if(InputManager.inst.player.GetButtonDown("Left"))
                    {currentFormation.MoveLeft();}
                }
                
                if(InputManager.inst.player.GetButtonDown("Confirm")) 
                { OpenSubmenu(); }

            }
            
            if(currentState == ActionMenuState.ROAM && !open)
            {
                if(InputManager.inst.player.GetButtonDown("Cancel"))
                {
                    foreach (var item in MapManager.inst.allSlots)
                    { item. DisableHover();}
                    GameManager.inst.ChangeGameState(GameState.PLAYERUI);
                    Show(slot);
                }
            }

            if(currentState == ActionMenuState.SKIP && !open && !SkillAimer.inst.castDecided)
            {  
                if(InputManager.inst.player.GetButtonDown("Cancel"))
                {
                    foreach (var item in MapManager.inst.allSlots)
                    { item. DisableHover();}
                    GameManager.inst.ChangeGameState(GameState.PLAYERUI);
                    SkillAimer.inst.Leave();
                    SkillHandler.inst.hoveredSkill = null;
                    Show(slot);
                }
            }
        } 
    }

    public void OpenSubmenu()
    {
        switch(currentState)
        {
            case ActionMenuState.SKILL:
            if(slot.cont.unit.battleTokens.canAct())
            {
                if(!SkillHandler.inst.open&& !ItemBattleHander.inst.open)
                {
                    SkillHandler.inst.Open();
                    currentFormation.HideIcons();
                }
            }
            else
            {
                AudioManager.inst.GetSoundEffect().Play(error);
                Debug.Log("Error Noise");
            }
            break;

            case ActionMenuState.MOVE:
            if(slot.cont.unit.battleTokens.canMove())
            {
                FUCKOFF = true;
                rt.DOAnchorPos(hidden,.2f).OnComplete(()=>
                { FUCKOFF = false; open = false;});
                UnitMover.inst.EnterSelectionMode(slot);
                Cursor.lockState = CursorLockMode.Confined;
            }
            else
            {
                AudioManager.inst.GetSoundEffect().Play(error);
                Debug.Log("Error Noise");
            }
            break;

            case ActionMenuState.ITEM:
            if(slot.cont.unit.battleTokens.canAct()&& InventoryManager.inst.BattleItemCount() > 0)
            {
                if(!SkillHandler.inst.open&& !ItemBattleHander.inst.open)
                {
                    ItemBattleHander.inst.Open();
                    currentFormation.HideIcons();
                }
            }
            else
            {
                AudioManager.inst.GetSoundEffect().Play(error);
                Debug.Log("Error Noise");
            }
            break;

            case ActionMenuState.SKIP:
            
            Hide();
         
            SkillAimer.inst.Go(wait);
            Debug.Log("SKIP");
            break;

            case ActionMenuState.ROAM:
            BattleTicker.inst.Type("Roaming...");
            BattleManager.inst.currentUnit.slot.hoverBorderOn();
            CamFollow.inst.ChangeCameraState(CameraState.FREE);
            GameManager.inst.ChangeGameState(GameState.PLAYERHOVER);
            Hide();
            Cursor.lockState = CursorLockMode.Confined;
        
            break;
        }
    }

    public void Reset()
    {
        BattleTicker.inst.Type(BattleManager.inst. TurnState());
        currentState = ActionMenuState.SKILL;
    
        currentFormation.Reset();
    }

    public void ChangeActionMenuState(ActionMenuState newState)
    {currentState = newState;}
    
    public void ReturnFromSkillMenu()
    {
        BattleTicker.inst.Type(BattleManager.inst. TurnState());
        currentFormation.ShowIcons();
        Reset();
    }

    public void ReturnFromItemMenu(){
        currentState = ActionMenuState.ITEM;
        currentFormation.ShowIcons();
        BattleTicker.inst.Type(BattleManager.inst. TurnState());
       
        //border.DORotate(new Vector3(0,0,120),0);
        currentFormation.ItemReset();
    }
    
    public void Hide()
    { 
        if(!FUCKOFF){

            FUCKOFF = true;
            if(!GameManager.inst.checkGameState(GameState.PLAYERHOVER)){
    SlotInfoDisplay.inst.Disable();
            }
        
            rt.DOAnchorPos(hidden,.2f).OnComplete(()=>
            { 
               
                open = false;
         
                FUCKOFF = false;
                
            });
           
        }
        
    }
}


    
