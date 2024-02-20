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

    public Unit CreatePlayerUnit(Slot slot)
    {

        Unit u =  Instantiate(unitPrefab);
        CharacterGraphic graphic =  CharacterBuilder.inst.Generate();
        u.character = graphic.character;
        graphic.unit = u;
        u.side = Side.PLAYER;
       (Stats stats,List<Skill> skills ) statsAndSkills = CharacterBuilder.inst.GenerateStatsAndSkills(CharacterBuilder.inst.jobDict[u.character.job],u.character);
        u.character.baseStats = statsAndSkills.stats;
        u.character.skills = new List<Skill>( statsAndSkills.skills);
        u.RecieveGraphic(graphic);
        u.health.Init(u.stats().hp);
        u.gameObject.name = graphic.character.characterName.fullName();
        ReposUnit(u,slot);
        if(CharacterBuilder.inst.sfxDict.ContainsKey(u.character.species))
        {
            u.sounds = CharacterBuilder.inst.sfxDict[u.character.species];
        }
   
        BattleManager.inst.playerUnits.Add(u);
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
        u.health.Init(u.stats().hp);
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
        MapManager.inst.grid.UpdateGrid();
    }
}