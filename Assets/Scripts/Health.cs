using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Health : MonoBehaviour
{
    public int shield;
    public int maxHealth;
    public int currentHealth;
    public UnityEvent onDie,onHit,onInit,onShieldBreak;
    public void Init(int max){
        maxHealth = max;
        currentHealth = maxHealth;
        onInit.Invoke(); 
    }

    public void GainShield(int shieldAmount){
        shield = shieldAmount;
       
    }

    public void Hit(int damage)
    {   
        for (int i = 0; i < damage; i++)
        {
             
            if(shield > 0)
            {
                shield--;
                if(shield == 0)
                {onShieldBreak.Invoke();}
            }
            else
            {currentHealth--;}
        }
        onHit.Invoke();  
        if(currentHealth <=0)
        {  onDie.Invoke();  }
    }
    

  
}
