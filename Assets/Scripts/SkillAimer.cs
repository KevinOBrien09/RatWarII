using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillAimer : Singleton<SkillAimer>
{
    public enum Aim{SELF,RADIUS,PROJECTILE}
    public Aim currentState;
    public bool aiming;
   public Slot slot;  
   public Unit caster;
    public    List<Slot> validSlots = new List<Slot>();
    public List<Unit> validTargets = new List<Unit>();
    public bool castDecided;
    public Skill _skill;
    public   SkillCastBehaviour skillCastBehaviour;
    public void Leave()
    {
        if(!castDecided){
 //Debug.Log("Leave");
        foreach (var item in validSlots)
        {item.ChangeColour(item.normalColour);}
         BattleManager.inst.currentUnit.slot.DisableHover();
        validSlots.Clear();
         validTargets.Clear();
                _skill = null;
        BattleManager.inst. ToggleHealthBars(false);
        BattleManager.inst.StartCoroutine(SkillHandler.inst.SetObject(SkillHandler.inst.hoveredBehaviour.gameObject));
        ActionMenu.inst.Show(slot);
        aiming = false;
        }
       
    }

    public bool canCast(Unit u,Skill s)
    {return u.skillResource.canSpend(s.resourceCost);}

    public void RecieveSlot(Slot s)
    {
        if(!castDecided)
        { 
           
            if(validSlots.Contains(s))
            {  
               

                foreach (var item in validSlots)
                {
                
               item.ChangeColour(item.normalColour);
                    
                }
                Minimap.inst.Hide();
                BattleManager.inst.currentUnit.slot.DisableHover();
                castDecided = true;  
                BattleTicker.inst.Type(_skill.skillName);
                GameManager.inst.ChangeGameState(GameState.UNITMOVE);
                CastArgs args = new CastArgs();
                args.caster = BattleManager.inst.currentUnit ;
                if(s.cont.unit != null)
                {
                    args.target = s.cont.unit;
                }
                
                args.targetSlot = s;
                args.skill = _skill;
                
                if(_skill.skillCastBehaviour == null)
                {Debug.LogAssertion("No SkillCastBehaviour in " + _skill.skillName + "'s scriptable object. Game is now softlocked.");}
                else
                {
                    SkillHandler.inst.costTab.SetActive(false);
                    skillCastBehaviour  = Instantiate( _skill.skillCastBehaviour);
                    int realCost = BattleManager.inst.currentUnit.skillResource.Convert(_skill.intendedResource,_skill.resourceCost);
                    BattleManager.inst.currentUnit.skillResource.Spend(realCost);
                    skillCastBehaviour.Go(args);
                    Cursor.lockState = CursorLockMode.Locked;
                }
            }
        }
        
    }
    
    public void Go(Skill s)
    {
        
        _skill = s;
        caster = BattleManager.inst.currentUnit;
        slot = caster.slot;
        if(BattleManager.inst.currentUnit.side == Side.PLAYER){
            GameManager.inst.ChangeGameState(GameState.PLAYERSELECT);
        }
        BattleTicker.inst.Type("Preparing " + s.skillName+"...");
       
        //BattleManager.inst.ToggleHealthBars(true);
        aiming = true;
        if(s is SelfSkill selfSkill)
        {SelfCast(selfSkill);}
        else if(s is ProjectileSkill proj)
        {ProjectileAim(proj);}
        else if(s is RadiusSkill radiusSkill)
        {RadiusAim(radiusSkill);}
        else{
           Debug.LogWarning("Base Skill Not Allowed!");
         
        }
       
    }

    public void Finish(bool wasSkipped = false){
        validTargets.Clear();
        BattleManager.inst.ToggleHealthBars(false);
    
      //  Debug.Log("Finish");
        aiming = false;
        
        if(skillCastBehaviour != null){
 Destroy(skillCastBehaviour.gameObject);
        }
        else{
            Debug.LogWarning("Skill cast behaviour was already killed.");
        }
       
        skillCastBehaviour = null;
        foreach (var item in validSlots)
        {item.ChangeColour(item.normalColour);}
        castDecided = false;
        _skill = null;
        validSlots.Clear();
        SkillHandler.inst.  Close();
        BattleManager.inst.EndTurn(wasSkipped);
    }
    
    public void ProjectileAim(ProjectileSkill skill)
    {
        if(BattleManager.inst.currentUnit.side == Side.PLAYER){
            currentState = Aim.PROJECTILE;
                Minimap.inst.ResizeFOV(skill.howManyTiles);
            Minimap.inst.Show();
            CamFollow.inst.ChangeCameraState(CameraState.FREE);
            Cursor.lockState = CursorLockMode.Confined;
       
        }
   
      //  Character casterChar = caster.character;
    

        switch(skill.projectilePath)
        {
            case ProjectileSkill.ProjectilePathShape.PLUS:
            validSlots   = new List<Slot>(slot.func.GetSlotsInPlusShape(skill.howManyTiles,skill));
            break;
            case ProjectileSkill.ProjectilePathShape.VERT:
            validSlots   = new List<Slot>( slot.func.GetVerticalSlots(skill.howManyTiles,skill));
            break;
            case ProjectileSkill.ProjectilePathShape.HORI:
            validSlots   = new List<Slot>(slot.func.GetHorizontalSlots(skill.howManyTiles,skill));
            break;
            case ProjectileSkill.ProjectilePathShape.X:
            validSlots   = new List<Slot>(slot.func.GetXSlots(skill.howManyTiles,skill));
            break;
             case ProjectileSkill.ProjectilePathShape.ASTERISK:
            validSlots   = new List<Slot>(slot.func.GetAsteriskSlots(skill.howManyTiles,skill));
            break;
            default:
            Debug.LogAssertion("PROJECTILE PATH NOT IMPLEMENTED!!");
            break;

        }

        List<Slot> validClone = new List<Slot>(validSlots);
        foreach (var item in validClone)
        {
            if(item.cont.unit != null)
            {
                if(!item.cont.unit.isEntity() )
                {
                    if(!skill.canHitBreakableSlots)
                    {validSlots.Remove(item);}
                }
                
              
            }
        }

        if(BattleManager.inst.currentUnit.side == Side.PLAYER){
            foreach (var item in validSlots)
            {item.ChangeColour(item.skillColour);} 
        }
         
    }

    public void RadiusAim(RadiusSkill skill)
    {
        currentState = Aim.RADIUS;
        if(BattleManager.inst.currentUnit.side == Side.PLAYER){
Minimap.inst.ResizeFOV(skill.radius);
        Minimap.inst.Show();
        }
        
       
        Character casterChar = caster.character;
        Cursor.lockState = CursorLockMode.Confined;
        CamFollow.inst.ChangeCameraState(CameraState.FREE);
        validSlots   = slot.func. GetRadiusSlots(skill.radius,skill,false);
       
        foreach (var item in validSlots)
        {
            if(item.cont.unit != null)
            {
                if(!validTargets.Contains(item.cont.unit)){
                validTargets.Add(item.cont.unit);
                }
            }
        }
        if(skill.canSelfCast)
        {
            if(!validSlots.Contains(caster.slot))
            {
                validSlots.Add(caster.slot);
                validTargets.Add(caster);
            }
        }

        foreach (var item in validTargets)
        {   
            if(item.isEntity())
            {
                if(item.side == skill.side)
                {
                    if(skill.showHealthBars){
                    item.healthBar.gameObject.transform.parent.gameObject.SetActive(true);
                    }
        
                }
                else
                {validSlots.Remove(item.slot);}
            }
            else
            {
                if(!skill.canHitBreakableSlots){
                    validSlots.Remove(item.slot);
                }

            }
        }
   
   
      
        foreach (var item in validSlots)
        {
            if(item.cont.unit == null){
                
            }
            item.ChangeColour(item.skillColour);
        }  

    }

    public void SelfCast(SelfSkill skill)
    {
        currentState = Aim.SELF;
        validSlots   = slot.func.GetRadiusSlots(skill.radius,skill,false);
      
        validSlots.Add(slot);
        BattleManager.inst.currentUnit.slot.hoverBorderOn();
        List<Slot> validClone = new List<Slot>(validSlots);
        foreach (var item in validClone)
        {
            if(item.cont.unit != null)
            {
                if(item.cont.unit.isEntity() )
                {
                    if(skill.showHealthBars)
                    {
                    if(item.cont.unit.side == skill.side)
                    {item.cont.unit.healthBar.gameObject.transform.parent.gameObject.SetActive(true);}
                    }
                }
                else{
                     if(!skill.canHitBreakableSlots){
                    validSlots.Remove(item);
                    }
                }
                
              
            }
           item.ChangeColour(item.skillColour);
        }

       
    }

    void Update()
    {
        if(!castDecided && aiming)
        {
            if(currentState == Aim.SELF)
            {
                if(InputManager.inst.player.GetButtonDown("Confirm")){
                    RecieveSlot(slot);
                }
            }
        }
    
    }

    public void Skip(){
        StartCoroutine(q());
        IEnumerator q()
        {
            
            yield return new WaitForSeconds(.5f);
            Finish();

       }
    }


 
}
