using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.Events;


public class PartyManager : Singleton<PartyManager>
{
    public int gold;
    
    public string currentParty;
    public GenericDictionary<string,Party> parties = new GenericDictionary<string, Party>();
    public GenericDictionary<string, CharacterHolder> benched = new GenericDictionary<string, CharacterHolder>();
    public GenericDictionary<string, CharacterHolder> deadCharacters = new GenericDictionary<string, CharacterHolder>();
    public Names partyNameSO;

    void Start()
    {
        gold = 900;
        Refresh();
        
    }

    
    public void Wipe(){
        currentParty = string.Empty;
        parties.Clear();
        benched.Clear();
        deadCharacters.Clear();
        gold = 0;
    }

    public bool canAfford(int i)
    {return gold >= i;}

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


    public bool characterBelongsInLocation(Character c)
    {
        if(benched.ContainsKey(c.ID))
        {
            if(benched[c.ID].mapTileID == LocationManager.inst.currentLocation)
            {return true;}

        }
        
        return false;
    }

    public void AddNewParty(Party p)
    {
        if(!parties.ContainsKey(p.ID))
        {
            parties.Add(p.ID,p);
            if(parties.Count == 1){
                currentParty = p.ID;
            }
        }
    }

    public void RemoveParty(Party p)
    {
        if(parties.ContainsKey(p.ID))
        {
           
            foreach (var item in p.members)
            {benched.Add(item);}
            parties.Remove(p.ID);
           
        }
       
       
        

        if(parties.Count == 0){
            currentParty = string.Empty;
        }
        else{
     currentParty = parties.First().Value.ID;
        }
    }
   
   
    public void AddPartyFromSave(PartySaveData.IndividualPartySave ips)
    {
        Party p = new Party();
        p.ID = ips.id;
        p.mapTileID = ips.mapTileID;
        p.partyName = ips.partyName;
        foreach (var item in ips.members)
        {
            CharacterHolder holder = new CharacterHolder();
            holder.position = item.position;
            holder.mapTileID = item.mapTileID;
            holder.character = CharacterBuilder.inst.GenerateFromSave(item.charSave);
         
            p.members.Add(item.charSave.ID,holder); 
        }
        parties.Add(p.ID,p);
    }
    
    public PartySaveData Save()
    {
        PartySaveData psd = new PartySaveData();
        psd.activePartyID = currentParty;
        foreach (var item in parties)
        {
            psd.individualParties.Add(item.Value.Save());
        }
        foreach (var item in benched)
        {
            HolderSaveData hsd = new HolderSaveData();
            hsd.mapTileID = item.Value.mapTileID;
            hsd.position = item.Value.position;
            hsd.charSave = item.Value.character.Save();
            
            psd.benched.Add(hsd);
        }
        foreach (var item in deadCharacters)
        {
            HolderSaveData hsd = new HolderSaveData();
            hsd.mapTileID = item.Value.mapTileID;
            hsd.position = -10;
            hsd.charSave = item.Value.character. Save();
           
            psd.deceased.Add(hsd);
        }
        return psd;
    }

   


    public SaveData PartyUpdateSave()
    {
        SaveData sd = SaveLoad.Load(GameManager.inst.saveSlotIndex);
        sd.partySaveData = Save();
        return sd;

    }

    public void AddToPossession(Character c)
    {
        if(!PartyManager.inst.benched.ContainsKey(c.ID))
        {
            // if(members.Count < partySize)
            // {
            //     CharacterHolder holder = new CharacterHolder();
            //     holder.mapTileID = LocationManager.inst.currentLocation;
            //     holder.character = c;   
            //     members.Add(c.ID,holder);
            //     holder.position = lastOpenPosition();
            // }
            // else
            // {   
                CharacterHolder holder = new CharacterHolder();
                holder.mapTileID = LocationManager.inst.currentLocation;
                holder.character = c;
                holder.position = -5;
                // holder.unitSaveData = new UnitSaveData();
                // holder.unitSaveData.currentHP = c.stats().hp;
                PartyManager.inst.benched.Add(c.ID,holder);
            // }
            // onPartyEdit.Invoke();
            SaveLoad.Save(GameManager.inst.saveSlotIndex,PartyManager.inst.PartyUpdateSave());
        }
      
    }

    public void Dismiss(Character c){
        benched.Remove(c.ID);
        SaveLoad.Save(GameManager.inst.saveSlotIndex,PartyManager.inst.PartyUpdateSave());
    }

    public void XD(Vector2 lastLoc)
    {
    
        foreach (var item in parties)
        {
            if(item.Value.mapTileID == lastLoc)
            {
                currentParty = item.Value.ID;
                return;
            }  
          
        }
        currentParty = string.Empty;

        
    }

    public void Load(PartySaveData psd,Vector2 lastLoc )
    {
        currentParty = psd.activePartyID;
        foreach (var item in psd.individualParties)
        {
           AddPartyFromSave(item);
        }
        if(!parties.ContainsKey(currentParty)){
            Debug.LogWarning("Old Party string was not found!");
          XD(lastLoc);
        }
        else{
if(parties.Count == 1| parties[currentParty].mapTileID != lastLoc)
        {
            XD(lastLoc);
         //parties.First().Key;
        }
        }
        
        foreach (var item in psd.benched)
        {
            CharacterHolder holder = new CharacterHolder();
            holder.mapTileID = item.mapTileID;
            holder.position = item.position;
            holder.character = CharacterBuilder.inst.GenerateFromSave(item.charSave);
            benched.Add(item.charSave.ID,holder);
        }
        foreach (var item in psd.deceased)
        {
            CharacterHolder holder = new CharacterHolder();
            holder.mapTileID = item.mapTileID;
            holder.position = item.position;
            holder.character = CharacterBuilder.inst.GenerateFromSave(item.charSave);
            deadCharacters.Add(item.charSave.ID,holder);
        }
     //   onPartyEdit.Invoke();
    }

}