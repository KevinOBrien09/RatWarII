using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "Skill/SkillBase")]
public class Skill : GUIDScriptableObject
{
    public string skillName;
    [TextArea(10,10)]  public string desc;
    public List<Effect> effects = new List<Effect>();
    public Side side;
    public SkillCastBehaviour skillCastBehaviour;

    public void Go()
    {Debug.Log(skillName);}

}