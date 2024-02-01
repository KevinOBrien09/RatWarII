using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class Stats
{
    public int hp;
    public int speed;
    public int moveRange = 0;
    public bool passable;
    

    public Stats Stack(Stats statMods)
    {
        Stats s = new Stats();
        s.hp = hp + statMods.hp;
        s.speed = speed + statMods.speed;
        s.moveRange = moveRange + statMods.moveRange;
        s.passable = passable;

        return s;
    }
}