using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public enum Side{PLAYER,ENEMY}
public class Unit : MonoBehaviour
{
    public CharacterGraphic graphic;
    public Character character;
    public Slot slot;
    public SpriteRenderer spriteRenderer;
    public GameObject activeUnitIndicator;
    public Health health;
    public HealthBar healthBar;
    public Stats statMods;
    public Side side;
    public Enemy enemy;
    public List<StatusEffect> statusEffects = new List<StatusEffect>();
    public bool facingRight;
   public  ParticleSystemRenderer shieldGraphic,shieldKillRend;
   public ParticleSystem shieldKill;
   public bool movedThisTurn;
   public bool inKnockback;
   public bool stunned;
   public Material whiteFlash,spriteDefault;
   public GameObject stunIndicator;

    public void RecieveGraphic(CharacterGraphic _graphic)
    {
        graphic = _graphic;
        graphic.transform.parent = this.transform;
        if(_graphic.unit.side == Side.PLAYER){
    graphic.transform.localPosition = new Vector3(0,-2.25f,0);
        }
    
        graphic.transform.localRotation = Quaternion.Euler(0,0,0);
        character = graphic.character;
    }
    

    public void AddStatusEffect(StatusEffect se)
    {
        statusEffects.Add(se);
        se.add.Invoke();
    }

    public void RemoveStatusEffect(StatusEffect se)
    {
       
        if(statusEffects.Contains(se)){
            se.remove.Invoke();
            statusEffects.Remove(se);
        }
        else{
            Debug.LogWarning("Status Effect not found");
        }
      
    }
    public void MoveAlongPath(Queue<Slot> q,Slot finalSlot)
    {
        loop();
        void loop()
        {
            if(q.Count > 0)
            {
                Slot s= q.Dequeue();
                Vector3 v =  new Vector3(s.transform.position.x, transform.position.y ,s.transform.position.z);
                graphic.ChangeSpriteSorting(s.node.iGridY);
                Flip(v);
             

                transform.DOMove(v,.1f).OnComplete(()=>
                { loop(); });
            }
            else
            { 
                Reposition(finalSlot);
                MapManager.inst.grid.UpdateGrid();
                movedThisTurn = true;
                SlotInfoDisplay.inst.sl = finalSlot;
                GameManager.inst.ChangeGameState(GameState.PLAYERUI);
                ActionMenu.inst.Reset();
               
                ActionMenu.inst.Show(this.slot);

                //BattleManager.inst.EndTurn();
            }
        }
    }

    public void Stun(){
        stunned = true;
        graphic.WhiteFlash(()=>{

   stunIndicator.SetActive(true);
        });
     
    }

    public void RemoveStun(){
        stunned =  false;
           stunIndicator.SetActive(false);
    }

    public bool willUnitDie(int damage){
        int temp = health.currentHealth - damage;
        bool dead = temp <= 0;
         return !dead;
    }

    public void Hit(int damage)
    {   int i =health.dmgAmount(damage);
        ObjectPoolManager.inst.Get<BattleNumber>(ObjectPoolTag.BATTLENUMBER).Go(i.ToString(),Color.white,transform.position);
        int temp = health.currentHealth - damage;
        bool dead = temp <= 0;
        graphic.RedFlash(dead,(()=>
        {health.Hit(damage);}));
    }

    public void Heal(int amount){
        graphic.GreenFlash(()=>{
            int i =health.healAmount(amount);

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
        BattleManager.inst.UnitIsDead(this);
        slot.unit = null;
        health.currentHealth = 0;
        StartCoroutine(q());
        IEnumerator q()
        {
            yield return new WaitForSeconds(.4f);
            Destroy(gameObject);
            MapManager.inst.grid.UpdateGrid();
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
        if(slot != null)
        {
            slot.node.isBlocked = false;
            slot.unit = null;
        }
        SlotInfoDisplay.inst.Disable();
        //SlotSelector.inst.gameObject.SetActive(true);
        slot = newSlot;
     
        if(!stats(). passable)
        {slot.node.isBlocked = true;}
        else
        {slot.node.isBlocked = false;}
        slot.unit = this;
        graphic.ChangeSpriteSorting(slot.node.iGridY);
        
    }

    
    public Stats stats()
    {return character.baseStats;}
}