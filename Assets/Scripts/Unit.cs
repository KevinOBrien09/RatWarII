using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public enum Side{PLAYER,ENEMY,NEITHER,BOTH}
public class Unit : MonoBehaviour
{
    
    public CharacterGraphic graphic;
    public Character character;
    public Slot slot;
    public SpriteRenderer spriteRenderer;
    public ParticleSystemRenderer activeUnitIndicator;
    public Health health;
    public HealthBar healthBar;
    public Stats statMods;
    public Side side;
    public DefinedCharacter enemy;
    public GenericDictionary<StatusEffectEnum, List< StatusEffect>> statusEffects = new GenericDictionary<StatusEffectEnum, List< StatusEffect>>();
    public bool facingRight;
    public  ParticleSystemRenderer shieldGraphic,shieldKillRend;
    public ParticleSystem shieldKill,bleedVFX;
    public List<SoundData> systemSounds = new List<SoundData>();
    public bool movedThisTurn;
    public bool inKnockback;
    public bool stunned;
    public Material whiteFlash,spriteDefault;
    public GameObject stunIndicator;
    public List<TempTerrain> tempTerrainCreated = new List<TempTerrain>();
    public CharacterAI charAI;
    public bool moving;
    public CharacterSounds sounds;
    public Corpse corpsePrefab;
    public bool dead;
    public SoundData footstep;
    public bool isHostage;
    void Start()
    {
        var values = System.Enum.GetValues(typeof(StatusEffectEnum));
        foreach (StatusEffectEnum item in values)
        {statusEffects.Add(item,new List<StatusEffect>());}
    }

    public void RecieveGraphic(CharacterGraphic _graphic)
    {
        graphic = _graphic;
        graphic.transform.parent = this.transform;
        if(_graphic.unit.side == Side.PLAYER){
        graphic.transform.localPosition = new Vector3(0,-2.25f,0);
        }
    
        graphic.transform.localRotation = Quaternion.Euler(0,0,0);
        character = graphic.character;
        healthBar.gameObject.transform.parent.gameObject.SetActive(false);
    }
    

    public void AddStatusEffect(StatusEffect se)
    {
       statusEffects[se.statusEffectEnum].Add(se);
        se.add.Invoke();
    }

    public void RemoveStatusEffect(StatusEffect se)
    {
       

        if(statusEffects[se.statusEffectEnum].Contains(se))
        {
            se.remove.Invoke();
            statusEffects[se.statusEffectEnum].Remove(se);
        }
        else{
            Debug.LogWarning("Status Effect not found");
        }
      
    }
    public void MoveAlongPath(Queue<Slot> q,Slot finalSlot)
    {
        if(sounds != null)
        {AudioManager.inst.GetSoundEffect().Play(sounds.move);}
        loop();
        moving = true;
        void loop()
        {
            if(q.Count > 0)
            {

                Slot s= q.Dequeue();
                Vector3 v =  new Vector3(s.transform.position.x, transform.position.y ,s.transform.position.z);
                graphic.ChangeSpriteSorting(s.node);
                Flip(v);
             

                transform.DOMove(v,.1f).OnComplete(()=>
                { 
                    AudioManager.inst.GetSoundEffect().Play(footstep);
                    loop(); 
                });
            }
            else
            { 
                moving = false;
                Reposition(finalSlot);
                MapManager.inst.map.UpdateGrid();
                if(MapManager.inst.mapQuirk == MapQuirk.ROOMS){
                    if(BattleManager.inst.roomLockDown)
                    {
                        movedThisTurn = true;
                    }
                }
                else{
                    movedThisTurn = true;
                }
           
                SlotInfoDisplay.inst.sl = finalSlot;
                if(side == Side.PLAYER)
                { 
                    if(MapManager.inst.mapQuirk == MapQuirk.ROOMS){
                        if(BattleManager.inst.roomLockDown){
                        ActionMenu.inst.RemoveMoveOption();
                        }
                    }
                    else{
                           ActionMenu.inst.RemoveMoveOption();
                    }
                    if(isHostage)
                    {
                        if(!ObjectiveManager.inst.CheckIfComplete())
                        {
                            GameManager.inst.ChangeGameState(GameState.PLAYERUI);
                            ActionMenu.inst.Reset();
                            ActionMenu.inst.Show(this.slot);
                        }
                        else{
                            BattleManager.inst.Win();
                        }
                    }
                    else
                    {
                        GameManager.inst.ChangeGameState(GameState.PLAYERUI);
                        ActionMenu.inst.Reset();
                        ActionMenu.inst.Show(this.slot);
                    }
                   
                }
                

                //BattleManager.inst.EndTurn();
            }
        }
    }

    public void Stun(){
        stunned = true;
        graphic.WhiteFlash(()=>{
        AudioManager.inst.GetSoundEffect().Play(systemSounds[0]);
   stunIndicator.SetActive(true);
        });
     
    }

