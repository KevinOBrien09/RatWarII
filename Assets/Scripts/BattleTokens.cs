using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public struct BattleTokens
{
    public int actionToken;
    public int moveToken;
    public int godToken;
    
    public BattleTokens(BattleTokens bt)
    {
        // if(bt != null){
        actionToken = bt.actionToken;
        moveToken = bt.moveToken;
        godToken = bt.godToken;
        // }
        // else{
        //     Set(1,1,0);
        // }
       
    }

    public void DeductMoveToken()
    {
        if(moveToken > 0)
        {
            moveToken--;
        }
        else if (godToken > 0)
        {
            godToken--;
        }
        
        
    }

    public void DeductActionToken(){
        if(actionToken > 0)
        {
           actionToken--;
        }
        else if (godToken > 0)
        {
            godToken--;
        }
        
    }

    public void Set(int action,int move,int god = 0){
        actionToken = action;
        moveToken = move;
        godToken = god;
    }

    public bool canMove(){
        if(godToken > 0){
            return true;
        }
        else if(moveToken > 0){
            return true;
        }
        else{
            return false;
        }
    }

    public bool gotTurnBecauseOfGodToken(){
        return actionToken ==0 && moveToken == 0 && godToken > 0;
    }

    public bool canAct(){
        if(godToken > 0){
            return true;
        }
        else if(actionToken > 0){
            return true;
        }
        else{
            return false;
        }
    }

    public bool canAdd()
    {
        
        bool w = total() >= 5;
        return !w;
    }

    public int total(){
        return actionToken + moveToken + godToken;
    }

    
}

