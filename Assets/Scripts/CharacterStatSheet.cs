using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;
using UnityEngine.UI;
using System.Linq;
using UnityEngine.EventSystems;
public class CharacterStatSheet : Singleton<CharacterStatSheet>
{
    
    public GenericDictionary<Aligment,Sprite> traitSpritDict = new GenericDictionary<Aligment, Sprite>();
    public CanvasGroup cg;
    public List<Image> charSprites = new List<Image>();
    public TextMeshProUGUI charName,jobSpecies;
    public int maxTraitCount;
    public TraitTab traitTabPrefab;
    public List<TraitTab> traitTabs = new List<TraitTab>();
    public List<StatScreenSkill> skillTabs = new List<StatScreenSkill>();
    public Transform ttHolder,skillHolder;
    public Trait empty;
    public HubStateHandler.HubState lastHubState;
    public TextMeshProUGUI hp,str,mgk,move,spd;
    public Image hpFill;
    public bool open;
    public StatScreenSkill statScreenSkillPrefab;
    public Character currentCharacter;
    void Start(){
        cg.alpha = 0;
        gameObject.SetActive(false);
    }

    public void Open(Character c)
    {
            SkillWindow.inst.Close();
        foreach (var item in traitTabs)
        { Destroy(item.gameObject); }

        foreach (var item in skillTabs)
        { Destroy(item.gameObject); }

        skillTabs.Clear();
        traitTabs.Clear();
        gameObject.SetActive(true);
        SetCharSprites(c);
        charName.text = c.characterName.fullName();
        jobSpecies.text = c.job.ToString() + " "+ c.species.ToString();
        hpFill.DOFillAmount((float) c.battleData.currentHP/(float)c.stats().hp,0);
        hp.text = "HP:"+c.battleData.currentHP+"/"+c.stats().hp;
        str.text = "STR:" + c.stats().strength;
        mgk.text = "MGK:" + c.stats().magic;
        spd.text = "SPD:" + c.stats().speed;
        move.text = "MVE:" + c.stats().moveRange;
        SpawnTraits(c);
        SpawnSkills(c);
        if(HubStateHandler.inst.currentState != HubStateHandler.HubState.STATSHEET){
             lastHubState = HubStateHandler.inst.currentState;
        }
      
        HubStateHandler.inst.ChangeStateString("Party-Profile");
        HubStateHandler.inst.ChangeState(HubStateHandler.HubState.STATSHEET);
        cg.DOFade(1,.2f);
        open = true;
        currentCharacter = c;
    }

    public void Close()
    {
        SkillWindow.inst.Close();
        cg.DOFade(0,.2f).OnComplete(()=>
        {
            open = false;
            if(lastHubState == HubStateHandler.HubState.PARTYEDIT){
                HubStateHandler.inst.ChangeState(HubStateHandler.HubState.PARTYEDIT);
                HubStateHandler.inst.ChangeStateString("Party-Edit");
            }
            else if(lastHubState == HubStateHandler.HubState.ORGANIZER)
            {
                HubStateHandler.inst.ChangeStateString("Party");
                HubStateHandler.inst.ChangeState(  HubStateHandler.HubState.ORGANIZER);
            }
            else{
                Debug.LogAssertion("Uh Oh...");
            }
            gameObject.SetActive(false);
            currentCharacter = null;
        });
    }

    public void SpawnSkills(Character c){
        string waitID = "718ac21d-535f-41ac-856e-cb018cfa695c";

        foreach (var item in c.skills)
        {
            if(item.ID != waitID){
                StatScreenSkill sss = Instantiate(statScreenSkillPrefab,skillHolder);
                sss.Init(item);
                skillTabs.Add(sss);
            }
          
        }
    }

    public void SpawnTraits(Character c){
        for (int i = 0; i < maxTraitCount; i++)
        {
            TraitTab tt = Instantiate(traitTabPrefab,ttHolder);
            if(c.traits.ElementAtOrDefault(i) != null){
                tt.Init(c.traits[i],traitSpritDict[c.traits[i].aligment]);
            }
            else{
                tt.Init(empty,traitSpritDict[empty.aligment]);
            }
            traitTabs.Add(tt);
        }

    }

    public void SetCharSprites(Character c){
        if(c.gender == Gender.FEMALE)
        {
            if(c.job == Job.KNIGHT)
            {
                if(c.species != Species.FROG)
                {
                    if(c.spriteVarient == 3){
                        charSprites[1].gameObject.SetActive(false);   //Female BucketHelm Knights do not have lashes"
             
                        goto spriteSetUp;
                    }
                }

            }
            charSprites[1].gameObject.SetActive(true);
            charSprites[1].sprite = CharacterBuilder.inst.female[c.species];
        }
        else
        {charSprites[1].gameObject.SetActive(false);}

        spriteSetUp:
    
        charSprites[0].sprite = CharacterBuilder.inst.classVarients[c.species][c.job][c.spriteVarient];
    }
}