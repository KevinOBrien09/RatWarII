using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "Skill/SkillBase")]
public class Skill : GUIDScriptableObject
{
    public string skillName;
    [TextArea(10,10)]  public string desc;
    public int resourceCost,additionalMoveTokenCost;
    public SkillResource.Catagory intendedResource;
    public Side side;
    public SkillCastBehaviour skillCastBehaviour;
    public bool doesntNeedUnitInSlot;
    public Sprite statusEffectIcon;
    public List<SoundData> sounds = new List<SoundData>();
    public List<int> value = new List<int>();
    public bool canHitBreakableSlots;
    public void Go()
    {Debug.Log(skillName);}

}