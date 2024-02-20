using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class UnitMover : Singleton<UnitMover>
{
    public Slot selectedSlot;
    public List<Slot> validSlots = new List<Slot>();
    Unit selectedUnit;
    public Quaternion unitStartRot;
    public bool inCoro;
    public Color32 baseSlotColour,validSlotColour;

    public void EnterSelectionMode(Slot sSlot)
    {
        if(!inCoro)
        {
            inCoro = true;
            selectedUnit = sSlot.cont. unit;
            unitStartRot = selectedUnit.transform.rotation;
            if(selectedUnit.side == Side.PLAYER)
            {
                BattleTicker.inst.Type("Preparing Move...");
            }
            CamFollow.inst.Focus(sSlot.cont. unit.transform,()=>
            {
                if(selectedUnit.side == Side.PLAYER)
                {
                    BattleTicker.inst.Type("Preparing Move...");
                    GameManager.inst.ChangeGameState(GameState.PLAYERSELECT);
                CamFollow.inst.ChangeCameraState(CameraState.FREE);
                }
                inCoro = false;
            });
            
            selectedSlot = sSlot;
            validSlots =sSlot.func.GetRadiusSlots(sSlot.cont. unit.stats().moveRange,null,true);
            if(selectedUnit.side == Side.PLAYER){
            SelectionUI(sSlot);
            }
       
        }
    }

    public void SelectionUI(Slot sSlot)
    {
        SlotInfoDisplay.inst.Apply(sSlot);
        SlotInfoDisplay.inst.Disable();

        foreach (var item in validSlots)
        {item.ChangeColour(item.moveColour);} 
    }

    public void NewHover(List<Node> nodes)
    {
        if(GameManager.inst.checkGameState(GameState.UNITMOVE))
        { return; }
        if(!inCoro)
        {
           

             DirectionIndicator.inst. GetLayout(nodes);
              
        
                // foreach (var item in nodes)
                // {
                //     if(item.slot == selectedSlot)
                //     {continue;}
                //     item.slot.indicator.SetActive(true);
                // }
            
        }
    }
    
    public void ExitSelectionMode()
    {
        if(CamFollow.inst.CheckCameraState(CameraState.FOCUS))
        {return; }

        if( GameManager.inst.checkGameState(GameState.ENEMYTURN) || GameManager.inst.checkGameState(GameState.PLAYERSELECT)||GameManager.inst.checkGameState(GameState.UNITMOVE) && !inCoro)
        {
            StartCoroutine(q());
        }
        IEnumerator q()
        {
            inCoro = true;
            CamFollow.inst.ZoomOut();
           DirectionIndicator.inst.Reset();
            foreach (var item in MapManager.inst.slots)
            {
            
                item.ChangeColour(item.normalColour);
              
            }
            MapManager.inst.fuckYouSlots.Clear();
            selectedUnit.transform.rotation = unitStartRot;
            validSlots.Clear();
            // if(!GameManager.inst.checkGameState(GameState.UNITMOVE))
            // {GameManager.inst.ChangeGameState(GameState.PLAYERHOVER);}
           
            selectedSlot.ChangeColour(selectedSlot.normalColour); //??
            while(GameManager.inst.checkGameState(GameState.UNITMOVE))
            {break;}
            yield return new WaitForSeconds(.15f);
            inCoro = false;
            selectedSlot = null;
        }
    }

    public void InitializeMove(Slot slot)
    {
        if(slot.cont.unit != null)
        {return;}
        GameManager.inst.ChangeGameState(GameState.UNITMOVE);
        CamFollow.inst.target =  selectedUnit.transform;
        Cursor.lockState = CursorLockMode.Locked;
        ExitSelectionMode();
       BattleTicker.inst.Type("Moving..");
        selectedUnit.activeUnitIndicator.gameObject.SetActive(false);
        selectedUnit.slot.ChangeColour(selectedUnit.slot.normalColour);
        List<Node> path = MapManager.inst.aStar.FindPath(UnitMover.inst.selectedSlot.transform.position,
       slot.transform.position);
        Queue<Slot> q = new Queue<Slot>();
        foreach (var item in path)
        {   q.Enqueue(item.slot); }
        selectedUnit.MoveAlongPath(q,slot);
        
        CamFollow.inst.ChangeCameraState(CameraState.LOCK);
     
    }

    void LateUpdate()
    {
        if(GameManager.inst.checkGameState(GameState.PLAYERSELECT) && !inCoro && !ActionMenu.inst.FUCKOFF)
        {
            if(InputManager.inst.player. GetButtonDown("Cancel"))
            {
                ExitSelectionMode();
                MapManager.inst.grid.UpdateGrid();
                ActionMenu.inst.Show(selectedSlot);
                //CamFollow.inst.ChangeCameraState(CameraState.FREE);
            }
        }
    }


}