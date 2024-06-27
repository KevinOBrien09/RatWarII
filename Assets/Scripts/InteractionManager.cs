using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionManager : Singleton<InteractionManager>
{
    public Interactable currentInteractable;
    public float distToInteractable;
    public float range;
    void Update()
    {
        if(currentInteractable != null)
        {
            distToInteractable = currentInteractable.GetDistanceToPlayer();
            bool inRange = distToInteractable < range;
            if(inRange)
            {
                currentInteractable.OutlineToggle(true,inRange);
                if(!BattleManager.inst.inBattle && !Menu.inst.open)
                {
                    if(InputManager.inst.player.GetButtonDown("Confirm"))
                    {
                        if(inRange){
                             currentInteractable.Go();
                        }
                       
                    }
                }
            }
            else
            {
               currentInteractable.OutlineToggle(true,inRange);
            }
        }
    }

    public void SetInteractable(Interactable newInteractable){
        currentInteractable = newInteractable;
    }

    public void RemoveInteractable(){
        currentInteractable.OutlineToggle(false);
        currentInteractable = null;
    }
}
