using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Species{RAT,MOUSE,ROBIN,FROG} //,FROG
public enum Gender{NA,MALE,FEMALE}
public enum Job{KNIGHT,WIZARD,ARCHER}
[System.Serializable]
public class Character 
{
    public string ID;
    public CharacterName characterName;
    public Species species;
    public Gender gender;
    public Job job;
    public Stats baseStats;
    public EXP exp;
    public int spriteVarient;
    public List<Skill> skills = new List<Skill>();

    public CharacterSaveData Save()
    {
        CharacterSaveData csd = new CharacterSaveData();
        csd.ID = ID;
        csd.characterName = characterName;
        csd.species = species;
        csd.gender = gender;
        csd.job = job;
        csd.baseStats = baseStats;
        csd.exp = exp;
        csd.spriteVarient = spriteVarient;
        csd.skills = new List<string>();
        foreach (var item in skills)
        { csd.skills.Add(item.ID); }
        return csd;
    }

    
   
}

[System.Serializable]
public class CharacterSaveData
{
    public string ID;
    public CharacterName characterName;
    public Species species;
    public Gender gender;
    public Job job;
    public Stats baseStats;
    public EXP exp;
    public int spriteVarient;
    public List<string> skills = new List<string>();
}