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
    public void Leave()
    {
        Debug.Log("Leave");
        foreach (var item in MapManager.inst.slots)
        {
            item.indicator.SetActive(false);
            item.ChangeColour(UnitMover.inst. baseSlotColour);
            
        }
        SlotSelector.inst.gameObject.SetActive(false);
        validSlots.Clear();

        BattleManager.inst.StartCoroutine(SkillHandler.inst.SetObject(SkillHandler.inst.hoveredBehaviour.gameObject));
        ActionMenu.inst.Show(slot);
        aiming = false;
    }

    public void RecieveSlot(Slot s)
    {
        if(validSlots.Contains(s))
        {
            switch (currentState)
            {
                case Aim.SELF:
                Finish();
                break;
                case Aim.PROJECTILE:
                Finish();
                break;
                case Aim.RADIUS:
                Finish();
                break;
                
                default:
                Debug.LogAssertion("DEFAULT CASE");
                break;
            }
        }
    }
    
    public void Go(Skill s)
    {
        caster = BattleManager.inst.currentUnit;
        slot = caster.slot;
        BattleTicker.inst.Type("Preparing " + s.skillName+"...");
        aiming = true;
        if(s is SelfSkill selfSkill)
        {SelfCast(selfSkill);}
        else if(s is ProjectileSkill proj)
        {ProjectileAim(proj);}
        else if(s is RadiusSkill radiusSkill)
        {RadiusAim(radiusSkill);}
        else{
           // Debug.LogWarning("Base Skill Not Allowed!");
          Finish();
        }
       
    }

    public void Finish(){
           MapManager.inst.fuckYouSlots.Clear();
     Debug.Log("Finish");
        aiming = false;
        validSlots.Clear();
        foreach (var item in MapManager.inst.slots)
        {
            item.indicator.SetActive(false);
            item.ChangeColour(UnitMover.inst. baseSlotColour);
            
        }
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
    
       
        Character casterChar = caster.character;
        Cursor.lockState = CursorLockMode.Confined;
        CamFollow.inst.ChangeCameraState(CameraState.FREE);
        validSlots   = new List<Slot>(slot.GetValidSlotsInRadius(skill.radius,true));
        foreach (var item in validSlots)
        {item.ChangeColour(Color.gray);}  

    }

    public void SelfCast(SelfSkill skill)
    {
           currentState = Aim.SELF;
    }
}
