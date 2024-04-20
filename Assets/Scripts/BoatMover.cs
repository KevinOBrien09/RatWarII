using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class BoatMover 
{
    public Boat boat;
    public List<BoatSlot> validSlots = new List<BoatSlot>();
    public     SwampGeneratorBrain swampGeneratorBrain;
    public  void Init(SwampGeneratorBrain sgb){
        swampGeneratorBrain = sgb;
    }
    public void EnterSelection(Slot poopdeckSlot)
    {
    //  if(!boat.movedThisTurn ){
    //     validSlots = boat.Grab(5);

        foreach (var item in validSlots)
        {
            item.Toggle(true);
        }
     //}
      
    }


    public void ExitSelection(){
        foreach (var item in validSlots)
        {
            item.Toggle(false);
            item.indicator.color =  item.normalColour;
        }
        validSlots.Clear();
    }

    public void InitializeMove(BoatSlot target){
        GameManager.inst.ChangeGameState(GameState.UNITMOVE);
     //   CamFollow.inst.target =  boat.transform;
     CamFollow.inst.target =  BattleManager.inst.currentUnit.transform;
        Cursor.lockState = CursorLockMode.Locked;
        UnitMover.inst.ExitSelectionMode();
        BattleTicker.inst.Type("Sailing..");
        boat.Depart();
        List<Node> path =  swampGeneratorBrain.waterGrid .aStar.FindPath(UnitMover.inst.boatMover.boat.slot.node,target.node);
        Queue<Slot> q = new Queue<Slot>();
        foreach (var item in path)
        {   q.Enqueue(item.slot); }
        boat.MoveAlongPath(q,target,(()=>
        {
            boat.Dock();
            boat.Reposition(target);
            GameManager.inst.ChangeGameState(GameState.PLAYERUI);
            ActionMenu.inst.Reset();
            BattleManager.inst.currentUnit.DeductMoveToken();
            //boat.movedThisTurn = true;
            ActionMenu.inst.Show(BattleManager.inst.currentUnit.slot);
            MapManager.inst.map.UpdateGrid();
            swampGeneratorBrain.waterGrid.UpdateGrid();
          
        }));
        
        CamFollow.inst.ChangeCameraState(CameraState.LOCK);
    }
}