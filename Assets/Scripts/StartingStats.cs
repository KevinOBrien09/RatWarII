using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "StartingStats")]
public class StartingStats : ScriptableObject

{
    public Vector2 hp;
    public Vector2 resource;
    public Vector2 speed;
    public Vector2 strength;
    public Vector2 magic;
    public int moveRange;
    public Skill bnbSkill;
    public List<Skill> otherSkills = new List<Skill>();

}