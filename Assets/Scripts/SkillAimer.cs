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
        foreach (var item in MapManager.inst.slots)
        {
            item.indicator.SetActive(false);
            item.ChangeColour(UnitMover.inst. baseSlotColour);
            
        }
        SlotSelector.inst.gameObject.SetActive(false);
        validSlots.Clear();
         validTargets.Clear();
                _skill = null;
        BattleManager.inst. ToggleHealthBars(false);
        BattleManager.inst.StartCoroutine(SkillHandler.inst.SetObject(SkillHandler.inst.hoveredBehaviour.gameObject));
        ActionMenu.inst.Show(slot);
        aiming = false;
        }
       
    }

    public void RecieveSlot(Slot s)
    {
        if(!castDecided)
        { 
           
            if(validSlots.Contains(s))
            {  
               

                foreach (var item in MapManager.inst.slots)
                {
                    item.indicator.SetActive(false);
                    item.ChangeColour(UnitMover.inst. baseSlotColour);
                    
                }
                
            
                castDecided = true;  
                BattleTicker.inst.Type(_skill.skillName);
                SlotSelector.inst.gameObject.SetActive(false);
                CastArgs args = new CastArgs();
                args.caster = BattleManager.inst.currentUnit ;
                if(s.unit != null)
                {
                    args.target = s.unit;
                }
                
                args.targetSlot = s;
                args.skill = _skill;
                args.castEffects = ()=>
                {
                    foreach (var item in _skill.effects)
                    {item.Go();}
                
                };
                if(_skill.skillCastBehaviour == null)
                {Debug.LogAssertion("No SkillCastBehaviour in " + _skill.skillName + "'s scriptable object. Game is now softlocked.");}
                else
                {
                    skillCastBehaviour  = Instantiate( _skill.skillCastBehaviour);
                    skillCastBehaviour.Go(args);
                }
            }
        }
        
    }
    
    public void Go(Skill s)
    {
        _skill = s;
        caster = BattleManager.inst.currentUnit;
        slot = caster.slot;
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

    public void Finish(){
        validTargets.Clear();
        BattleManager.inst.ToggleHealthBars(false);
        MapManager.inst.fuckYouSlots.Clear();
      //  Debug.Log("Finish");
        aiming = false;
        validSlots.Clear();
        if(skillCastBehaviour != null){
 Destroy(skillCastBehaviour.gameObject);
        }
        else{
            Debug.LogWarning("Skill cast behaviour was already killed.");
        }
       
        skillCastBehaviour = null;
        foreach (var item in MapManager.inst.slots)
        {
            item.indicator.SetActive(false);
            item.ChangeColour(UnitMover.inst. baseSlotColour);
            
        }
        castDecided = false;
        _skill = null;
        SlotSelector.inst.gameObject.SetActive(false);
        SkillHandler.inst.  Close();
        BattleManager.inst.EndTurn();
    }
    
    public void ProjectileAim(ProjectileSkill skill)
    {
        if(BattleManager.inst.currentUnit.side == Side.PLAYER){
        currentState = Aim.PROJECTILE;
        SlotSelector.inst.gameObject.SetActive(true);
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
        if(BattleManager.inst.currentUnit.side == Side.PLAYER){
            foreach (var item in validSlots)
            {item.ChangeColour(Color.gray);} 
        }
         
    }

    public void RadiusAim(RadiusSkill skill)
    {
        currentState = Aim.RADIUS;
        SlotSelector.inst.gameObject.SetActive(true);
        SlotSelector.inst.Attach(slot);
       
        Character casterChar = caster.character;
        Cursor.lockState = CursorLockMode.Confined;
        CamFollow.inst.ChangeCameraState(CameraState.FREE);
        validSlots   = slot.func. GetRadiusSlots(skill.radius,true);
       
        foreach (var item in validSlots)
        {
            if(item.unit != null)
            {
                if(!validTargets.Contains(item.unit)){
                validTargets.Add(item.unit);
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
            if(item.side == skill.side)
            {
                if(skill.showHealthBars){
          item.healthBar.gameObject.transform.parent.gameObject.SetActive(true);
                }
      
            }
            else
            {validSlots.Remove(item.slot);}
        }
   
   
      
        foreach (var item in validSlots)
        {
            if(item.unit == null){
                
            }
            item.ChangeColour(Color.gray);
        }  

    }

    public void SelfCast(SelfSkill skill)
    {
        currentState = Aim.SELF;
        validSlots   = slot.func.GetRadiusSlots(skill.radius,true);
        validSlots.Add(slot);
        foreach (var item in validSlots)
        {
            if(item.unit != null)
            {
                if(skill.showHealthBars){
                    if(item.unit.side == skill.side)
                    {item.unit.healthBar.gameObject.transform.parent.gameObject.SetActive(true);}
                }
              
            }
            item.ChangeColour(Color.gray);
        }
        SlotSelector.inst.gameObject.SetActive(true);
        SlotSelector.inst.Attach(slot);
       
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


    public bool canCast(Skill skill)
    {
        // switch(skill.ID)
        // {
        //     case "35433542-3142-45a9-b3d4-93096ef99883": //barrier
        //     if(BattleManager.inst.)
        //     break;
        //     default:
        //     return true;
            
        // }
        return false;
    }
}
