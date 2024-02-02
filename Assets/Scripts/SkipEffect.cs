using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "Effect/Skip")]
public class SkipEffect : Effect
{
    public override void Go(){
       SkillAimer.inst.Skip();
    }
}