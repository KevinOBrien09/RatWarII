using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "Skill/SkillBase")]
public class Skill : ScriptableObject

{
    
    public string skillName;
    [TextArea(10,10)]  public string desc;


    public void Go()
    {Debug.Log(skillName);}

}