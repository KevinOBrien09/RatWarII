using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "Effect/BaseDamage")]
public class DamageEffect : Effect
{
    public int damageValue;
    public override void Go(){
        Debug.Log(name);
    }

    public virtual int GetDamage(){
        return damageValue;
    }

}