using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Health : MonoBehaviour
{
    public int maxHealth;
    public int currentHealth;
    public UnityEvent onDie,onHit,onInit;
    public void Init(int max){
        maxHealth = max;
        currentHealth = maxHealth;
        onInit.Invoke(); 
    }

    public void Hit(int damage)
    {
        currentHealth -= damage;
        onHit.Invoke();  
        if(currentHealth <=0)
        {Die();}
    }
    

    public void Die()
    {
       onDie.Invoke();  
    }
}
