using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.Events;
using DG.Tweening;


public class UnitFactory : Singleton<UnitFactory>
{
    public Unit unitPrefab;
    public List<DefinedCharacter> enemies = new List<DefinedCharacter>();
   
    public bool debug;
    public Unit CreatePlayerUnit(Slot slot,Character c = null)
    {

        Unit u =  Instantiate(unitPrefab);
        Character character = null;
        if(c != null)
        {
            character = c;
           
        }
        else if(c == null && debug)
        {
            character = CharacterBuilder.inst.GenerateCharacter();
            Debug.Log("Generated:" + character.characterName.firstName);
        }
        else if(c == null && !debug)
        {
            Debug.LogAssertion("Character is null, Not generating character because not in debug mode.");
            return null;
        }

        CharacterGraphic graphic =  CharacterBuilder.inst.GenerateGraphic(character);
        graphic.KillCamera();
        u.character = graphic.character;
        graphic.unit = u;
        u.side = Side.PLAYER;
      
        u.RecieveGraphic(graphic);
        u.health.Init(u.stats().hp,character.battleData.currentHP);
        u.skillResource.Init(u.stats().resource,character.battleData.currentResource);
        u.health.onRefresh .AddListener(()=>
        { u.character.RefreshBattleData(u) ;});
      

        u.skillResource.onChange.AddListener(()=>
        {u.character.RefreshBattleData(u);});
       

        
        u.skillResource.SetCatagory(u.character.job);

        u.gameObject.name = graphic.character.characterName.fullName();
        ReposUnit(u,slot);
        if(CharacterBuilder.inst.sfxDict.ContainsKey(u.character.species))
        {u.sounds = CharacterBuilder.inst.sfxDict[u.character.species];}
        
        BattleManager.inst.playerUnits.Add(u);
        //strike
        //bnb 1  whirlwind
        //bnb 2 barrier
        //bnb3(species) lick wounds 
        //lvl 3 class
        //lvl 5 species
        //lvl 7 class
        //lvl 8 speices 
        //specialist
        //specialist
        //specialist


        //lvl 1 starting stuff, lvl 2 stats, lvl 3 class ability , lvl 4 stats , lvl 5 speices ability, lvl 6 stats, lvl 7 class ability, lvl 8 speices ability, lvl 9 stats
    
        return u;
    }

    public Unit CreateNPC(Slot slot,DefinedCharacter npc)
    {
        Unit u =  CreateEnemyUnit(slot,npc);
        u.side = Side.NEITHER;
        BattleManager.inst.enemyUnits.Remove(u);
        foreach (var item in CharacterBuilder.inst.mandatorySkills)
        {
            u.character.skills.Add(item);
        }
        return u;
    }

    public Unit CreateEnemyUnit(Slot slot,DefinedCharacter dc = null)
    {
        DefinedCharacter e = null;  
        if(dc == null){
             e =enemies[Random.Range(0,enemies.Count)];
        }
        else{
e = dc;
        }
     
        CharacterGraphic graphic =  CharacterBuilder.inst.GenerateEnemy(e);
        Unit u =  Instantiate(unitPrefab);
        u.sounds = e.sounds;
        u.enemy = e;
        u.character = graphic.character;
        graphic.unit = u;
        u.side = Side.ENEMY;
        u.character.baseStats = CharacterBuilder.inst.genStats(e.startingStats);
        u.RecieveGraphic(graphic);
        u.health.Init(u.stats().hp,u.stats().hp);
        u.skillResource.Init((int) e.startingStats.resource.x,(int)e.startingStats.resource.x);
        u.skillResource.catagory = e.skillResCat;
        u.gameObject.name = graphic.character.characterName.fullName();
        ReposUnit(u,slot);
        u.charAI = Instantiate(e.charAI,u.transform);
        u.charAI.Init(u);
        BattleManager.inst.enemyUnits.Add(u);
        
        return u;
    }

    public void ReposUnit(Unit u,Slot slot){
        u.transform.position = new Vector3(slot.transform.position.x,u.transform.position.y,slot.transform.position.z);
        u.Reposition(slot);
        slot.cont.unit = u;  
         MapManager.inst.map.UpdateGrid();
    }
}