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
    public Vector2 battlePosition;
}
public enum XBattlePos{LEFT,CENTER,RIGHT}
public enum YBattlePos{FORWARD,MIDDLE,BACK}
[System.Serializable]
public struct BattlePosition{
    
    public XBattlePos x;
    public YBattlePos y;

    public BattlePosition(BattlePosition bp){
        x = bp.x;
        y = bp.y;
    }

    public Vector2 ToVector2()
    { return new Vector2(XToFloat(x),YToFloat(y)); }

    public  float XToFloat(XBattlePos x){
        switch(x)
        {
            case XBattlePos.LEFT:
            return 0;
           
            case XBattlePos.CENTER:
            return 1;
            
            case XBattlePos.RIGHT:
            return 2;
            
        }
        Debug.LogAssertion("AHH");
        return 0;
    }

    public  float YToFloat(YBattlePos y){
        switch(y)
        {
            case YBattlePos.FORWARD:
            return 0;
           
            case YBattlePos.MIDDLE:
            return 1;
            
            case YBattlePos.BACK:
            return 2;
            
        }
        Debug.LogAssertion("AHH");
        return 0;
    }
}

[System.Serializable]
public class HolderSaveData{
    public CharacterSaveData charSave;
    public Vector2 mapTileID;
    public Vector2 battlePos;
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
    public GenericDictionary<Vector2,string> battlePositions = new GenericDictionary<Vector2,string>();
 // public UnityEvent onPartyEdit;
    public int partySize = 4;

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
            hsd.battlePos = item.Value.battlePosition;
      
            ips.members.Add(hsd);
        }
        return ips;
    }

  
    
    public void KillMember(Character c)
    { 
        if(!PartyManager.inst.deadCharacters .ContainsKey(c.ID))
        {PartyManager.inst.deadCharacters .Add(c.ID,members[c.ID]);}

        if(members.ContainsKey(c.ID))
        {
            battlePositions.Remove(members[c.ID].battlePosition);
            members.Remove(c.ID);
        }
        if(members.Count <=0)
        {
            AllMembersDead();
        }
      
        SavePartyEdit();
        //SaveLoad.Save(GameManager.inst.saveSlotIndex,PartyManager.inst.PartyUpdateSave());
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
            CharacterHolder oldHolder = members[c.ID];

            CharacterHolder holder = new CharacterHolder();
            holder.mapTileID = LocationManager.inst.currentLocation;
            holder.character = c;
            holder.position = -5;
            if(battlePositions.ContainsKey(oldHolder.battlePosition)){
                battlePositions.Remove(oldHolder.battlePosition);
            }
            else{
                Debug.LogAssertion( c.ID + ":NOT IN BP DICTIONARY: " + oldHolder.battlePosition );
            }
           
            holder.battlePosition = CharacterBuilder.inst.jobDict[ c.job].battlePosition.ToVector2();
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
            Vector2 bp = GetValidBattlePosition(CharacterBuilder.inst.jobDict[c.job].battlePosition);
            Debug.Log(bp + "bp");
            if( !battlePositions.ContainsKey(bp)){
                battlePositions.Add(bp,c.ID);
            }
            
            holder.battlePosition = bp;
            members.Add(c.ID,holder);
            PartyManager.inst.benched.Remove(c.ID);
            InvokePartyEdit();
        }
    }

    public Vector2 GetValidBattlePosition(BattlePosition bp)
    {
        Vector2 v = bp.ToVector2();
        if(!battlePositions.ContainsKey(v)){
            return v;
        }
        else
        {
            float oldY = v.y;
            foreach(XBattlePos x in System.Enum.GetValues(typeof(XBattlePos)))
            {
                Vector2 XD = new Vector2(bp.XToFloat(x),oldY);
                if(!battlePositions.ContainsKey(XD)) 
                {
                    return  XD;
                }
                
            } // check if slot in preffered vertical pos
            foreach(YBattlePos y in System.Enum.GetValues(typeof(YBattlePos))) //if no valid preffered vert pos just place it anywhere valid
            {
                foreach(XBattlePos x in System.Enum.GetValues(typeof(XBattlePos)))
                {
                    Vector2 XD = new Vector2(bp.XToFloat(x),bp.YToFloat(y));
                    if(!battlePositions.ContainsKey(XD)) 
                    { return  XD; }
                }
            }

            Debug.LogAssertion("NO VALID START SLOTS!?1");
            return Vector2.zero;
        }
    }

    public void UpdatePosition(Character c, int i)
    {
        members[c.ID].position = i;     
          InvokePartyEdit();
    }
    
    public void SavePartyEdit() //??
    {SaveLoad.Save(GameManager.inst.saveSlotIndex,PartyManager.inst.PartyUpdateSave()); }

   
}