using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.Events;

[System.Serializable]
public class CharacterHolder{
    public Character character;
    public string lastPartyID;
    public Vector2 mapTileID;
  
    public int position;
}

[System.Serializable]
public class HolderSaveData{
    public CharacterSaveData charSave;
    public Vector2 mapTileID;
   
    public int position;
}


[System.Serializable]
public class PartySaveData
{
    [System.Serializable]
    public class IndividualPartySave
    {
        public string id;
        public string partyName;
        public Vector2 mapTileID;
        public List<HolderSaveData> members = new List<HolderSaveData>();

    }
    
    public string activePartyID;
    public List<IndividualPartySave> individualParties = new List<IndividualPartySave>();
    public List<HolderSaveData> benched = new List<HolderSaveData>();
    public List<HolderSaveData> deceased = new List<HolderSaveData>();

}

[System.Serializable]
public class Party 
{
    public delegate void PartyEvent();
    public PartyEvent onPartyEdit;
    public Vector2 mapTileID;
    public string ID;
    public string partyName;
    public GenericDictionary<string, CharacterHolder> members = new GenericDictionary<string, CharacterHolder>();

 // public UnityEvent onPartyEdit;
    public int partySize = 3;

    public void ChangeMapLocation(Vector2 v){
        mapTileID = v;
      InvokePartyEdit();
    }

    public void Create(){
        ID = System.Guid.NewGuid().ToString();
        string a,b;
        List<string> start =PartyManager.inst.partyNameSO .firstNames[Gender.NA];
        List<string> end = PartyManager.inst.partyNameSO .surnames;
        a =  start [Random.Range(0,start.Count)] ;
        b = end[Random.Range(0,end.Count)];
        partyName = a + " " + b;
        mapTileID = LocationManager.inst.currentLocation;
    }

 
    
   public PartySaveData.IndividualPartySave Save()
   {
        PartySaveData.IndividualPartySave ips = new PartySaveData.IndividualPartySave();
        ips.partyName = partyName;
        ips.id = ID;
        ips. mapTileID = mapTileID;
        foreach (var item in members)
        {
            HolderSaveData hsd = new HolderSaveData();
            hsd.mapTileID = mapTileID;
            hsd.position = item.Value.position;
            hsd.charSave = item.Value.character.Save();
          //  hsd.unitSaveData = PartyManager.inst. GetUnitData(item.Value.character,item.Value);
            ips.members.Add(hsd);
        }
        return ips;
    }

  
    
    public void KillMember(Character c)
    { 
        if(!PartyManager.inst.deadCharacters .ContainsKey(c.ID))
        {PartyManager.inst.deadCharacters .Add(c.ID,members[c.ID]);}

        if(members.ContainsKey(c.ID))
        {members.Remove(c.ID);}
        if(members.Count <=0)
        {
            AllMembersDead();
        }
        
        SaveLoad.Save(GameManager.inst.saveSlotIndex,PartyManager.inst.PartyUpdateSave());
        InvokePartyEdit();
    }

    public void AllMembersDead()
    {
        members.Clear();
        PartyManager.inst.RemoveParty(this);

    }

    void InvokePartyEdit()
    {
        if(onPartyEdit != null)
        {onPartyEdit.Invoke(); }
        else
        {Debug.LogWarning("onPartyEdit has no listeners!");}
    }
    
    public void PartyToBench(Character c)
    {
        if(members.ContainsKey(c.ID) )
        {
            CharacterHolder holder = new CharacterHolder();
            holder.mapTileID = LocationManager.inst.currentLocation;
            holder.character = c;
            holder.position = -5;
            PartyManager.inst.benched.Add(c.ID,holder);
            members.Remove(c.ID);
            InvokePartyEdit();
        }
    }

    public int TotalPartyLevel(){
        int i = 0;
        foreach (var item in members)
        {
            i += item.Value.character.exp.level;
            
        }

        return i;
    }

    public void BenchToParty(Character c,int i)
    {
        if(PartyManager.inst.characterBelongsInLocation(c) )
        {
            CharacterHolder holder = new CharacterHolder();
            holder.mapTileID = LocationManager.inst.currentLocation;
            holder.character = c;
            holder.position = i;
            members.Add(c.ID,holder);
            PartyManager.inst.benched.Remove(c.ID);
            InvokePartyEdit();
        }
    }

    public void UpdatePosition(Character c, int i)
    {
        members[c.ID].position = i;     
          InvokePartyEdit();
    }


    public int lastOpenPosition()
    {
        List<CharacterHolder> h = new List<CharacterHolder>();
        foreach (var item in members)
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

    public void SavePartyEdit() //??
    {SaveLoad.Save(GameManager.inst.saveSlotIndex,PartyManager.inst.PartyUpdateSave()); }

   
}