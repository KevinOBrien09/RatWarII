using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class SkillResource : MonoBehaviour
{
    public UnityEvent onChange;
    public enum Catagory{NONE,MANA,STAMINA,PUSS}
    public Catagory catagory;
    public int max;
    public int current;
    public int regen;
    public void Init(int max,int current)
    {
        this.max = max;
        this.current = current;
        onChange.Invoke();
    }

    public bool canSpend(float cost)
    { 
       return current >= cost;
    }

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
            regen = 1;
            break;
            case Job.WIZARD:
            catagory = Catagory.MANA;
            regen = 0;
            break;
            case Job.ARCHER:
            catagory = Catagory.STAMINA;
            regen = 1;
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
            case Catagory.PUSS:
            return "PUS: ";
            default:
            return "ERR: ";
        }
    }

    public int Convert(Catagory other,int value){
        switch (other)
        {
            case Catagory.NONE:
            return value;
            case Catagory.MANA:
            if(catagory == Catagory.STAMINA){
                return value/10;
            }
            else if(catagory == Catagory.MANA){
                return value;
            }
            return value;
            case Catagory.STAMINA:
            if(catagory == Catagory.STAMINA){
                return value;
            }
            else if(catagory == Catagory.MANA){
                 return value*10;
            }
            return value;
            case Catagory.PUSS:
            return value;
            default:
            return value;
        }
    }


}