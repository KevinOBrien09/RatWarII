using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Health : MonoBehaviour
{
    [System.Serializable]
    public class qwerty{
        public   StatusEffect se;
        public  int value;
        public Unit unit;
        
    }
    public List<qwerty> shields = new List<qwerty>();
    public Dictionary<StatusEffect,qwerty> dict = new Dictionary<StatusEffect, qwerty>();
    public int maxHealth;
    public int currentHealth;
    public UnityEvent onDie,onHit,onInit,onShieldBreak,onRefresh;
    public void Init(int max){
        maxHealth = max;
        currentHealth = maxHealth;
        onInit.Invoke(); 
    }

    public void GainShield(StatusEffect se,int amount,Unit u)
    {
        qwerty q = new qwerty();
        q.se = se;
        q.value  = amount;
        q.unit = u;
        dict.Add(se,q);
        shields.Add(q);
        onRefresh.Invoke();
    }

    public void RemoveShield(StatusEffect se)
    {
        if(dict.ContainsKey(se))
        {
            if(shields.Contains(dict[se])){
                shields.Remove(dict[se]);
                dict.Remove(se);
                onRefresh.Invoke();

                if(shield() <= 0){
                     if(shield() == 0)
        {onShieldBreak.Invoke();}
                }
            }
            else{
                Debug.LogWarning("Shield not found in list");
            }
        
        }
        else{
            Debug.LogWarning("ShieldNotFoundInDictionary");
        }
      
    }

    public void DeductShield()
    {
        Queue<qwerty> q = new Queue<qwerty>();
        foreach (var item in shields)
        {q.Enqueue(item);}
        qwerty currentQWERT = q.Dequeue();
        if( currentQWERT.value > 0)
        {
            currentQWERT.value--;
        }
        if(currentQWERT.value <= 0){
            RemoveShield(currentQWERT.se);
            currentQWERT.unit.RemoveStatusEffect(currentQWERT.se);
        }
        if(shield() == 0)
        {onShieldBreak.Invoke();}
    }

    public void Heal(int amount)
    {
        int HealAmount = healAmount(amount);
        currentHealth += HealAmount;
        onRefresh.Invoke();
    }

    public int healAmount(int amount){
    return (int)Mathf.Min(maxHealth -  currentHealth, amount);
    }

    public int dmgAmount(int amount){
    return (int)Mathf.Min(maxHealth, amount);
    }

    public void Hit(int damage)
    {   
        for (int i = 0; i < damage; i++)
        {
             
            if(shield() > 0)
            {
                DeductShield();
                // shield--;
              
            }
            else
            {currentHealth--;}
        }
        onHit.Invoke();  
        onRefresh.Invoke();
        if(currentHealth <=0)
        {  onDie.Invoke();  }
    }

    public int shield()
    {
        int i = 0;
        foreach (var item in shields)
        {i += item.value;}
        return i;
    }
    

  
}
