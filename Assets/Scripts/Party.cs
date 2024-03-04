using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Party : Singleton<Party>
{
    public int gold;
    public List<Character> activeParty = new List<Character>();
    public List<Character> benched = new List<Character>();
    bool startingGold;
    void Start(){
        if(!startingGold){
            gold = 900;
            startingGold = true;
        }
        Refresh();
      
    }

    public void AddGold(int i){
        gold += i;
       Refresh();
        
    }

    void Refresh()
    {
        if(GoldText.inst != null)
        { GoldText.inst.Refresh(); }
    }

    public void RemoveGold(int i){
        gold -= i;
        Refresh();
    }

    public bool canAfford(int i)
    {return gold >= i;}

    public void KillMember(Character c)
    {
        if(activeParty.Contains(c))
        {activeParty.Remove(c);}
    }

    public void AddToPossession(Character c)
    {
        if(activeParty.Count < 3)
        {activeParty.Add(c);}
        else
        {benched.Add(c);}
    }

  
}