using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.Events;
using DG.Tweening;
using UnityEngine.SceneManagement;

public class BattleManager : Singleton<BattleManager>
{
    public List<Unit> playerUnits = new List<Unit>();
    public List<Unit> enemyUnits = new List<Unit>();
    public List<Unit> neutralUnits = new List<Unit>();
    public List<Unit> unitsIDontCareAboutInTurnReset = new List<Unit>();
    public Unit currentUnit;
    public int turn;
    public Queue<Unit> turnOrder = new Queue<Unit>();
    public bool roomLockDown;
    public bool looping;
    public GameObject questComplete;
    public SoundData roomUnlockSting;
    public string lossReason = "REASON UNCLEAR";
    public bool skipFadeIn;
    public bool hasAmbush,inAmbush;
    public int iterationsTilAmbush;
    public GameObject overworld;
    public bool inBattle;
    public SoundData enterBattleSlam;
    public List<GameObject> shitToKill = new List<GameObject>();
    public List<Ambush> ambushes = new List<Ambush>();
    public List<StatusEffect> statusEffectBin = new List<StatusEffect>();
    //public SoundData tokenResetSFX;
    protected override void Awake()
    {
        base.Awake();
        #if UNITY_EDITOR
        if(GameManager.inst. loadFromFile && PartyManager.inst.currentParty == ""){
            Debug.LogWarning("LOADING FROMs BATTLEMANAGER");
            GameManager.inst.Load();
        }
        #endif

    }
    
    public IEnumerator Start()
    {
        if(!skipFadeIn)
        {
            BlackFade.inst.fade.DOFade(1,0);
            yield return new WaitForSeconds(1);
        }
        MusicManager.inst.FadeToSilence(0);
        GameManager.inst.GameInit();
        MapManager.inst.map.Generate();
    }


   
 
    public void Begin()
    {
        if(inBattle){
            return;
        }
        inBattle = true;
        foreach (var item in MapManager.inst.map.startRoom.slots)
        { item.cont.unit = null; }
        foreach (var item in allUnits())
        {
            List<TempTerrain> tt = new List<TempTerrain>(item.tempTerrainCreated);
            foreach (var t in   tt)
            { t.Kill(); }
            item.tempTerrainCreated.Clear();
        }
        //ZoomCameraSwap.inst.Attach(CamFollow.inst.gameObject);
        ResetUnitPositions();
        SkillHandler.inst.Close();
        //ActionMenu.inst.Reset();
     
        MusicManager.inst.ChangeMusic(LocationManager.inst.locationTravelingTo.battleMusic);
        AudioManager.inst.GetSoundEffect().Play(enterBattleSlam);
        BlackFade.inst.WhiteFlash();
        OverworldCamera.inst.FOVChange(179,(()=>
        {
            AmbushHandler.inst.SpawnAmbush(ambushes[Random.Range(0,ambushes.Count)]);
           
            foreach (var item in playerUnits)
            { 
                item.gameObject.SetActive(true); 
                item.graphic.breathing.Reset();
                item.skillResource.Regain(item.skillResource.regen);
            }

            foreach (var item in PartyController.inst.playerUnits)
            { 
                item.gameObject.SetActive(false); 
                item.agent.enabled = false;
                 
            }
            ResetTurnOrder();
            BattleTicker.inst.parent.SetActive(true);
            overworld.SetActive(false);
            MapManager.inst.map.gameObject.SetActive(true);
            OverworldCamera.inst.gameObject.SetActive(false);
            CamFollow.inst.gameObject.SetActive(true);
        
            ToggleHealthBars(false);
            NewTurn();


        }));
       
    }

    public void NewTurn()
    {
        BattleTicker.inst.Type(BattleManager.inst. TurnState());
        unitsIDontCareAboutInTurnReset.Clear();
      
        ResetTurnOrder();
        turn++;
        UnitIteration();
    }

    public void ResetUnitPositions()
    {
        Dictionary<Vector2,Slot>  d =    MapManager.inst.map.GetPlayerStartingSlots();
        foreach (var item in  playerUnits)
        {
            Vector2 v = PartyManager.inst.parties[PartyManager.inst.currentParty].members[item.character.ID].battlePosition;
            
            Unit u = item;
         
            u.slot = null;
            Slot s = d[v];
            u.transform.position =  new Vector3(s.transform.position.x,u.transform.position.y ,s.transform.position.z);
            u.Reposition(s);
        }
    }


    public void ResetTurnOrder()
    {
        turnOrder.Clear();
        List<Unit> u = new List<Unit>(allUnits());
        List<Unit> ordered = u.OrderBy(f => f.stats().speed).ToList();
        ordered.Reverse();
        
        foreach (var item in unitsIDontCareAboutInTurnReset)
        {
            if(ordered.Contains(item))
            {ordered.Remove(item);}
        }
        
        foreach (var item in ordered)
        {
            if(!turnOrder.Contains(item))
            {
                turnOrder.Enqueue(item);
                item.battleTokens = new BattleTokens(item.baselineTokens);
                item.skillResource.Regain(item.skillResource.regen);
            }
            else{
                Debug.Log("double up");
            }
        }
    }

 
 

