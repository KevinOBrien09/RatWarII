using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using DG.Tweening;
using System.Linq;

public class SlotInfoDisplay : Singleton<SlotInfoDisplay>
{
    public TextMeshProUGUI charName,hp,resource,speciesClass,level,moveTokenCount;
    public TextMeshProUGUI speed,moveRange,strength,magic,defence;
    public GenericDictionary<SkillResource.Catagory,Sprite> resBarDict = new GenericDictionary<SkillResource.Catagory, Sprite>();
    public RawImage icon;
    public Image resourceFill;
    public GameObject resourceBar;
    public Vector2 shown,hidden;
    public RectTransform rt;
    public StatusEffectStackHandler stackHandler;
    public HealthBar healthBar;
    public Transform moveTokenHolder;
    public List<GameObject>moveTokes = new List<GameObject>();
    public Slot sl;
    public GameObject actionTokenPrefab,moveTokenPrefab,godTokenPrefab;
    public bool doGodTokenShine;
    public List<Image> godTokenImages = new List<Image>();
    public SoundData refundSFX;
    void Start()
    {
        shown = rt.anchoredPosition;
        Disable();
     
    }

    public void Apply(Slot slot)
    {
        
        sl = slot;
        if(slot.cont.unit != null)
        { 
            if(slot.cont.unit.isEntity())
            {
               
                ApplyUnit(slot.cont.unit);
                if(slot.cont.specialSlot != null){
                    List<SlotContents> sc = new List<SlotContents>(slot.cont.slotContents);
                    sc.Add(slot.cont.specialSlot.slotContents);

                    stackHandler.SlotContents(sc);
                }
            }
        }
         else
            {   
                if(!gameObject.activeSelf){
                    gameObject.SetActive(true);
                    rt.DOAnchorPos(hidden,0);
                    rt.DOAnchorPos(shown,.2f);
                }
                else{      gameObject.SetActive(true);
                    rt.DOAnchorPos(shown,.2f);
                }
            
                if( !GameManager.inst.checkGameState(GameState.PLAYERUI)) 
                {
                    if(ActionMenu.inst.currentState == ActionMenuState.ROAM)
                    {CamFollow.inst.Focus(slot.transform,()=>{CamFollow.inst.ChangeCameraState(CameraState.FREE);});}
                    else
                    {CamFollow.inst.Focus(slot.transform,()=>{});}
                }
                if(slot.cont.specialSlot == null){
                    charName.text = "Empty Slot";
                    icon.texture = null;    
                    icon.gameObject.SetActive(false);
                }
                else{
                    charName.text = slot.cont.specialSlot.slotContents.contentName;
                    icon.texture =  slot.cont.specialSlot.slotContents.picture;   
                    icon.gameObject.SetActive(true );
                }
                
            
                healthBar.gameObject.SetActive(false);
                resourceBar.gameObject.SetActive(false);
                stackHandler.Kill();
            
                stackHandler.SlotContents(slot.cont.slotContents);
                
                hp.text = string.Empty;
                level.text = "0";
                speciesClass.text = string.Empty;
                speed.text =string.Empty;
                strength.text =string.Empty;
                moveRange.text = string.Empty;
                magic.text = string.Empty;
                defence.text = string.Empty;
                    
            }
    }



    public void ApplyUnit(Unit u)
    {
        healthBar.health = u.health;
            healthBar.gameObject.SetActive(true);
            resourceBar.gameObject.SetActive(true);
            healthBar.Refresh();
            
            if(!gameObject.activeSelf){
                gameObject.SetActive(true);
                rt.DOAnchorPos(hidden,0);
                rt.DOAnchorPos(shown,.2f);
            }
            else{      gameObject.SetActive(true);
                rt.DOAnchorPos(shown,.2f);
            }
            
            if( ! GameManager.inst.checkGameState(GameState.PLAYERUI)) 
            {
                if(CamFollow.inst.gameObject.activeSelf){

                    if(ActionMenu.inst.currentState == ActionMenuState.ROAM){
                    CamFollow.inst.Focus(u. transform,()=>{CamFollow.inst.ChangeCameraState(CameraState.FREE);});
                    }
                    else{
                        CamFollow.inst.Focus(u.transform,()=>{});
                    }

                }
               
            
            }
            
           
            Character c = u.character;
            charName.text = c.characterName.fullName();
        
            level.text = c.exp.level.ToString();
            if(u.side == Side.PLAYER ){ //&& !sl.cont.unit.isHostage
                speciesClass.text = c.job.ToString() + " "+ c.species.ToString();
            }
            else{
                speciesClass.text = u.enemy.tagLine;
            }
            stackHandler.Kill();
            stackHandler.Spawn(u);
            

            
            resourceFill.sprite = resBarDict[u.skillResource.catagory];
            resourceFill.DOFillAmount((float)u.skillResource.current/(float)u.skillResource.max,0);
            resource.text = u.skillResource.abbrv() + u.skillResource.current.ToString() +"/" + u.skillResource.max.ToString();
            
            SetStats(u);
            icon.gameObject.SetActive(true);
            if(u.side == Side.PLAYER)
            {
            if(u.isHostage) 
            {
            icon.texture = u.enemy.icon;
            }
            else{

                icon.texture = IconGraphicHolder.inst.dict[u.character.ID];
            }
            
            }
            else{
                icon.texture = u.enemy.icon;
            }

        MoveTokens(u);
    }

