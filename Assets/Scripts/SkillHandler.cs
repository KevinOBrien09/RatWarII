using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;
using TMPro;

public class SkillHandler : Singleton<SkillHandler>
{
    public SkillBehaviour prefab;
    public List<SkillBehaviour> currentSkills = new List<SkillBehaviour>();
    public RectTransform holder;
    public bool open;
    public Skill hoveredSkill;
    public SkillBehaviour hoveredBehaviour;
    public ScrollRectAutoScroll scrollRectAutoScroll;
    void Start(){
        Close();
    }
    public void Open()
    {
        gameObject.SetActive(true);
        if(currentSkills.Count > 0){
            StartCoroutine(q(currentSkills[0].gameObject));
           IEnumerator q(GameObject g)
            {
                EventSystem.current.SetSelectedGameObject(null);
                yield return new WaitForEndOfFrame();
                EventSystem.current.SetSelectedGameObject(g);
                scrollRectAutoScroll.ScrollToSelected();
            }
            
     
        }
    open = true;

    }

    void Update()
    {
        if(open && !ActionMenu.inst.FUCKOFF)
        {
              
            if(InputManager.inst.player.GetButtonDown("Cancel"))
            { 
                if(SkillAimer.inst.castDecided){
                    return;
                }
                if(!SkillAimer.inst.aiming )
                { Close(); }
                else{ SkillAimer.inst.Leave(); }
            
            }
            else if(InputManager.inst.player.GetButtonDown("Confirm"))
            {
              
                if(hoveredSkill != null)
                {
                    if(!SkillAimer.inst.aiming)
                    {
                        BattleManager.inst.StartCoroutine(SkillHandler.inst.SetObject(null));
                        SkillAimer.inst.Go(hoveredSkill);
                      
                        ActionMenu.inst.Hide();
                    }
                
                    // Close();
                    // BattleManager.inst.EndTurn();
                }
                else{
                    Debug.LogWarning("hover skill is null");
                }
               
            }

            
        }
    }

    public void NewUnit(Unit u)
    {
        foreach (var item in currentSkills)
        { Destroy(item.gameObject); }
        currentSkills.Clear();
        int i = 0;
        foreach (var item in u.character.skills)
        {
            SkillBehaviour sb = Instantiate(prefab,holder);
            sb.Init(i,item);
            currentSkills.Add(sb);
            i++;
            
        }
    }

    public IEnumerator SetObject(GameObject g)
    {
        EventSystem.current.SetSelectedGameObject(null);
        yield return new WaitForEndOfFrame();
        EventSystem.current.SetSelectedGameObject(g);//scrollRectAutoScroll.ScrollToSelected();
    }

    public void Close()
    {
        hoveredSkill = null;
        hoveredBehaviour = null;
        ActionMenu.inst.ReturnFromSkillMenu();
        gameObject.SetActive(false);
       // BattleTicker.inst.Wipe();
        open = false;
    }

}