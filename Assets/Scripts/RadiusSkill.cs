using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "Skill/Radius")]
public class RadiusSkill : Skill

{
    public int radius;
    public bool canSelfCast,showHealthBars,cannotCastOnSpecialSlot;
}