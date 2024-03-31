using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections.Generic;
public class CharacterCell : DragDropCell
{
    public enum CellType{TAB,PARTY}
    public CellType cellType;
    public int position;
    public override int canTake(Draggable d)
    {
       
        CharacterDragger a = d as CharacterDragger;
        CharacterDragger b = draggable as CharacterDragger;
        if(cellType == CellType.TAB)
        {
            
            if(a.tab.cell == this)
            {
            
              
                if(BackbenchHandler.inst.editing  )
                {
                    if(BackbenchHandler.inst.editingParty.members.ContainsKey(a.tab.character.ID)){
   a.NoCell();
                    return 0; 
                    }
                    else{
                        return 1;
                    }
                 
                }
                if(draggable == null)
                { 
                    AudioManager.inst.GetSoundEffect().Play(a. snap);
                    return 1; }
                }   
        }
        else if(cellType == CellType.PARTY)
        {
            if(b != null)
            {
                
                if(  BackbenchHandler.inst.editingParty.members.ContainsKey(a.tab.character.ID) &&   BackbenchHandler.inst.editingParty.members.ContainsKey(b.tab.character.ID))
                { 
                    AudioManager.inst.GetSoundEffect().Play(a. snap);
                    return 2; 
                }
            }
            else
            {
                Character c = a.tab.character;
                if(draggable == null)
                { 
                    if(  BackbenchHandler.inst.editingParty.members.ContainsKey(c.ID)) //moving from party slot to unoccupied party slot
                    {
                        BackbenchHandler.inst.editingParty.UpdatePosition(c,position);
                        AudioManager.inst.GetSoundEffect().Play(a. snap);
                    }
                   
                    else if(!  BackbenchHandler.inst.editingParty.members.ContainsKey(c.ID) && PartyManager.inst.characterBelongsInLocation(c))
                    {
                        a.tab.inPartySignifier.gameObject.SetActive(true);
                        AudioManager.inst.GetSoundEffect().Play(a. snap);
                        AudioManager.inst.GetSoundEffect().Play(CharacterBuilder.inst.sfxDict[c.species].turnStart);
                        BackbenchHandler.inst.editingParty.BenchToParty(c,position);
                        a.tab.ToggleDismissButton(false);
                    }
                   
                    return 1; 
                }
            }
        }

        return 0; 
    }
}