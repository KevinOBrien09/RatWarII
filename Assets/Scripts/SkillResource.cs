using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class SkillResource : MonoBehaviour
{
    public UnityEvent onChange;
    public enum Catagory{NONE,MANA,STAMINA}
    public Catagory catagory;
    public int max;
    public int current;

    public void Init(int max,int current)
    {
        this.max = max;
        this.current = current;
        onChange.Invoke();
    }

    public bool canSpend()
    { return true; }

    public void Spend(int cost){
        current -=cost;
        if(onChange != null){
            onChange.Invoke();
        }
    }

   
    public void Regain(int amount)
    {
        int a = regainAmount(amount);
        current += a;
        onChange.Invoke();
    }

    public int regainAmount(int amount)
    {return (int)Mathf.Min(max -  current, amount);}

    public void SetCatagory(Job job)
    {
        switch (job)
        {
            case Job.KNIGHT:
            catagory = Catagory.STAMINA;
            break;
            case Job.WIZARD:
            catagory = Catagory.MANA;
            break;
            case Job.ARCHER:
            catagory = Catagory.STAMINA;
            break;
            default:
            Debug.LogWarning("CATAGORY FOR " + job.ToString()+" NOT SET!!");
            catagory = Catagory.STAMINA;
            break;
        }
    }
    public string abbrv()
    {
        switch (catagory)
        {
            case Catagory.NONE:
            return "ERR: ";
            case Catagory.MANA:
            return "MNA: ";
            case Catagory.STAMINA:
            return "STA: ";
            default:
            return "ERR: ";
        }
       

    }


}