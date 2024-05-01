using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum StatEnum{NA,RES_REGEN,SPEED,STRENGTH,MAGIC,MOVE_RANGE,DEFENCE}
[System.Serializable]
public class Stats
{
    public int hp;
    public int resource;
    public int speed;
    public int strength;
    public int magic;
    public int moveRange = 0;
    public int defence;
    public bool passable;

    public void Edit(StatEnum statEnum,int i){
        switch(statEnum){

            case StatEnum.SPEED:
            speed += i;
            break;
            case StatEnum.STRENGTH:
            strength += i;
         
            break;
            case StatEnum.MAGIC:
            magic += i;
            break;
            case StatEnum.MOVE_RANGE:
            moveRange += i;
            break;
            case StatEnum.DEFENCE:
            defence += i;
            break;
            default:

            break;
        }
    }
    

    public Stats Stack(Stats statMods)
    {
        Stats s = new Stats();
        s.hp = hp + statMods.hp;
        s.resource = resource + statMods.resource;

        s.strength = strength + statMods.strength;
        s.strength = (int)Mathf.Clamp(s.strength,0,Mathf.Infinity);

        s.speed = speed + statMods.speed;
        s.speed = (int)Mathf.Clamp(s.speed,0,Mathf.Infinity);

        s.moveRange = moveRange + statMods.moveRange;
        s.moveRange = (int)Mathf.Clamp(s.moveRange,1,15);

        s.magic = magic + statMods.magic;
        s.magic = (int)Mathf.Clamp(s.magic,0,Mathf.Infinity);

        s.defence = defence+ statMods.defence;
        s.defence = (int)Mathf.Clamp(s.defence,-Mathf.Infinity,100);

        s.passable = passable;
        return s;
    }
}