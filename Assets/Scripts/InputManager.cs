using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;

public class InputManager : Singleton<InputManager>
{
    public Player player;
    protected override   void Awake(){
        base.Awake();
        player = Rewired.ReInput.players.GetPlayer(0);

    }

    public bool AnyWASDKeyHeld()
    {
        if(player.GetButton("Up") | player.GetButton("Down") | player.GetButton("Left")| player.GetButton("Right"))
        { return true; }
        return false;
    }

}