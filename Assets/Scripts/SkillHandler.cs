using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
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
    public GameObject costTab;
    public List<GameObject> icons = new List<GameObject>();
    public GenericDictionary<SkillResource.Catagory,Sprite> dict = new GenericDictionary<SkillResource.Catagory, Sprite>();
    public List<TextMeshProUGUI> costText = new List<TextMeshProUGUI>();
    public Image resourceIcon;
    void Start(){
        Close();
        Debug.Log(gameObject.name);
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
                    if(!SkillAimer.inst.aiming&& !ActionMenu.inst.FUCKOFF)
                    {
                        if(SkillAimer.inst.canCast(BattleManager.inst.currentUnit,hoveredSkill)){
                            BattleManager.inst.StartCoroutine(SkillHandler.inst.SetObject(null));
                            SkillAimer.inst.Go(hoveredSkill);
                        
                            ActionMenu.inst.Hide();
                        }
                        else{
                            Debug.LogWarning("Cannot Cast " +hoveredSkill.skillName);
                        }
                     
                       
                        
                       
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
         costTab.SetActive(false);
        hoveredSkill = null;
        hoveredBehaviour = null;
        ActionMenu.inst.ReturnFromSkillMenu();
        gameObject.SetActive(false);
       // BattleTicker.inst.Wipe();
        open = false;
    }

    public void ChangeCostDetails(Skill skill){
     
        costTab.SetActive(true);
        icons[0].gameObject.SetActive(true);
        int q = skill.additionalMoveTokenCost+1;
        if(skill.resourceCost > 0){
            icons[1].gameObject.SetActive(true);
            costText[0].text = "X"+ q.ToString();
            resourceIcon.sprite = dict[BattleManager.inst.currentUnit.skillResource.catagory];
            int i =  BattleManager.inst.currentUnit.skillResource.Convert(skill.intendedResource,skill.resourceCost);
            costText[1].text = ":"+i;
        }
        else
        {icons[1].gameObject.SetActive(false);}
      
        
    }

}