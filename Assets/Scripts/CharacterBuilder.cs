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
     public GenericDictionary<Species,Sprite> female = new GenericDictionary<Species, Sprite>();
    public GenericDictionary<Species,GenericDictionary<Job,List<Sprite>>> classVarients = new GenericDictionary<Species, GenericDictionary<Job, List<Sprite>>>();
    public List<Skill> mandatorySkills = new List<Skill>();
    public List<Skill> allSkills = new List<Skill>();
    public List<Trait> allTraits = new List<Trait>();
    public GenericDictionary<string,Skill> skillDict = new GenericDictionary<string, Skill>();
    public GenericDictionary<string,Trait> traitDict = new GenericDictionary<string, Trait>();
    public bool addAllSkills;

    protected override  void Awake(){
        base.Awake();
        foreach (var item in allSkills)
        {
            skillDict.Add(item.ID,item);
        }
        foreach (var item in allTraits)
        {
            traitDict.Add(item.ID,item);
        }
    }

    public Character GenerateCharacter()
    {
        Character character = new Character();
        character.ID = System.Guid.NewGuid().ToString();
        character.species = MiscFunctions.RandomEnumValue<Species>();
        character.job = MiscFunctions.RandomEnumValue<Job>();
        character.exp = new EXP();
        character.exp.Init(0);
        character.exp.level = 1;
        character.gender = GetGender();   
        character.spriteVarient = GetRandomSpriteVar(character);
        character.characterName = nameDict[character.species].GenName(character);
        (Stats stats,List<Skill> skills ) statsAndSkills = CharacterBuilder.inst.GenerateStatsAndSkills(CharacterBuilder.inst.jobDict[character.job],character);
        character.baseStats = statsAndSkills.stats;
        character.skills = new List<Skill>( statsAndSkills.skills);
        if(!IconGraphicHolder.inst.dict.ContainsKey(character.ID))
        { IconGraphicHolder.inst.MakeIcon(character);}
        character.battleData = new CharacterBattleData();
        character.battleData.currentHP = character.stats().hp;
       
        character.battleData.currentResource = character.stats().resource;
        
        return character;
    }
    public CharacterGraphic GenerateGraphic(Character c)
    {
        CharacterGraphic cg =  Instantiate(characerGraphicPrefab,Vector3.zero,Quaternion.identity);
        cg.Init(c);
     
       
        cg.character = c;
        return cg;
    }

    public Character GenerateFromSave(CharacterSaveData saveData){
        Character character = new Character();
        character.ID = saveData.ID;
        character.species = saveData.species;
        character.job = saveData.job;
        character.exp = new EXP();
        character.exp.Load(saveData.expSave);
       
        character.gender = saveData.gender;
        character.spriteVarient = saveData.spriteVarient;
        character.characterName = saveData.characterName;
       
        character.baseStats =saveData.baseStats;

        List<Skill> skills = new List<Skill>();
        foreach (var item in saveData.skills)
        {skills.Add(skillDict[item]);}
        character.skills = new List<Skill>( skills);

        List<Trait> traits = new List<Trait>();
        foreach (var item in saveData.traits)
        {traits.Add(traitDict[item]);}
        character.traits = new List<Trait>( traits);

        if(!IconGraphicHolder.inst.dict.ContainsKey(character.ID))
        { IconGraphicHolder.inst.MakeIcon(character);}
        character.battleData = new CharacterBattleData();
        character.battleData.Load(saveData.battleData);
        return character;
    }


    public int GetRandomSpriteVar(Character c)
    {return  Random.Range(0,classVarients[c.species][c.job].Count);}

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
        s.resource = (int)Random.Range(ss.resource.x, ss.resource.y);
        s.strength = (int)Random.Range(ss.strength.x,ss.strength.y);
        s.speed = (int)Random.Range(ss.speed.x, ss.speed.y);
        s.magic = (int)Random.Range(ss.magic.x,ss.magic.y);
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
