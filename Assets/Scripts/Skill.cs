using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "Skill/SkillBase")]
public class Skill : Castable
{
    public int resourceCost,additionalMoveTokenCost;
    public SkillResource.Catagory intendedResource;
    public Side side;
    public bool doesntNeedUnitInSlot;
    public bool canHitBreakableSlots;
  

}