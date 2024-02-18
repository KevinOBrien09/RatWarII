using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.Events;
using DG.Tweening;

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
    bool looping;
    public SpikeSlot spikeSlotPrefab;
   
    public AudioSource music;

    IEnumerator Start()
    {
        yield return new WaitForEndOfFrame();
        List<Slot> shuffle = MapManager.inst.StartingRadius();

        
        for (int i = 0; i < 15; i++)
        {
            Slot s = MapManager.inst.RandomSlot();
            s.MakeSpecial(spikeSlotPrefab);
        }
      
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
        {
            
            turnOrder.Enqueue(item);
            item. movedThisTurn = false;
        }

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
        if(playerLose())
        {Lose();}
        else
        {StartCoroutine(q());}
        
        IEnumerator q(){
            
            if(turnOrder.Count > 0)
            { 
                ActionMenu.inst.FUCKOFF = false;
                
                yield return new WaitForSeconds(.25f);
              
                currentUnit = turnOrder.Dequeue();
                bool terrainShit = false;
                if(currentUnit.tempTerrainCreated.Count != 0)
                {
                    terrainShit = true;
          
                    Queue<TempTerrain> TQ = new Queue<TempTerrain>();
                    foreach (var item in currentUnit.tempTerrainCreated)
                    {
                        if(item.turnToDieOn == turn)
                        {TQ.Enqueue(item);}
                        
                    }
                    if(TQ.Count == 0)
                    {
                        terrainShit = false;
                        MapManager.inst.grid.UpdateGrid();
                    }
                    else
                    {
                        CamFollow.inst.target = currentUnit.slot. transform;
                        yield return new WaitForSeconds(.5f);
                        Loop();
                        void Loop()
                        {
                            
                            StartCoroutine(d());
                            IEnumerator d()
                            {
                                if(TQ.Count == 0)
                                {
                                    terrainShit = false;
                                    MapManager.inst.grid.UpdateGrid();
                                }
                                else
                                {
                                    TempTerrain t =  TQ.Dequeue();
                                    CamFollow.inst.Focus(t.transform,(()=>{}));
                                    BattleTicker.inst.Type("Removing Terrain");
                                    yield return new WaitForSeconds(.25f);
                                    t.Kill();
                                    yield return new WaitForSeconds(.5f);
                                    Loop();
                                }
                            }
                        }
                    }

                    while(terrainShit)
                    {yield return null;}
                    yield return new WaitForSeconds(.5f);
                }
         
                if(currentUnit.slot.cont.specialSlot != null)
                {
                   // CamFollow.inst.ForceFOV(20);
                    BattleTicker.inst.Type(currentUnit.slot.cont.specialSlot.slotContents.contentName);
                    CamFollow.inst.target = currentUnit.slot. transform;
                    bool willUnitDie = currentUnit.slot.cont.specialSlot.willUnitDie();
                    yield return new WaitForSeconds(1);
                    currentUnit.slot.cont.specialSlot.Tick();
                    yield return new WaitForSeconds(1);
                    if(willUnitDie){
                        UnitIteration();
                        yield break;
                    }
                   
                 

                }


                CamFollow.inst.ForceFOV(CamFollow.inst.baseFOV);
                if( currentUnit.stunned)
                {
                    BattleTicker.inst.Type(BattleManager.inst. TurnState());
                    CamFollow.inst.target = currentUnit.slot. transform;
                    yield return new WaitForSeconds(.5f);
                    BattleTicker.inst.Type("Stunned!");
                    currentUnit.RemoveStun();
                    StatusEffectLoop(currentUnit);
                    while(looping)
                    {yield return null;}
           
                    
                    yield return new WaitForSeconds(.75f);
                    UnitIteration();
                }
                else
                {
                    BattleTicker.inst.Type(BattleManager.inst. TurnState());

                    CamFollow.inst.target = currentUnit.slot. transform;
                    if(currentUnit.side == Side.PLAYER)
                    {       
                        CamFollow.inst.Focus(currentUnit.slot.transform,()=>{});
                        currentUnit.activeUnitIndicator.gameObject.SetActive(true);
                        yield return new WaitForSeconds(.25f);
                        StatusEffectLoop(currentUnit);
                        while(looping)
                        {yield return null;}
                    
                       
                        if(0 >= currentUnit.health.currentHealth)
                        {
                            Debug.Log("Unit Died from bleed");
                            UnitIteration();
                        }
                        else
                        {
                            if(currentUnit. sounds != null)
                            {AudioManager.inst.GetSoundEffect().Play(currentUnit.sounds.turnStart);}
                            
                            ActionMenu.inst.FUCKOFF = false;
                            GameManager.inst.ChangeGameState(GameState.PLAYERUI);
                            ActionMenu.inst.Reset();
                            ActionMenu.inst.Show(currentUnit.slot);
                            SkillHandler.inst.NewUnit(currentUnit);
                        }
                    }
                    else
                    {
                        SlotInfoDisplay.inst.Apply(currentUnit.slot);
                        GameManager.inst.ChangeGameState(GameState.ENEMYTURN);
                        yield return new WaitForSeconds(.25f);
                        if(    currentUnit.enemyAI != null){
                            
                            currentUnit.enemyAI.ConductTurn();
                        }
                        else{
                            Debug.LogAssertion("NO ENEMY AI FOUND");
                            UnitIteration();
                        }
                        
                        //
                        //AI
                    }
                }
                
            }
            else
            {
                NewTurn();
            }
        }
    }

    public bool playerLose()
    {return playerUnits.Count == 0;}

    public void Lose()
    {
        BattleTicker.inst.Type("All the adventurers have perished...");
        music.DOFade(0,1);
    }

    public void StatusEffectLoop(Unit u) //THIS IS BAAADDDD
    {
        looping = true;
        Queue<StatusEffectEnum> statusEffects = new Queue<StatusEffectEnum>();
        Queue<StatusEffect> holyfuckingshit = new Queue<StatusEffect>();
        List<StatusEffect> bin = new List<StatusEffect>();
        GenericDictionary<StatusEffectEnum, int> xd = new GenericDictionary<StatusEffectEnum,int>();
        var values = System.Enum.GetValues(typeof(StatusEffectEnum));
        foreach (StatusEffectEnum item in values)
        {xd.Add(item,0);}
        foreach (var catagory in u.statusEffects)
        {
            foreach (var statusEffect in catagory.Value)
            {
                if(BattleManager.inst.turn > statusEffect.turnToKill)
                {
                    if(!bin.Contains(statusEffect))
                    {bin.Add(statusEffect);}
                }
                else
                {
                    if(statusEffect.tick != null)
                    {
                        holyfuckingshit.Enqueue(statusEffect);
                        if(!statusEffects.Contains(statusEffect.statusEffectEnum))
                        {
                            statusEffects.Enqueue(statusEffect.statusEffectEnum);
                            xd[statusEffect.statusEffectEnum]++;
                        }
                        else
                        {xd[statusEffect.statusEffectEnum]++;}
                        
                    }
                }
            }
        }
        
        if(statusEffects.Count == 0)
        { 
            foreach (var catagory in u.statusEffects)
            {
                foreach (var statusEffect in catagory.Value)
                {
                    if(BattleManager.inst.turn == statusEffect.turnToKill)
                    {
                        if(!bin.Contains(statusEffect))
                        {bin.Add(statusEffect);}
                    }
                }
            }
            foreach (var item in bin)
            { item.Remove(); }
            looping = false;
        }
        else
        {
            Execute();
            void Execute()
            {
                StartCoroutine(delay());
                IEnumerator delay()
                {
                    if( 0 >= currentUnit.health.currentHealth)
                    {
                        looping = false;
                        yield break;
                    }

                   
                    if(statusEffects.Count > 0)
                    {
                        StatusEffectEnum se = statusEffects.Dequeue();
                        
                        switch (se)
                        {
                            case StatusEffectEnum.BARRIER:
                            Debug.LogAssertion("BARRIER SHOULD NOT TICK");
                            break;
                            case StatusEffectEnum.BLEED:
                            BattleTicker.inst.Type("Bleed");
                            currentUnit.Bleed(xd[StatusEffectEnum.BLEED]);
                            break;
                            case StatusEffectEnum.BILE:
                            Debug.LogAssertion("BILE SHOULD NOT TICK");
                            break;
                            default:
                            Debug.LogAssertion("DEFAULT CASE");
                            break;
                        }
                        yield return new WaitForSeconds(.75f);
                        Execute();
                    }
                    else
                    {
                        foreach (var item in bin)
                        { item.Remove(); }
                        for (int i = 0; i <  holyfuckingshit.Count; i++)
                        {
                            if(holyfuckingshit.Count > 0)
                            {
                                StatusEffect  se =  holyfuckingshit.Dequeue();
                                se.CheckForKill();
                            }
                        }
                      
                        looping = false;
                    }
                }
            }
        }
    }

 

    public void UnitIsDead(Unit u)
    {
        if(u.side == Side.ENEMY)
        {enemyUnits.Remove(u);}
        else
        {playerUnits.Remove(u);}
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
   
        playerUnits.Add(u);
        return u;
    }

    public Unit CreateEnemyUnit(Slot slot)
    {
        Enemy e =enemies[Random.Range(0,enemies.Count)];
        CharacterGraphic graphic =  CharacterBuilder.inst.GenerateEnemy(e);
        Unit u =  Instantiate(unitPrefab);
        u.sounds = e.sounds;
        u.enemy = e;
        u.character = graphic.character;
        graphic.unit = u;
        u.side = Side.ENEMY;
       
        u.character.baseStats = CharacterBuilder.inst.genStats(e.startingStats);
       // u.character.skills = new List<Skill>( statsAndSkills.skills);
        u.RecieveGraphic(graphic);
        u.health.Init(u.stats().hp);
        u.gameObject.name = graphic.character.characterName.fullName();
        ReposUnit(u,slot);
        u.enemyAI = Instantiate(e.enemyAI,u.transform);
     //   u.sounds = CharacterBuilder.inst.sfxDict[u.character.species];
        u.enemyAI.Init(u);
        enemyUnits.Add(u);
        return u;
    }

    public void ReposUnit(Unit u,Slot slot){
        u.transform.position = new Vector3(slot.transform.position.x,u.transform.position.y,slot.transform.position.z);
        u.Reposition(slot);
        slot.cont.unit = u;  
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
