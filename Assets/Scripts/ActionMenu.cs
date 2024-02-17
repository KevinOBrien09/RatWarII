using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;

public class ActionMenu : Singleton<ActionMenu>
{
    public enum ActionMenuState{SKILL,MOVE,INTERACT,ROAM}
    public RectTransform rt,border;
    public Vector2 hidden,shown;
    public bool FUCKOFF;
    public TextMeshProUGUI center;
    public ActionMenuState currentState;
    public GenericDictionary<ActionMenuState,RectTransform> icons = new GenericDictionary<ActionMenuState, RectTransform>();
   
    Slot slot;
    public bool open;
    void Start()
    {  
      
        Cursor.lockState = CursorLockMode.Locked;
        shown = rt.anchoredPosition;
        //gameObject.SetActive(false);
        rt.DOAnchorPos(hidden,0);
        Reset();
    }

    public void Show(Slot s)
    {
        if(GameManager.inst.checkGameState(GameState.UNITMOVE)){
   
            return;
        }

        if(!FUCKOFF){
            
    
            slot = s;
           
            // CamFollow.inst.ChangeCameraState(CameraState.LOCK);
            // GameManager.inst.ChangeGameState(GameState.PLAYERUI);
            CamFollow.inst.Focus(s.unit.transform,()=>
            { 
                if(ActionMenu.inst.currentState !=ActionMenu.ActionMenuState.SKILL){
                BattleTicker.inst.Type(BattleManager.inst. TurnState());
                }
              
                 CamFollow.inst.ChangeCameraState(CameraState.LOCK);
            GameManager.inst.ChangeGameState(GameState.PLAYERUI);
               
            });
            gameObject.SetActive(true);
            SlotInfoDisplay.inst.Apply(s);
            MoveOnScreen();
           
          
            
        }
       
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
    
    void Update(){
        if(GameManager.inst.checkGameState(GameState.ENEMYTURN)){
            return;
        }
       if(!FUCKOFF)
        {
            if(open)
            {
                if(!SkillHandler.inst.open){
                    if(Input.GetKeyDown(KeyCode.D)){
                    MoveRight();
                    }

                    else if(Input.GetKeyDown(KeyCode.A)){
                        MoveLeft();
                    }
                }
              

                if(Input.GetMouseButtonDown(0) | Input.GetKeyDown(KeyCode.Return)) {
                    OpenSubmenu();
                }

            }
            
            if(currentState == ActionMenuState.ROAM && !open){
                if(InputManager.inst.player.GetButtonDown("Cancel"))
                {
                  //  if(SkillAimer.inst.)
                   foreach (var item in MapManager.inst.slots)
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
            if(!SkillHandler.inst.open){
                SkillHandler.inst.Open();
                center.gameObject.SetActive(false);
                foreach (var item in icons)
                {item.Value.gameObject.SetActive(false);}
            }
           
               
            break;
            case ActionMenuState.MOVE:
           if(!slot.unit.movedThisTurn){
            FUCKOFF = true;
            rt.DOAnchorPos(hidden,.2f).OnComplete(()=>
            { FUCKOFF = false; open = false;});
           // SlotInfoDisplay.inst.Disable();
            UnitMover.inst.EnterSelectionMode(slot);
            Cursor.lockState = CursorLockMode.Confined;
           }
           else{
            Debug.Log("Error Noise");
           }
         
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

    public void Reset(){BattleTicker.inst.Type(BattleManager.inst. TurnState());
        currentState = ActionMenuState.SKILL;
        border.DORotate(Vector3.zero,0);
        icons[ActionMenuState.SKILL].DOLocalRotate(new Vector3(0,0,0),.25f);
        icons[ActionMenuState.ROAM].DOLocalRotate(new Vector3(0,0,180),.25f);
        icons[ActionMenuState.INTERACT].DOLocalRotate(new Vector3(0,0,-90),.25f);
        icons[ActionMenuState.MOVE].DOLocalRotate(new Vector3(0,0,90),.25f);
        center.text = currentState.ToString();
    }

    public void ChangeActionMenuState(ActionMenuState newState)
    {currentState = newState;}

    public void MoveRight()
    {
        switch(currentState)
        {
            case ActionMenuState.SKILL:
            ChangeState(ActionMenuState.MOVE);
            break;

            case ActionMenuState.MOVE:
            ChangeState(ActionMenuState.ROAM);
            break;

            case ActionMenuState.ROAM:
            ChangeState(ActionMenuState.INTERACT);
            break;

            case ActionMenuState.INTERACT:
            ChangeState(ActionMenuState.SKILL);
            break;
        }

       
    }

    public void MoveLeft()
    {
        switch(currentState)
        {
            case ActionMenuState.SKILL:
           
            ChangeState(ActionMenuState.INTERACT);
            break;

            case ActionMenuState.MOVE:
          
            ChangeState(ActionMenuState.SKILL);
            break;

            case ActionMenuState.INTERACT:
            
            ChangeState(ActionMenuState.ROAM);
            break;

            case ActionMenuState.ROAM:
           
            ChangeState(ActionMenuState.MOVE);
            break;

        }
          center.text = currentState.ToString();
    }

    public void ReturnFromSkillMenu()
    {BattleTicker.inst.Type(BattleManager.inst. TurnState());
        center.gameObject.SetActive(true);
        foreach (var item in icons)
        {item.Value.gameObject.SetActive(true);}
        Reset();
    }


    public void ChangeState(ActionMenuState newState)
    {
        ChangeActionMenuState(newState);
        switch(newState)
        {
            case ActionMenuState.SKILL:
            border.DORotate(Vector3.zero,.25f);
            icons[ActionMenuState.SKILL].DOLocalRotate(new Vector3(0,0,0),.25f);
            icons[ActionMenuState.ROAM].DOLocalRotate(new Vector3(0,0,180),.25f);
            icons[ActionMenuState.INTERACT].DOLocalRotate(new Vector3(0,0,-90),.25f);
            icons[ActionMenuState.MOVE].DOLocalRotate(new Vector3(0,0,90),.25f);
            break;

            case ActionMenuState.MOVE:
            border.DORotate(new Vector3(0,0,90),.25f);
            icons[ActionMenuState.SKILL].DOLocalRotate(new Vector3(0,0,-90),.25f);
            icons[ActionMenuState.ROAM].DOLocalRotate(new Vector3(0,0,90),.25f);
            icons[ActionMenuState.INTERACT].DOLocalRotate(new Vector3(0,0,180),.25f);
            icons[ActionMenuState.MOVE].DOLocalRotate(new Vector3(0,0,0),.25f);
            break;

            case ActionMenuState.ROAM:
            border.DORotate(new Vector3(0,0,180),.25f);
            icons[ActionMenuState.SKILL].DOLocalRotate(new Vector3(0,0,180),.25f);
            icons[ActionMenuState.ROAM].DOLocalRotate(new Vector3(0,0,0),.25f);
            icons[ActionMenuState.INTERACT].DOLocalRotate(new Vector3(0,0,90),.25f);
            icons[ActionMenuState.MOVE].DOLocalRotate(new Vector3(0,0,-90),.25f);
            break;

            case ActionMenuState.INTERACT:
            border.DORotate(new Vector3(0,0, -90),.25f);
            icons[ActionMenuState.SKILL].DOLocalRotate(new Vector3(0,0,90),.25f);
            icons[ActionMenuState.ROAM].DOLocalRotate(new Vector3(0,0,-90),.25f);
            icons[ActionMenuState.INTERACT].DOLocalRotate(new Vector3(0,0,0),.25f);
            icons[ActionMenuState.MOVE].DOLocalRotate(new Vector3(0,0,180),.25f);
            break;
        }
         center.text = currentState.ToString();
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


    
