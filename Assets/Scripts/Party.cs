using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.Events;

[System.Serializable]
public class CharacterHolder{
    public Character character;
    public int currentHP;
    public int position;
}

[System.Serializable]
public class HolderSaveData{
    public CharacterSaveData charSave;
    public int currentHP;
    public int position;
}
[System.Serializable]
public class PartySaveData
{
    
  
    public List<HolderSaveData> activeParty = new List<HolderSaveData>();
    public List<HolderSaveData> benched = new List<HolderSaveData>();
    public List<HolderSaveData> deceased = new List<HolderSaveData>();

}
public class Party : Singleton<Party>
{
    public int gold;
    public GenericDictionary<string, CharacterHolder> activeParty = new GenericDictionary<string, CharacterHolder>();
    public GenericDictionary<string, CharacterHolder> benched = new GenericDictionary<string, CharacterHolder>();
    public GenericDictionary<string, Character> deadCharacters = new GenericDictionary<string, Character>();
    public UnityEvent onPartyEdit;
    public int partySize = 3;
    public int benchSize = 3;
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
        if(activeParty.ContainsKey(c.ID))
        {activeParty.Remove(c.ID);}

        if(!deadCharacters.ContainsKey(c.ID))
        {deadCharacters.Add(c.ID,c);}

        SaveLoad.Save(GameManager.inst.saveSlotIndex);
        
        onPartyEdit.Invoke();
    }

    public void AddToPossession(Character c)
    {
        if(!activeParty.ContainsKey(c.ID) && !benched.ContainsKey(c.ID))
        {
            if(activeParty.Count < partySize)
            {
                CharacterHolder holder = new CharacterHolder();
                holder.character = c;   activeParty.Add(c.ID,holder);
                holder.position = lastOpenPosition();
            }
            else
            {   CharacterHolder holder = new CharacterHolder();
                holder.character = c;
                holder.position = -5;
                benched.Add(c.ID,holder);
            }
            onPartyEdit.Invoke();
        }
      
    }

    public void PartyToBench(Character c)
    {
        if(activeParty.ContainsKey(c.ID) )
        {
            CharacterHolder holder = new CharacterHolder();
            holder.character = c;
            holder.position = -5;
            benched.Add(c.ID,holder);
            activeParty.Remove(c.ID);
            onPartyEdit.Invoke();
        }
    }

    public void BenchToParty(Character c,int i)
    {
        if(benched.ContainsKey(c.ID) )
        {
            CharacterHolder holder = new CharacterHolder();
            holder.character = c;
            holder.position = i;
            activeParty.Add(c.ID,holder);
            benched.Remove(c.ID);
            onPartyEdit.Invoke();
        }
    }

    public void UpdatePosition(Character c, int i)
    {activeParty[c.ID].position = i;     onPartyEdit.Invoke();}


    public int lastOpenPosition()
    {
        List<CharacterHolder> h = new List<CharacterHolder>();
        foreach (var item in activeParty)
        {h.Add(item.Value);}

        for (int i = 0; i < partySize; i++)
        {
            if(h.ElementAtOrDefault(i) != null)
            {
                if(h[i].position != i)
                {return i;}
            }
        }
        return 0;
    }

    public PartySaveData Save()
    {
        PartySaveData psd = new PartySaveData();
        foreach (var item in activeParty)
        {
            HolderSaveData hsd = new HolderSaveData();
            hsd.position = item.Value.position;
            hsd.charSave = item.Value.character.Save();
            psd.activeParty.Add(hsd);
        }
        foreach (var item in benched)
        {
            HolderSaveData hsd = new HolderSaveData();
            hsd.position = item.Value.position;
            hsd.charSave = item.Value.character.Save();
            psd.benched.Add(hsd);
        }
        foreach (var item in deadCharacters)
        {
            HolderSaveData hsd = new HolderSaveData();
            hsd.position = -10;
            hsd.charSave = item.Value.Save();
            psd.deceased.Add(hsd);
        }
        return psd;
    }

    public void Load(PartySaveData psd )
    {
        foreach (var item in psd.activeParty)
        {
            CharacterHolder holder = new CharacterHolder();
            holder.position = item.position;
            holder.character = CharacterBuilder.inst.GenerateFromSave(item.charSave);
            activeParty.Add(item.charSave.ID,holder);
        }
        foreach (var item in psd.benched)
        {
            CharacterHolder holder = new CharacterHolder();
            holder.position = item.position;
            holder.character = CharacterBuilder.inst.GenerateFromSave(item.charSave);
            benched.Add(item.charSave.ID,holder);
        }
        onPartyEdit.Invoke();
    }
}