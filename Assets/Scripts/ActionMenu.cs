using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using TMPro;

 public enum ActionMenuState{SKILL,MOVE,INTERACT,ROAM}
public class ActionMenu : Singleton<ActionMenu>
{
   
    public enum Formation{DEFAULT,NOMOVE,PEACE}
    public GenericDictionary<Formation,ActionMenuFormation> formationDict = new GenericDictionary<Formation, ActionMenuFormation>();
    public ActionMenuFormation currentFormation;
    public RectTransform rt,border;
    public Vector2 hidden,shown;
    public bool FUCKOFF;
    public Image moveIcon,moveIconFade;
    public SoundData error;
    public TextMeshProUGUI center;
    public ActionMenuState currentState;
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
            Minimap.inst.Hide();
            slot = s;
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
        if(GameManager.inst.checkGameState(GameState.ENEMYTURN) || BattleManager.inst.gameOver||MapGenerator.inst.generating)
        {return;}
        if(!FUCKOFF)
        {
            if(open)
            {
                if(!SkillHandler.inst.open)
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
        } 
    }

    public void OpenSubmenu()
    {
        switch(currentState)
        {
            case ActionMenuState.SKILL:
            if(!SkillHandler.inst.open)
            {
                SkillHandler.inst.Open();
              currentFormation.HideIcons();
            }
            break;

            case ActionMenuState.MOVE:
            // if(!slot.cont.unit.movedThisTurn)
            // {
                FUCKOFF = true;
                rt.DOAnchorPos(hidden,.2f).OnComplete(()=>
                { FUCKOFF = false; open = false;});
                UnitMover.inst.EnterSelectionMode(slot);
                Cursor.lockState = CursorLockMode.Confined;
            // }
            // else
            // {
            //     AudioManager.inst.GetSoundEffect().Play(error);
            //     Debug.Log("Error Noise");
            // }
            break;

            case ActionMenuState.INTERACT:
            FUCKOFF = true;
            rt.DOAnchorPos(hidden,.2f).OnComplete(()=>
            { FUCKOFF = false; open = false;});
            InteractHandler.inst.Open();
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
        border.DORotate(Vector3.zero,0);
        currentFormation.Reset();
    }

    public void ChangeActionMenuState(ActionMenuState newState)
    {currentState = newState;}
    
    public void ReturnFromSkillMenu()
    {BattleTicker.inst.Type(BattleManager.inst. TurnState());
        currentFormation.ShowIcons();
        Reset();
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


    
