using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEngine.UI;
using DG.Tweening;
public class CharacterProfileMenu : Singleton< CharacterProfileMenu>
{
    public SoundData error,goldSpend;
    public List<Character> characters = new List<Character>();
    public TextMeshProUGUI charName,title,hp,str,spd,move,mgk;
    public Image baseChar,gender;
    public GameObject left,right,hireButton;
    public bool debug;
    int index;
 
    public void RecieveCharacters(List<Character> l)
    {
        characters = new List<Character>(l);
        if(characters.Count > 0){
            LoadCharacter(characters[0]);
            index = 0;
        }
    }

    public void LoadCharacter(Character c){
        
        if(c.gender == Gender.FEMALE)
        {
            if(c.job == Job.KNIGHT)
            {
                if(c.species != Species.FROG)
                {
                    if(c.spriteVarient == 3)
                    { 
                        gender.enabled = false; //Female BucketHelm Knights do not have lashes"
                        goto spriteSetUp;
                    }
                }
            }
            gender.enabled = true;
            gender.sprite =  CharacterBuilder.inst.female[c.species];
        }
        else{ gender.enabled = false;}
        spriteSetUp:
        baseChar.sprite = CharacterBuilder.inst.classVarients[c.species][c.job][c.spriteVarient];
        charName.text = c.characterName.fullName();
        title.text = c.job.ToString() + " "+ c.species.ToString();
        hp.text = "HP:" + c.stats().hp.ToString();
        spd.text = "SPD:" + c.stats().speed.ToString();
        move.text = "MVE:" + c.stats().moveRange.ToString();
        str.text = "STR:" + c.stats().strength.ToString();
        mgk.text = "MGK:" + c.stats().magic.ToString();

    }

    public void Left()
    {   
        if(CharacterRecruiter.inst.state == 2)
        {
            EventSystem.current.SetSelectedGameObject(null);
            index--;
            ResetArrows();
            LoadCharacter(characters[index]);
        }
        
    }

    public void Right()
    {
        if(CharacterRecruiter.inst.state == 2)
        {
            EventSystem.current.SetSelectedGameObject(null);
            index++;
            ResetArrows();
            LoadCharacter(characters[index]);
        }
    }

    public void ResetArrows()
    {
        if(index != characters.Count-1)
        {right.SetActive(true);}
        if(index == 0)
        {left.SetActive(false);}
        if(index == characters.Count-1)
        {right.SetActive(false);}
        if(index != 0)
        {left.SetActive(true);}
    }

    public void BackOutButton(){
        if(CharacterRecruiter.inst.state == 2){
            CharacterRecruiter.inst.BackOut();
        }
    }

    public void Hire()
    {
        if(CharacterRecruiter.inst.state == 2){
            if(PartyManager.inst.canAfford(300)|debug)
            {
                if(!debug){
               PartyManager.inst.RemoveGold(300);
                }
               
                Character c = characters[index];
                AudioManager.inst.GetSoundEffect().Play(CharacterBuilder.inst.sfxDict[c.species].move);
                AudioManager.inst.GetSoundEffect().Play(goldSpend); 
                characters.Remove(c);
                PartyManager.inst. AddToPossession(c);
                if(characters.Count > 0)
                {
                    
                    index--;
                    index = Mathf.Clamp(index,0,characters.Count);
                    ResetArrows();
                    LoadCharacter(characters[index]);
                }
                else
                {
                    hireButton.SetActive(false);
                    baseChar.enabled = false;
                    charName.text = "Empty";
                    title.enabled = false;
                    hp.enabled = false;
                    gender.enabled = false;
                    spd.enabled = false;
                    move.enabled = false;
                    str.enabled = false;
                    Debug.Log("No characters left");
                }
            }
            else
            {
                AudioManager.inst.GetSoundEffect().Play(error); 
                Debug.Log("No Money");
            }
        }
       
        
    }

}