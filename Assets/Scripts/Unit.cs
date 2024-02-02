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
                if( transform.position.x != v.x)
                {
                    bool movingRight = transform.position.x <= v.x ;
                    if(movingRight)
                    {transform.localScale = Vector3.one; 
                    healthBar.transform.parent. localScale = new Vector3(1,1,1);}
                    else
                    { transform.localScale = new Vector3(-1,1,1); 
                    healthBar.transform.parent.localScale = new Vector3(-1,1,1);}
                }
             

                transform.DOMove(v,.1f).OnComplete(()=>
                { loop(); });
            }
            else
            { Reposition(finalSlot); }
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
        SlotSelector.inst.gameObject.SetActive(true);
        slot = newSlot;
     
        if(!stats(). passable)
        {slot.node.isBlocked = true;}
        else
        {slot.node.isBlocked = false;}
        slot.unit = this;
        graphic.ChangeSpriteSorting(slot.node.iGridY);
        BattleManager.inst.EndTurn();
    }

    
    public Stats stats()
    {return character.baseStats;}
}