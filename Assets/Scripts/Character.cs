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
    public List<Trait> traits = new List<Trait>();
    public CharacterBattleData battleData;
    public SkillResource resource;
    public CharacterSaveData Save()
    {
        CharacterSaveData csd = new CharacterSaveData();
        csd.ID = ID;
        csd.characterName = characterName;
        csd.species = species;
        csd.gender = gender;
        csd.job = job;
        csd.baseStats = baseStats;
        csd.expSave = exp.Save();
        csd.spriteVarient = spriteVarient;
        csd.battleData = battleData.Save();
        csd.skills = new List<string>();
        csd.traits = new List<string>();
        foreach (var item in skills)
        { csd.skills.Add(item.ID); }
        foreach (var item in traits)
        { csd.traits.Add(item.ID); }
        return csd;
    }

    public void RefreshBattleData(Unit u){
        battleData.currentHP = u.health.currentHealth;
        battleData.currentResource = u.skillResource.current;
    }

    public Stats stats(){
        return baseStats;
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
    public EXPSave expSave;
    public int spriteVarient;
    public List<string> skills = new List<string>();
    public List<string> traits = new List<string>();
    public CharacterBattleData battleData;
}

[System.Serializable]
public class CharacterBattleData
{
    public int currentHP;
    public int currentResource;
    public void Load(CharacterBattleData cbd)
    {
        currentHP = cbd.currentHP;
        currentResource = cbd.currentResource;
    }

    public CharacterBattleData Save(){
        CharacterBattleData cbd = new CharacterBattleData();
        cbd.currentHP = currentHP;
        cbd.currentResource = currentResource;
        return cbd;
    }
}