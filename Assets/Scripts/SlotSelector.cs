using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
public class SlotSelector : Singleton<SlotSelector>
{
    public SpriteRenderer mRenderer;
    public Color32 selectedColour;
    Slot currentSlot;
    public IEnumerator Start(){
        yield return new WaitForEndOfFrame();
    
        ChangeColour(selectedColour);
        Attach(MapManager.inst.grid.NodeArray[0,0].slot);
    }

    public void ChangeColour(Color32 color)
    { mRenderer.color = color; }

    public void Attach(Slot s)
    {
        if(GameManager.inst.checkGameState(GameState.UNITMOVE))
        {return;}
        if(GameManager.inst.checkGameState(GameState.INTERACT))
        {
            if(InteractHandler.inst.slots.Contains(s))
            {
                if(s.unit != null)
                {
                    if(s.unit.side == Side.ENEMY)
                    {
                    ChangeColour(Color.red); 
                    }
                    else{
                    ChangeColour(Color.cyan); 
                    }
                
                }
                else{
                    ChangeColour(Color.white);
                    
                }
                    transform.position = s.border.transform.position;
               
            }
             return;
        }
        

       if(SkillAimer.inst.aiming)
        {
            if(!SkillAimer.inst.validSlots.Contains(s)){
                return;
            }
            else{
                if(s.unit != null){
                    if(s.unit.side == Side.ENEMY){
                    ChangeColour(Color.red); 
                    }
                    else{
                    ChangeColour(Color.cyan); 
                    }
              
                }
                else{
                      ChangeColour(Color.white);
                   
                }
            }

            
            transform.position = s.border.transform.position;
            return;
        }

      

     
        if(GameManager.inst.checkGameState(GameState.PLAYERSELECT) && !UnitMover.inst.inCoro)
        { 
            if(s.unit == null)
            {ChangeColour(Color.white);}
        }
        else
        {ChangeColour(selectedColour);}
        
     
        transform.position = s.border.transform.position;
        currentSlot = s;
    }

}