using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "StartingStats")]
public class StartingStats : ScriptableObject

{
    public Vector2 hp;
    public Vector2 speed;
    public int moveRange;
    public Skill bnbSkill;
    public List<Skill> otherSkills = new List<Skill>();

}