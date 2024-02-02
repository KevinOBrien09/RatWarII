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
    IEnumerator Start()
    {
        yield return new WaitForEndOfFrame();
        List<Slot> shuffle = MapManager.inst.StartingRadius();
      
        for (int i = 0; i < 4; i++)
        {CreatePlayerUnit(shuffle[i]);}

        for (int i = 0; i < 4; i++)
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

    public string TurnState()
    {
        int XD = turnOrder.Count+1;
        return "Turn:" + turn.ToString()+"-"+XD.ToString();
    }

    public Unit CreatePlayerUnit(Slot slot)
    {
        Unit u =  CreateUnit(slot,CharacterBuilder.inst.Generate(),Side.PLAYER);
      
        playerUnits.Add(u);
         MapManager.inst.grid.UpdateGrid();
        return u;
    }

    public Unit CreateEnemyUnit(Slot slot){
        Enemy e =enemies[Random.Range(0,enemies.Count)] ;
        Unit u =  CreateUnit(slot, CharacterBuilder.inst.GenerateEnemy(e),Side.ENEMY);
        u.enemy = e;
        enemyUnits.Add(u);
         MapManager.inst.grid.UpdateGrid();
        return u;
    }

    public void ToggleHealthBars(bool state)
    {
        foreach (var item in allUnits())
        {item.healthBar.gameObject.transform.parent.gameObject. SetActive(state);}
    }

  

    Unit CreateUnit(Slot slot,CharacterGraphic characterGraphic,Side side)
    {
        Unit u = Instantiate(unitPrefab);
        characterGraphic.unit = u;
        u.side = side;
        CharacterBuilder.inst.GenerateStatsAndSkills(  characterGraphic.character);
      
        u.RecieveGraphic(characterGraphic);
        u.health.Init(u.stats().hp);
       
        
        u.transform.position = new Vector3(slot.transform.position.x,u.transform.position.y,slot.transform.position.z);
        u.Reposition(slot);
        slot.unit = u;  
        u.gameObject.name = characterGraphic.character.characterName.fullName();
       
        return u;          
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
