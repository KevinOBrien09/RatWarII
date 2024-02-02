using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillAimer : Singleton<SkillAimer>
{
    public enum Aim{SELF,RADIUS,PROJECTILE}
    public Aim currentState;
    public bool aiming;
    Slot slot;  
    Unit caster;
    public    List<Slot> validSlots = new List<Slot>();
    public List<Unit> validTargets = new List<Unit>();
   public bool castDecided;
   public Skill _skill;
    public void Leave()
    {
        if(!castDecided){
 Debug.Log("Leave");
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
        if(!castDecided){
           
            if(validSlots.Contains(s))
            {  CamFollow.inst.Focus(slot.transform,()=>
            {  CamFollow.inst.ChangeCameraState(CameraState.LOCK);
            });
                castDecided = true;  
                BattleTicker.inst.Type(_skill.skillName);
                SlotSelector.inst.gameObject.SetActive(false);
                /// do stuff
                //then here
                foreach (var item in _skill.effects)
                {
                    item.Go();
                }
                // switch (currentState)
                // {
                //     case Aim.SELF:
                //     Finish();
                //     break;
                //     case Aim.PROJECTILE:
                //     Finish();
                //     break;
                //     case Aim.RADIUS:
                //     Finish();
                //     break;
                    
                //     default:
                //     Debug.LogAssertion("DEFAULT CASE");
                //     break;
                // }
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
        Debug.Log("Finish");
        aiming = false;
        validSlots.Clear();
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
        currentState = Aim.PROJECTILE;
   
        Character casterChar = caster.character;
        Cursor.lockState = CursorLockMode.Confined;
          SlotSelector.inst.gameObject.SetActive(true);
        CamFollow.inst.ChangeCameraState(CameraState.FREE);
        validSlots   = new List<Slot>(slot.GetSlotsInPlusShape(skill.howManyTiles,skill));
        foreach (var item in validSlots)
        {item.ChangeColour(Color.gray);}  


    }

    public void RadiusAim(RadiusSkill skill)
    {
        currentState = Aim.RADIUS;
        SlotSelector.inst.gameObject.SetActive(true);
        SlotSelector.inst.Attach(slot);
       
        Character casterChar = caster.character;
        Cursor.lockState = CursorLockMode.Confined;
        CamFollow.inst.ChangeCameraState(CameraState.FREE);
        validSlots   = new List<Slot>(slot.GetValidSlotsInRadius(skill.radius,true));
       
        foreach (var item in validSlots)
        {
            if(item.unit != null)
            {
                if(!validTargets.Contains(item.unit)){
                validTargets.Add(item.unit);
                }
            }
        }
        foreach (var item in validTargets)
        {
            if(item.side == skill.side)
            {item.healthBar.gameObject.transform.parent.gameObject.SetActive(true);}
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
        validSlots = new List<Slot>();
        validSlots.Add(slot);
        SlotSelector.inst.gameObject.SetActive(true);
        SlotSelector.inst.Attach(slot);
       
    }

    void Update()
    {
        if(!castDecided)
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
