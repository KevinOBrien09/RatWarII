using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.Events;
public enum Side{PLAYER,ENEMY,NEITHER,BOTH}
public class Unit : MonoBehaviour
{
    
    public CharacterGraphic graphic;
    public Character character;
    public Slot slot;
    public SpriteRenderer spriteRenderer;
    public ParticleSystemRenderer activeUnitIndicator;
    public Health health;
    public SkillResource skillResource;
    public HealthBar healthBar;
    public Stats statMods;
    public Side side;
    public DefinedCharacter enemy;
    public GenericDictionary<StatusEffectEnum, List< StatusEffect>> statusEffects = new GenericDictionary<StatusEffectEnum, List< StatusEffect>>();
    public bool facingRight;
    public  ParticleSystemRenderer shieldGraphic,shieldKillRend;
    public ParticleSystem shieldKill,bleedVFX;
    public List<SoundData> systemSounds = new List<SoundData>();
    //public bool movedThisTurn;
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
    public float moveSpeed = .1f;
    public int baseLineMoveTokens,currentMoveTokens;
    public SpriteRenderer minimapIcon;

    void Start()
    {
        baseLineMoveTokens = 2;
        currentMoveTokens = baseLineMoveTokens;
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
        //
        StartCoroutine(Q());

        IEnumerator Q(){
            yield   return new WaitForSeconds(1);
            if(side == Side.PLAYER){
                Texture2D tex= IconGraphicHolder.inst.dict[character.ID];

                minimapIcon.sprite = Sprite.Create(tex, new Rect(0.0f, 0.0f, tex.width, tex.height), new Vector2(0.5f, 0.5f), 100.0f);
            }
            else{
                 minimapIcon.sprite =  Sprite.Create(enemy.icon, new Rect(0.0f, 0.0f, enemy.icon.width, enemy.icon.height), new Vector2(0.5f, 0.5f), 100.0f);
            }
           
        }
      
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
    public void MoveAlongPath(Queue<Slot> q,Slot finalSlot,UnityAction end = null)
    {    
        if(currentMoveTokens == baseLineMoveTokens) 
        {
            if(sounds != null)
            {AudioManager.inst.GetSoundEffect().Play(sounds.move);}
        }
        loop();
        moving = true;
        void loop()
        {
            if(q.Count > 0)
            {

                Slot s= q.Dequeue();
                Vector3 v =  new Vector3(s.transform.position.x, transform.position.y ,s.transform.position.z);
                if(isEntity()){
   graphic.ChangeSpriteSorting(s.node);
                
                }
                Flip(v);
             

                transform.DOMove(v,moveSpeed).OnComplete(()=>
                { 
                    AudioManager.inst.GetSoundEffect().Play(footstep);
                    loop(); 
                });
            }
            else
            { 
                moving = false;
                Reposition(finalSlot);
                if(end != null){
                    end.Invoke();
                }
                  if(isEntity()){
                MapManager.inst.map.UpdateGrid();
                // if(MapManager.inst.mapQuirk == MapQuirk.ROOMS)
                // {
                //     if(BattleManager.inst.roomLockDown)
                //     {
                //         movedThisTurn = true;
                //     }
                // }
                // else
                // {
                //     movedThisTurn = true;
                // }
           
                SlotInfoDisplay.inst.sl = finalSlot;
            
                if(side == Side.PLAYER)
                { 
                   
                    if(isHostage)
                    {
                        if(!ObjectiveManager.inst.CheckIfComplete())
                        {
                            
                            BattleManager.inst.EndTurn(); 
                            
                        }
                        else{
                            BattleManager.inst.Win();
                        }
                    }
                    else{
                    BattleManager.inst.EndTurn();
                    }
                   
                       
                              
                        
                        
                    
                   
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

    public void DeductMoveToken(){
        currentMoveTokens--;
    }


    public virtual void Hit(int damage,CastArgs castArgs, bool bleed = false)
    {   
        int i = health.dmgAmount(damage);
        i = PostMitDamage(i);
        ObjectPoolManager.inst.Get<BattleNumber>(ObjectPoolTag.BATTLENUMBER).Go(i.ToString(),Color.white,transform.position);
        Vector3 v = new Vector3();
        bool ca = castArgs != null;
        if(ca){
           v = castArgs.caster.transform.position;
         
        }

        CastArgs c= castArgs;
        Debug.Log(c.caster.name);
        graphic.RedFlash(()=>
        {
            if(ca)
            {Flip(v);}
           int d = PostMitDamage(damage);
            health.Hit(d,c);
            
        if(bleed)
        {
            bleedVFX.gameObject.SetActive(true);
            bleedVFX.Play();
            AudioManager.inst.GetSoundEffect().Play(systemSounds[3]);
        }
        AudioManager.inst.GetSoundEffect().Play(systemSounds[2]);});
    }

    public int PostMitDamage(int premit)
    {
        int mod = MiscFunctions.GetPercentage(premit,stats().defence);
        return premit -= mod;
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

    public virtual void Die()
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


    public virtual void Flip(Vector3 v)
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
        if(isEntity()){
            graphic.ChangeSpriteSorting(slot.node);
        }
        if(newSlot.isBoat){
            transform.SetParent(newSlot.transform);
            Debug.Log("BoatSlot");
        }
       
        
    }

    
    public Stats stats()
    {
        return character.baseStats.Stack(statMods);
    }

    public virtual bool isEntity(){
        return true;
    }

    public virtual bool CheckType<T>()
    {
        return this.GetType() == typeof(T);
    }
}