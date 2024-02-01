using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public enum ColourVarient{RED,BLUE,GREEN}
public class CharacterBuilder : Singleton<CharacterBuilder>
{
    public GenericDictionary<Species,Names> nameDict = new GenericDictionary<Species,Names>();
    public GenericDictionary<Species,CharacterGraphic> graphicDict = new GenericDictionary<Species, CharacterGraphic>();
    public GenericDictionary<Job,StartingStats> jobDict = new GenericDictionary<Job,StartingStats>();
    public CharacterGraphic currentChar;
    public TextMeshProUGUI charNameTxt;
    public Skill skipSkill;
    public Character character;
    public bool DEBUG;
    void Start(){
        if(DEBUG){
            character =  Generate().character;
            charNameTxt.text = character.characterName.fullName();
        }
     
    
    }

    void Update()
    {
        if(DEBUG){
            if(Input.GetKeyDown(KeyCode.Space)){
                Destroy(currentChar.gameObject);
            character =  Generate().character;
            charNameTxt.text = character.characterName.fullName();
            }
        }
       
    }

    public CharacterGraphic Generate()
    {
        Character character = new Character();
        character.species = MiscFunctions.RandomEnumValue<Species>();
        character.job = MiscFunctions.RandomEnumValue<Job>();
        character.exp = new EXP();
        character.exp.level = 1;
        character.gender = GetGender();   
        character.colourVarient = MiscFunctions.RandomEnumValue<ColourVarient>();
        character.baseStats = genStats(character);
        character.skills = genSkills(character);
        CharacterGraphic cg =  Instantiate(graphicDict[character.species],Vector3.zero,Quaternion.identity);
        character.characterName = nameDict[character.species].GenName(character);
        cg.Init(character);
        cg.character = character;
        //currentChar = cg;
        return cg;
    }

    public Stats genStats(Character c)
    {
        StartingStats ss = jobDict[c.job];
        Stats s = new Stats();
        s.hp = (int)Random.Range(ss.hp.x, ss.hp.y);
        s.speed = (int)Random.Range(ss.speed.x, ss.speed.y);
        s.moveRange = ss.moveRange;
        s.passable = false;
        return s;
    }

    public List<Skill> genSkills(Character c)
    {
        List<Skill> skills = new List<Skill>();
        StartingStats ss = jobDict[c.job];
           skills.Add(jobDict[Job.KNIGHT].bnbSkill);
              skills.Add(jobDict[Job.WIZARD].bnbSkill);
                 skills.Add(jobDict[Job.ARCHER].bnbSkill);
                 skills.Add(skipSkill);
        // skills.Add(ss.bnbSkill);
        //    skills.Add(ss.bnbSkill);
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
