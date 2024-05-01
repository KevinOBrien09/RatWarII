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
    public BoatMover boatMover;
    public void EnterSelectionMode(Slot sSlot)
    {
        if(!inCoro)
        {
            if(sSlot.isBoat)
            {
                if(boatMover != null)
                {boatMover.EnterSelection(sSlot);}
                else
                { Debug.LogAssertion("BOAT MOVER IS NULL!!!");}
            }
            inCoro = true;
            selectedUnit = sSlot.cont. unit;
            unitStartRot = selectedUnit.transform.rotation;
            if(selectedUnit.side == Side.PLAYER)
            { Minimap.inst.ResizeFOV(sSlot.cont. unit.stats().moveRange);
            Minimap.inst.Show();
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
            int  moveRange =sSlot.cont. unit.stats().moveRange;
           
            if(MapManager.inst.mapQuirk == MapQuirk.ROOMS &&  !BattleManager.inst.roomLockDown)
            {moveRange = 99;}
            validSlots =sSlot.func.GetRadiusSlots(moveRange,null,false);
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
        if(CamFollow.inst.CheckCameraState(CameraState.FOCUS)|BattleManager.inst.gameOver)
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
            
            if(boatMover != null)
            {boatMover.ExitSelection();}
            else
            { Debug.LogWarning("BOAT MOVER IS NULL (only matters in island level tho!!!)");}
            
            foreach (var item in  validSlots)
            { item.ChangeColour(item.normalColour); }
           
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
        if(!slot.isBoat){
            selectedUnit.transform.SetParent(null);
        }
         Minimap.inst.Hide();
        GameManager.inst.ChangeGameState(GameState.UNITMOVE);
        CamFollow.inst.target =  selectedUnit.transform;
        Cursor.lockState = CursorLockMode.Locked;
        ExitSelectionMode();
        BattleTicker.inst.Type("Moving..");
 
        selectedUnit.slot.ChangeColour(selectedUnit.slot.normalColour);
        List<Node> path =  MapManager.inst.map.aStar. FindPath(UnitMover.inst.selectedSlot.node,
       slot.node);
        Queue<Slot> q = new Queue<Slot>();
        foreach (var item in path)
        {   q.Enqueue(item.slot); }
        selectedUnit.MoveAlongPath(q,slot);
        validSlots.Clear();
        CamFollow.inst.ChangeCameraState(CameraState.LOCK);
     
    }

   


    void LateUpdate()
    {
        if(GameManager.inst.checkGameState(GameState.PLAYERSELECT) && !inCoro && !ActionMenu.inst.FUCKOFF && !SkillAimer.inst.castDecided)
        {
            if(InputManager.inst.player. GetButtonDown("Cancel"))
            {
                ExitSelectionMode();
                 MapManager.inst.map.UpdateGrid();
                ActionMenu.inst.Show(selectedSlot);
               
                //CamFollow.inst.ChangeCameraState(CameraState.FREE);
            }
        }
    }


}