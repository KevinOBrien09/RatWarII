using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Species{RAT,MOUSE,ROBIN,FROG} //,FROG
public enum Gender{NA,MALE,FEMALE}
public enum Job{KNIGHT,WIZARD,ARCHER}
[System.Serializable]
public class Character 
{
    public CharacterName characterName;
    public Species species;
    public Gender gender;
    public Job job;
    public Stats baseStats;
    public EXP exp;
    public ColourVarient colourVarient;
    public List<Skill> skills = new List<Skill>();
}