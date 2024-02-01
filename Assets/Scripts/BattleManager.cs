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
    public CharacterGraphic fly;
    void Start()
    {
        CreatePlayerUnit(248);
        CreatePlayerUnit(239);
        CreatePlayerUnit(99);
        CreatePlayerUnit(89);
 
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

    public Unit CreatePlayerUnit(int slot)
    {
        Unit u =  CreateUnit(slot);
        u.side = Side.PLAYER;
        playerUnits.Add(u);
        return u;
    }

  

    Unit CreateUnit(int i)
    {
        Unit u = Instantiate(unitPrefab);
        CharacterGraphic characterGraphic = CharacterBuilder.inst.Generate();
        u.RecieveGraphic(characterGraphic);
        u.currentHP = u.stats().hp;
        // u.character.baseStats = new Stats();
        // u.character.baseStats.speed = (int)Random.Range(1,10);
        Slot s = MapManager.inst.slots[i];
        u.transform.position = new Vector3(s.transform.position.x,u.transform.position.y,s.transform.position.z);
        u.Reposition(s);
        s.unit = u;  
        u.gameObject.name = characterGraphic.character.characterName.fullName();
        MapManager.inst.grid.UpdateGrid();
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