    public void RemoveStun(){
        stunned =  false;
        stunIndicator.SetActive(false);
    }


    public void Hit(int damage,CastArgs castArgs, bool bleed = false)
    {   
        int i = health.dmgAmount(damage);
        ObjectPoolManager.inst.Get<BattleNumber>(ObjectPoolTag.BATTLENUMBER).Go(i.ToString(),Color.white,transform.position);
        Vector3 v = new Vector3();
        bool ca = castArgs != null;
        if(ca){
           v = castArgs.caster.transform.position;
        }
        
        graphic.RedFlash(()=>
        {
            if(ca)
            {Flip(v);}
           
            health.Hit(damage,castArgs);
            
        if(bleed)
        {
            bleedVFX.gameObject.SetActive(true);
            bleedVFX.Play();
            AudioManager.inst.GetSoundEffect().Play(systemSounds[3]);
        }
        AudioManager.inst.GetSoundEffect().Play(systemSounds[2]);});
    }

    public void RemoveStatusEffect(StatusEffectEnum statusEffect){
        statusEffects[statusEffect].Clear();
    }

    public int BleedDamage(int howManyBleeds){
float percent = (5f / 100f) * (float) health.maxHealth;
        int bleed = 0;
        for (int i = 0; i < howManyBleeds; i++)
        {bleed += (int)percent;}
        return bleed;
    }

    public void Bleed(int howManyBleeds)
    {
        
        int bleed = BleedDamage(howManyBleeds);
        Hit(bleed,null, true);
    }

    public void Heal(int amount){
        graphic.GreenFlash(()=>
        {
            int i =health.healAmount(amount);
            AudioManager.inst.GetSoundEffect().Play(systemSounds[1]);
            ObjectPoolManager.inst.Get<BattleNumber>(ObjectPoolTag.BATTLENUMBER).Go(i.ToString(),Color.green,transform.position);
            health.Heal(amount);
        });
    }

    public void ShieldBreak(){
        if(shieldGraphic != null){
 Destroy(shieldGraphic.gameObject);
        shieldKill.gameObject.SetActive(true);
        shieldKill.Play();
        shieldGraphic = null;
        }
       
       
    }

    public void Die()
    {  
        if(!dead)
        {
            if(isHostage)
            {
                BattleManager.inst.lossReason = "The hostage has died.";
                BattleManager.inst.gameOver = true;
            }
            if(side == Side.ENEMY){
                if(ObjectiveManager.inst.objective.objectiveEnum == Objective.ObjectiveEnum.CLEARAREA){
                    ObjectiveManager.inst.CheckIfComplete();
                    int i =BattleManager.inst.enemyUnits.Count-1 ;
                    ObjectiveProgressIndicator.inst.Show("Quest Progress:<br>" + i + " Left!" );
                }
            }
            else if(side == Side.PLAYER)
            {
                if(PartyManager .inst.currentParty != string.Empty){
      PartyManager .inst.parties[PartyManager .inst.currentParty]. KillMember(character);
                }
                else {
                    Debug.LogWarning("Character died in debug mode, hence there is no save to remove them from. This warning should not happen in build.");
                }
          
            }
            dead = true;
            BattleManager.inst.UnitIsDead(this);
            Corpse c =  ObjectPoolManager.inst.Get<Corpse>(ObjectPoolTag.CORPSE);
             //Instantiate(corpsePrefab,new Vector3(slot. transform.position.x,0,slot. transform.position.z) ,transform.rotation);
            c.Spawn(this,this.slot);
            if(sounds != null)
            {AudioManager.inst.GetSoundEffect().Play(sounds.die);}
            slot.cont.unit = null;
            health.currentHealth = 0;
            StartCoroutine(q());
            IEnumerator q()
            {
                yield return new WaitForSeconds(.4f);
                Destroy(gameObject);
                MapManager.inst.map.UpdateGrid();
            }
        }
    }


    public void Flip(Vector3 v)
    {
        if( transform.position.x != v.x)
        {
            bool movingRight = transform.position.x <= v.x ;
            if(movingRight)
            {transform.localScale = Vector3.one; 
            healthBar.transform.parent. localScale = new Vector3(1,1,1);
            facingRight = true;}
            else
            { transform.localScale = new Vector3(-1,1,1); 
            healthBar.transform.parent.localScale = new Vector3(-1,1,1);
            facingRight = false;}
        }
    }

    

    
    public void Reposition(Slot newSlot)
    {
        Slot oldSlot = slot;
       
        if(oldSlot != null)
        {slot.cont.unit = null;
        }
        if(side == Side.PLAYER )
        {SlotInfoDisplay.inst.Disable();}
        
        slot = newSlot;
    
        slot.cont.unit = this;
                MapManager.inst.CheckForIntrusions();
        graphic.ChangeSpriteSorting(slot.node);
        
    }

    
    public Stats stats()
    {return character.baseStats;}
}