    public void EndTurn(bool wasSkipped = false)
    {
        if(wasSkipped|currentUnit.stunned){
            ActuallyEnd();
            return;
        }

        if(currentUnit != null) 
        {
           
            if(currentUnit.side == Side.PLAYER)
            {
                //currentUnit.DeductMoveToken();
                if(UnitHasMoveTokens())
                {
                    
                  
                    GameManager.inst.ChangeGameState(GameState.PLAYERUI);
                    ActionMenu.inst.Reset();
                    ActionMenu.inst.Show(currentUnit.slot);
                }
                else{
                    ActuallyEnd();
                }
            }
            else{
                ActuallyEnd();
            }
        }
        else{
            ActuallyEnd();
        }
    
        void ActuallyEnd()
        {
            
            if(hasAmbush){
                 iterationsTilAmbush--;
            }
       
                // if(wasSkipped &&!roomLockDown){
                //     currentUnit = turnOrder.Dequeue();
                // }
            
            
            if(BattleManager.inst.turn != 0)
            {BattleManager.inst.UnitIteration();}
            MapManager.inst.map.UpdateGrid();
            ActionMenu.inst.Hide();
        }
       
    }
    
    public void UnitIteration()
    {
        if(playerLose())
        {Lose();}
        else
        {
            if(!CheckForWin())
            {StartCoroutine(q());}
            else
            { Win(); }
        }
        
     
        IEnumerator q()
        {
            SkillAimer.inst.validSlots.Clear();
            UnitMover.inst.validSlots.Clear();
            if(turnOrder.Count > 0)
            { 
                ActionMenu.inst.FUCKOFF = false;
                
                yield return new WaitForSeconds(.25f);
                currentUnit = turnOrder.Dequeue();
                
                foreach (var item in allUnits())
                {
                    item.activeUnitIndicator.gameObject.SetActive(false);
                }
                SlotInfoDisplay.inst.doGodTokenShine = false;
                currentUnit.activeUnitIndicator.gameObject.SetActive(true);
               
                unitsIDontCareAboutInTurnReset.Add(currentUnit);
                // if(!MapManager.inst.doNotSpawnEnemies)
                // {
                //     if(checkForAmbush())
                //     {
                //         iterationsTilAmbush = Random.Range(5,10);
                //         yield return new WaitForSeconds(1);
                //         ResetTurnOrder();
                //     }
                // }
               
                
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
                       MapManager.inst.map.UpdateGrid();
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
                                    MapManager.inst.map.UpdateGrid();
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
                    if(currentUnit.slot.cont.specialSlot.sotTrigger){
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
              
                }
                CamFollow.inst.ForceFOV(CamFollow.inst.baseFOV);
                MapManager.inst.CheckForIntrusions();
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
                        CamFollow.inst.Focus(currentUnit.transform,()=>{});
                        ActionMenu.inst.ResetMoveOption();
                    
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
                        if(currentUnit.charAI != null){
                            
                            currentUnit.charAI.ConductTurn();
                        }
                        else{
                            Debug.LogAssertion("NO ENEMY AI FOUND");
                            UnitIteration();
                        }
                    }
                }
            }
            else //end turn
            {
                foreach (var item in statusEffectBin)
                { 
                    if(item != null)
                    {
                        if(item.unit != null)
                        { item.Remove(); }
                    }
                }
                statusEffectBin.Clear();
                NewTurn();
            }
        }
    }
    
    public void StatusEffectLoop(Unit u)
    {
        looping = true;
        List<StatusEffect> dotBin = new List<StatusEffect>();
        Dictionary<StatusEffectEnum,int> dotStatusEffects = new Dictionary<StatusEffectEnum, int>();
        Queue<StatusEffectEnum> q = new Queue<StatusEffectEnum>();
        foreach (var catagory in u.statusEffects)
        {
            StatusEffectEnum e = catagory.Key;
          
            foreach (var effect in catagory.Value)
            {
                if(effect.statusEffectEnum == StatusEffectEnum.BLEED) //if status effect triggers on turn start add it to the q
                {
                    if(!dotStatusEffects.ContainsKey(StatusEffectEnum.BLEED))
                    {
                        dotStatusEffects.Add(StatusEffectEnum.BLEED,1);
                        q.Enqueue(StatusEffectEnum.BLEED);
                        
                    }
                    else
                    {dotStatusEffects[StatusEffectEnum.BLEED]++;}
                   
                }
                if(effect.TurnsLeft() == 0)
                {
                    if(effect.statusEffectEnum == StatusEffectEnum.BLEED){
                        dotBin.Add(effect);
                    }
                    else{
                        statusEffectBin.Add(effect);
                    }
                    
                }
            }
        }
        StartCoroutine(DOTLoop());
        
        IEnumerator DOTLoop()
        {
            if(q.Count > 0)
            {
                yield return new WaitForSeconds(.5f);
                StatusEffectEnum se = q.Dequeue();
                
                switch (se)
                {
                    case StatusEffectEnum.BARRIER:
                    Debug.LogAssertion("BARRIER SHOULD NOT TICK");
                    break;
                    case StatusEffectEnum.BLEED:
                    BattleTicker.inst.Type("Bleed");
                    currentUnit.Bleed(dotStatusEffects[StatusEffectEnum.BLEED]);
                    break;
                    case StatusEffectEnum.BILE:
                    Debug.LogAssertion("BILE SHOULD NOT TICK");
                    break;
                    default:
                    Debug.LogAssertion("DEFAULT CASE");
                    break;
                }
             
                yield return new WaitForSeconds(.5f);
                StartCoroutine(DOTLoop());
            }
            else
            {
                foreach (var item in dotBin)
                {
                    if(item != null)
                    {
                        if(item.unit != null)
                        {
                            
                            item.Remove();
                        }
                    }
                }
                looping = false;
            }
        }
    }
    
    public void LeaveBattle()
    {
        MusicManager.inst.ChangeMusic(LocationManager.inst.locationTravelingTo.locationMusic);
        AudioManager.inst.GetSoundEffect().Play(enterBattleSlam);
        ActionMenu.inst.Reset();
        Cursor.lockState =   CursorLockMode.Confined;
        BlackFade.inst.WhiteFlash();
        MapManager.inst.map.gameObject.SetActive(false); 
        CamFollow.inst.gameObject.SetActive(false);
      
        OverworldCamera.inst.gameObject.SetActive(true);
        //ZoomCameraSwap.inst.Attach(OverworldCamera.inst.gameObject);
        OverworldCamera.inst.FOVChange(OverworldCamera.inst.baseFOV,(()=>{ PartyController.inst. TakeControl();   inBattle = false;   CamFollow.inst.transform.position = Vector3.zero;}));
        foreach (var item in playerUnits)
        { 
            item.gameObject.SetActive(false); 
            item.RemoveStun();
            foreach (var statuEffectCollection in item.statusEffects)
            {
                List<StatusEffect> bin = new List<StatusEffect>();
                if(statuEffectCollection.Key == StatusEffectEnum.BARRIER){
                    foreach (var statusEffect in statuEffectCollection.Value)
                    {
                        bin.Add(statusEffect);
                       
                    }
                    foreach (var trash in bin)
                    {
                        trash.Remove();
                    }
                }
                
            }
        
        
        }
        ResetUnitPositions();
        enemyUnits.Clear();
        foreach (var item in PartyController.inst.playerUnits)
        { 
            item.gameObject.SetActive(true); 
            item.agent.enabled = true;
            item.graphic.breathing.Reset();

           
        }
        
        foreach (var item in ObjectPoolManager.inst.dict[ObjectPoolTag.CORPSE])
        {
            foreach (var q in ObjectPoolManager.inst.dict[ObjectPoolTag.CORPSE])
            { q.gameObject.SetActive(false); }
           
        }
        List<GameObject> g = new List<GameObject>(shitToKill);
        foreach (var item in g)
        {Destroy(item); }
        shitToKill.Clear();
        BattleTicker.inst.parent.SetActive(false);
        overworld.SetActive(true);
        ToggleHealthBars(false);
        turn = 0;
    }
    
    public void Win()
    {
        BattleTicker.inst.Type("Win");
        Cursor.lockState =   CursorLockMode.Confined;
        StartCoroutine(q());
        IEnumerator q(){
            MusicManager.inst.FadeToSilence(.5f);
            yield return new WaitForSeconds(1);
            BattleEndPopUp.inst.Show();
        }
    }
    
    public void Lose()
    {
        BattleTicker.inst.Type(lossReason);
        MusicManager.inst.source.DOFade(0,1);
        LocationManager.inst.inTravel = false;
        BlackFade.inst.FadeInEvent(()=>
        {
            StartCoroutine(q());
            IEnumerator q()
            {
                yield return new WaitForSeconds(1);
                SceneManager.LoadScene("Hub");
            }
          
        });
       
    }
    
    public bool playerLose()
    {
        bool b = playerUnits.Count == 0;
        if(b)
        {
            lossReason = "The adventurers have perished...";
        }
     
        return b;
    }

    public bool CheckForWin()
    {return enemyUnits.Count == 0;}


    public void AddNewUnit(Unit u,Side side)
    {
        if(side == Side.ENEMY)
        {enemyUnits.Add(u);}
        else
        {playerUnits.Add(u);}
        u.side = side;
    }


    public bool UnitHasMoveTokens()
    {
        if(currentUnit != null) 
        {
            if(currentUnit.battleTokens.total() != 0){
                return true;
            }
            else{
                return false;
            }
        
        }


        return false;
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
