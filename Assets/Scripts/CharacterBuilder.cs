using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CharacterBuilder : Singleton<CharacterBuilder>
{
    public GenericDictionary<Species,Names> nameDict = new GenericDictionary<Species,Names>();
    public CharacterGraphic characerGraphicPrefab;
    public GenericDictionary<Job,StartingStats> jobDict = new GenericDictionary<Job,StartingStats>();
    public GenericDictionary<Species,List<Skill>> speciesSkill = new GenericDictionary<Species,List<Skill>>();
    public GenericDictionary<Species,CharacterSounds> sfxDict = new GenericDictionary<Species, CharacterSounds>();
    public List<Skill> mandatorySkills = new List<Skill>();
    public List<Skill> allSkills = new List<Skill>();
    public bool addAllSkills;
    public CharacterGraphic Generate()
    {
        Character character = new Character();
        character.species = MiscFunctions.RandomEnumValue<Species>();
        character.job = MiscFunctions.RandomEnumValue<Job>();
        character.exp = new EXP();
        character.exp.level = 1;
        character.gender = GetGender();   
        character.spriteVarient = characerGraphicPrefab.GetRandomSpriteVar(character);
        CharacterGraphic cg =  Instantiate(characerGraphicPrefab,Vector3.zero,Quaternion.identity);
        character.characterName = nameDict[character.species].GenName(character);
        cg.Init(character);
        cg.character = character;
        //currentChar = cg;
        return cg;
    }

    public CharacterGraphic GenerateEnemy(DefinedCharacter e){
        Character character = new Character();
        character.exp = new EXP();
        character.exp.level = 1;
        character.gender = GetGender(); 
        CharacterGraphic cg =  Instantiate(e.characterGraphic);
        character.characterName = new CharacterName();
        character.characterName.firstName = e.names[Random.Range(0,e.names.Count)];
        cg.EnemyInit(e);
        cg.character = character;
        return cg;

    }

    public (Stats stats,List<Skill> skills ) GenerateStatsAndSkills(StartingStats ss,Character c)
    {return (genStats(ss), genSkills(ss,c));}


    public Stats genStats(StartingStats ss)
    {
        
      
        Stats s = new Stats();
        s.hp = (int)Random.Range(ss.hp.x, ss.hp.y);
        s.strength = (int)Random.Range(ss.strength.x,ss.strength.y);
        s.speed = (int)Random.Range(ss.speed.x, ss.speed.y);
        s.moveRange = ss.moveRange;
        s.passable = false;
        return s;
    }

    public List<Skill> genSkills(StartingStats startingStats,Character c)
    {
        List<Skill> skills = new List<Skill>();
        if(addAllSkills)
        {
            foreach (var item in allSkills)
            {skills.Add(item); }
        }
        else
        {
            foreach (var item in mandatorySkills)
            {skills.Add(item);}
            
            skills.Add(startingStats.bnbSkill);
            if(startingStats.otherSkills.Count > 0){
            skills.Add(startingStats.otherSkills[Random.Range(0,startingStats.otherSkills.Count)]);
            }
            
            if(speciesSkill.ContainsKey(c.species))
            {
                skills.Add(speciesSkill[c.species][Random.Range(0,speciesSkill[c.species].Count)]);
            }
        }
        
        return skills;  
    }

    Gender GetGender(){
        Gender g = Gender.NA;
        if(Random.Range(0,2) == 1)
        {g = Gender.MALE;}
        else
        {g = Gender.FEMALE;}
        return g;
    }
      
}
