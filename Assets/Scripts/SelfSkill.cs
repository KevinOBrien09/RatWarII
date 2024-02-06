using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "Skill/Self")]
public class SelfSkill : Skill
{
    public int radius;
    public bool castOnCasterSlot;
    public bool showHealthBars;
}