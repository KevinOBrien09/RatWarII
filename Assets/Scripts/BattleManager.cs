using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class BattleManager : Singleton<BattleManager>
{
    public Unit unitPrefab;
    public List<Unit> playerUnits = new List<Unit>();
    public List<Unit> enemyUnits = new List<Unit>();
    public Unit currentUnit;
    public int turn;
    public Queue<Unit> turnOrder = new Queue<Unit>();
    public List<Enemy> enemies = new List<Enemy>();
    public int howManyPartyMembers,howManyEnemies;
    IEnumerator Start()
    {
        yield return new WaitForEndOfFrame();
        List<Slot> shuffle = MapManager.inst.StartingRadius();
      
        for (int i = 0; i < howManyPartyMembers; i++)
        {CreatePlayerUnit(shuffle[i]);}

        for (int i = 0; i < howManyEnemies; i++)
        {CreateEnemyUnit(MapManager.inst.RandomSlot());}
      
        ToggleHealthBars(false);
        NewTurn();
    }

    public void NewTurn()
    {
        BattleTicker.inst.Type(BattleManager.inst. TurnState());
        List<Unit> u = new List<Unit>(allUnits());
        List<Unit> ordered = u.OrderBy(f => f.stats().speed).ToList();
        ordered.Reverse();
        foreach (var item in ordered)
        {turnOrder.Enqueue(item);}

        UnitIteration();
        turn++;
        
    }

    public void EndTurn(){
    
           if(BattleManager.inst.turn != 0)
        {BattleManager.inst.UnitIteration();    currentUnit.activeUnitIndicator.gameObject.SetActive(false);}
           MapManager.inst.grid.UpdateGrid();
        ActionMenu.inst.Hide();
    }

    public void UnitIteration()
    {
        StartCoroutine(q());
        IEnumerator q(){
            
            if(turnOrder.Count > 0)
            { 
                ActionMenu.inst.FUCKOFF = false;
                
                yield return new WaitForSeconds(.25f);
                BattleTicker.inst.Type(BattleManager.inst. TurnState());
                currentUnit = turnOrder.Dequeue();
                CamFollow.inst.target = currentUnit.slot. transform;
                if(currentUnit.side == Side.PLAYER)
                {       
                    
                    
                    ActionMenu.inst.  FUCKOFF = false;
                    GameManager.inst.ChangeGameState(GameState.PLAYERUI);
                     ActionMenu.inst.Reset();
                    ActionMenu.inst.Show(currentUnit.slot);
                    SkillHandler.inst.NewUnit(currentUnit);
                    CamFollow.inst.Focus(currentUnit.slot.transform,()=>{});
                    currentUnit.activeUnitIndicator.gameObject.SetActive(true);

                }
                else
                {
                       yield return new WaitForSeconds(.25f);
                       UnitIteration();
                    //AI
                }
            }
            else
            {
                NewTurn();
            }
        }
        
    }

    public void UnitIsDead(Unit u){
        if(u.side == Side.ENEMY){
            enemyUnits.Remove(u);
        }
        else{
            playerUnits.Remove(u);
        }
        List<Unit> XD = new List<Unit>();
        XD.Add(u);
       turnOrder = new Queue<Unit>(turnOrder.Where(x => !XD.Contains(x)));
    }

    public string TurnState()
    {
        int XD = turnOrder.Count+1;
        return "Turn:" + turn.ToString()+"-"+XD.ToString();
    }

    public Unit CreatePlayerUnit(Slot slot)
    {

        Unit u =  Instantiate(unitPrefab);
        CharacterGraphic graphic =  CharacterBuilder.inst.Generate();
        u.character = graphic.character;
        graphic.unit = u;
        u.side = Side.PLAYER;
       (Stats stats,List<Skill> skills ) statsAndSkills = CharacterBuilder.inst.GenerateStatsAndSkills(CharacterBuilder.inst.jobDict[u.character.job]);
        u.character.baseStats = statsAndSkills.stats;
        u.character.skills = new List<Skill>( statsAndSkills.skills);
        u.RecieveGraphic(graphic);
        u.health.Init(u.stats().hp);
        u.gameObject.name = graphic.character.characterName.fullName();
        ReposUnit(u,slot);
        playerUnits.Add(u);
        return u;
    }

    public Unit CreateEnemyUnit(Slot slot)
    {
        Enemy e =enemies[Random.Range(0,enemies.Count)];
        CharacterGraphic graphic =  CharacterBuilder.inst.GenerateEnemy(e);
        Unit u =  Instantiate(unitPrefab);
        u.enemy = e;
        u.character = graphic.character;
        graphic.unit = u;
        u.side = Side.ENEMY;
        (Stats stats,List<Skill> skills ) statsAndSkills = CharacterBuilder.inst.GenerateStatsAndSkills(e.startingStats);
        u.character.baseStats = statsAndSkills.stats;
        u.character.skills = new List<Skill>( statsAndSkills.skills);
        u.RecieveGraphic(graphic);
        u.health.Init(u.stats().hp);
        u.gameObject.name = graphic.character.characterName.fullName();
        ReposUnit(u,slot);

        enemyUnits.Add(u);
        return u;
    }

    public void ReposUnit(Unit u,Slot slot){
        u.transform.position = new Vector3(slot.transform.position.x,u.transform.position.y,slot.transform.position.z);
        u.Reposition(slot);
        slot.unit = u;  
        MapManager.inst.grid.UpdateGrid();
    }

    public void ToggleHealthBars(bool state)
    {
        foreach (var item in allUnits())
        {item.healthBar.gameObject.transform.parent.gameObject. SetActive(state);}
    }


    public List<Unit> allUnits()
    {
        List<Unit> u = new List<Unit>();
        foreach (var item in playerUnits)
        {u.Add(item);}
        foreach (var item in enemyUnits)
        {u.Add(item);}

        return u;
    }
}