    public void MoveTokens(Unit u){
        foreach (var item in moveTokes)
        {
            Destroy(item.gameObject);
        }
        moveTokes.Clear();
        moveTokenCount.text = " BT:"+ u.battleTokens.total().ToString();
        BattleTokens bt = u.battleTokens;

        for (int i = 0; i < bt.moveToken; i++)
        {
            GameObject g = Instantiate(moveTokenPrefab,moveTokenHolder);
            moveTokes.Add(g);
        }

        for (int i = 0; i < bt.actionToken; i++)
        {
            GameObject g = Instantiate(actionTokenPrefab,moveTokenHolder);
            moveTokes.Add(g);
        }
        
        for (int i = 0; i < bt.godToken; i++)
        {
            GameObject g = Instantiate(godTokenPrefab,moveTokenHolder);
            moveTokes.Add(g);
            if(doGodTokenShine){
                godTokenImages.Add(g.GetComponent<Image>());
            }
        }
     
        if(doGodTokenShine && godTokenImages.Count > 0)
        {
            StartCoroutine(z());
            IEnumerator z()
            {
                yield return new WaitForSeconds(.3f);
                doGodTokenShine = false;
                Image gT =   godTokenImages.Last();
                Color co = gT.color;
                gT.transform.localRotation = Quaternion.Euler(0,0,-90);
                gT.transform.DOLocalRotate(Vector3.zero,.2f).OnComplete(()=>
                {
                    StartCoroutine(q());
                    IEnumerator q(){
                    yield return new WaitForSeconds(.2f);
                    gT.transform.DOScale(new Vector3(.5f,.5f,.5f),.2f);
                }});
                
                AudioManager.inst.GetSoundEffect().Play(refundSFX);
                gT.transform.DOScale(new Vector3(.6f,.6f,.6f),.2f).OnComplete(()=>
                {
                   
                StartCoroutine(q());
                IEnumerator q(){
                    gT.DOColor(Color.white,0);
                    yield return new WaitForSeconds(refundSFX.audioClip.length + .3f);
                  
                    gT.DOColor(co,.2f);
                      godTokenImages.Clear();
                }
                });
            }
            }
        
        
      

        
    }
    
    public void Disable()
    {
        
        stackHandler.Kill();
        sl = null;
        healthBar.health = null;
        rt.DOAnchorPos(hidden,.2f).OnComplete(()=>
        {
           // gameObject.SetActive(false);
        });

    }

    public void SetStats(Unit u){
        string normal =  "<color=white>";
        string increase = "<color=yellow>";
        string decrease = "<color=lightblue>";
        string end = "</color>";


        string speedModColour = normal;
        if(u.stats().speed > u.character.baseStats.speed)
        {speedModColour = increase;}
        else if(u.stats().speed < u.character.baseStats.speed)
        {speedModColour = decrease;}
        speed.text = "SPD:" + speedModColour + u.stats().speed.ToString() + end;

        string strModColour = normal;
        if(u.stats().strength > u.character.baseStats.strength)
        {strModColour = increase;}
        else if(u.stats().strength <  u.character.baseStats.strength)
        {strModColour = decrease;}
        strength.text = "STR:" + strModColour + u.stats().strength.ToString() + end;

        string moveModColour = normal;
        if(u.stats().moveRange > u.character.baseStats.moveRange)
        {moveModColour = increase;}
        else if(u.stats().moveRange < u.character.baseStats.moveRange)
        {moveModColour = decrease;}
        moveRange.text = "MVE:" + moveModColour + u.stats().moveRange.ToString() + end;

        string mgkModColour = normal;
        if(u.stats().magic > u.character.baseStats.magic)
        {mgkModColour = increase;}
        else if(u.stats().magic < u.character.baseStats.magic)
        {mgkModColour = decrease;}
        magic.text = "MGK:" + mgkModColour+ u.stats().magic.ToString() + end;

        string defModColour = normal;
        if(u.stats().defence > u.character.baseStats.defence)
        {defModColour = increase;}
        else if(u.stats().defence < u.character.baseStats.defence)
        {defModColour = decrease;}
        defence.text ="DEF:"+ defModColour + u.stats().defence.ToString() +"%</color>";
    }

  
}