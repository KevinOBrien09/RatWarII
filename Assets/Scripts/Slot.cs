using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System.Linq;
public class Slot : MonoBehaviour,IPointerEnterHandler,IPointerClickHandler
{
    public SpriteRenderer border;
    public Unit unit;
    public Node node;
    public TempTerrain tempTerrain;
    public GameObject indicator;
    public List<Vector3> directions = new List<Vector3>();
    public Transform rayShooter;
    public SpecialSlot specialSlot;
    void Start()
    {
        
        directions.Add(rayShooter.forward);
        directions.Add(-rayShooter.forward);
        directions.Add(rayShooter.right);
        directions.Add(-rayShooter.right);
    }

    public void ChangeColour(Color32 color)
    {border.color = color;}

    public List<Slot> GetNeighbouringSlots()
    {
        List<Slot> slots = new List<Slot>();
        foreach (var item in directions)
        {
            RaycastHit hit ;
            if(Physics.Raycast(rayShooter.position,item * 5,maxDistance: 5,hitInfo: out hit))  
            {
                Slot s = null;
                if(hit.collider.gameObject !=null)
                {
                    if(hit.collider.gameObject.TryGetComponent<Slot>(out s))
                    { slots.Add(s); }
                }
            }
        }
        return slots;
    }

    public void MakeSpecial(SpecialSlot specialSlotPrefab){
   
        specialSlot = Instantiate(specialSlotPrefab,transform);
    
        specialSlot.slot = this;
    }

    public List<Slot> peewee(List<Slot> candidates,List<Slot> slots,bool skill)
    {
        foreach (var item in GetNeighbouringSlots())
        {
            if(candidates.Contains(item))
            {
                if(!slots.Contains(item))
                {
                    if(skill)
                    {
                        // if(item.unit == null)
                        // {
                            slots.Add(item);
                            item.peewee(candidates,slots,skill);
                     //   }
                       
                    }
                    else
                    {
                        if(item.unit == null && !item.node.isBlocked)
                        {
                            slots.Add(item);
                            item.peewee(candidates,slots,skill);
                        }
                    }
                   
                }
            }
        }
        return slots;
    }
 

    public List<Slot> GetValidSlotsInRadius(int radius,bool skill)
    {
        List<Slot> candidateSlots = new List<Slot>();
        List<Slot> validSlots = new List<Slot>();
        Collider[] c = Physics.OverlapSphere(transform.position,radius*5);
        foreach(var item in c)
        {
            Slot s = null;
            bool v = item.TryGetComponent<Slot>(out s);
            if(v)  
            {
                if(!skill){
                if(!s.node.isBlocked && s.unit == null)
                {candidateSlots.Add(s);}
                }
                else{
                    candidateSlots.Add(s);
                }
               
            }
        }
        List<Slot> ss = new List<Slot>(peewee(candidateSlots,new List<Slot>(),skill));
        if(skill)
        {goto end; }
        foreach (var item in MapManager.inst.slots)
        {
            if(!ss.Contains(item))
            {
                if(item.unit != null)
                { 
                    if(item.unit.stats().passable)
                    {
                        item.node.isBlocked = false;
                        MapManager.inst.fuckYouSlots.Add(item);
                    }
                    else
                    { 
                        item.node.isBlocked = true;
                        MapManager.inst.fuckYouSlots.Add(item);
                       
                    }
                    
                }
                else
                {
                    MapManager.inst.fuckYouSlots.Add(item);
                    item.node.isBlocked = true;
                }
              
            }
            
        }
        end:
        if(ss.Contains(this)){
  ss.Remove(this);
        }
      
        return ss;
    }

    public List<Slot> GetHorizontalSlots(int length,Skill skill = null)
    {
        List<Slot> validSlots = new List<Slot>();
        int maxX = MapManager.inst.grid.iGridSizeX-1;
        List<Slot> right =   Loop(length, node.iGridX,maxX,true,true,skill);//right
        List<Slot> left =   Loop(length,node.iGridX,maxX,true,false,skill);//left


        foreach (var item in left)
        {
            if(!validSlots.Contains(item))
            {validSlots.Add(item);}
        }

        foreach (var item in right)
        {
            if(!validSlots.Contains(item))
            {validSlots.Add(item);}
        }

        return validSlots;
    }

     public List<Slot> GetVerticalSlots(int length,Skill skill = null)
    {
        List<Slot> validSlots = new List<Slot>();
        int maxY = MapManager.inst.grid.iGridSizeY-1;
     
     
        List<Slot> up = Loop(length,node.iGridY,maxY,false,true,skill);//up
        List<Slot> down = Loop(length,node.iGridY,maxY,false,false,skill);//down

        foreach (var item in up)
        {
            if(!validSlots.Contains(item))
            {validSlots.Add(item);}
        }

        foreach (var item in down)
        {
            if(!validSlots.Contains(item))
            {validSlots.Add(item);}
        }
        return validSlots;
    }

    public List<Slot> GetSlotsInPlusShape(int length,Skill skill = null)
    {
        List<Slot> validSlots = new List<Slot>();

        List<Slot> vertical = GetVerticalSlots(length,skill);
        List<Slot> horizontal = GetHorizontalSlots(length,skill);

        foreach (var item in horizontal)
        {
            if(!validSlots.Contains(item))
            {validSlots.Add(item);}
        }

        foreach (var item in vertical)
        {
            if(!validSlots.Contains(item))
            {validSlots.Add(item);}
        }
        return validSlots;
    }

    List<Slot> Loop(int length, int dir,int clamp,bool X,bool increase,Skill skill)
    {
        List<Slot> candidateSlots = new List<Slot>();
        int tiles = length+1;
        var projectileSkill = skill as ProjectileSkill;
        bool PENIS = false;
        List<Slot>previousSlots = new List<Slot>();

        for (int i = 0; i < tiles; i++)
        {
            if(PENIS)
            { break; }
            if(dir <= clamp && dir >= 0 )
            {
                Slot s = null;
                if(X)
                { s = MapManager.inst.grid.NodeArray[dir,(int)node.iGridY].slot; }
                else
                { s = MapManager.inst.grid.NodeArray[(int)node.iGridX,dir].slot; }
                
                if(s == null)
                { 
                    if(projectileSkill!= null)
                    {
                        if(!projectileSkill.goThroughWalls)
                        { break; }
                    }
                    else
                    { break; }
                }
                else
                {
                    if(s. tempTerrain != null){
                        break;
                    }

                    if(previousSlots.Count > 0)
                    {
                        Slot lastSlot = previousSlots.Last();
                        if(lastSlot != null){
                            if(lastSlot.unit != null){
                                if(!projectileSkill.passThrough){
                                    PENIS  = true;
                                    break;
                                }
                            }
                        }
                    }   
                    
                    AddToCandidateSlots(s); 
                    if(!previousSlots.Contains(s) && s!= this)
                    {previousSlots.Add(s);}
                    
                }
            }
            else
            { break; }

            if(increase)
            {dir++;}
            else
            {dir--;}
        
        }
        void AddToCandidateSlots(Slot s)
        {
            if(s == this){return;}
            
            candidateSlots.Add(s);
        }
        return candidateSlots;
    }
        
    public void OnPointerClick(PointerEventData eventData)
    {
        if(GameManager.inst.checkGameState(GameState.UNITMOVE))
        {return;}
        if(SkillAimer.inst.castDecided)
        {return;}

        if(eventData.button == PointerEventData.InputButton.Left)
        {
            if(SkillAimer.inst.aiming)
            {
                if(SkillAimer.inst._skill.doesntNeedUnitInSlot){
                    SkillAimer.inst.RecieveSlot(this);
                }
                else{
                    if(this.unit != null)
                    {
                        SkillAimer.inst.RecieveSlot(this);
                    }
                }
               
               
            }
            else{
                if(unit != null)
                {
                    if(!GameManager.inst.checkGameState(GameState.PLAYERUI) )
                    {
                        if(!GameManager.inst.checkGameState(GameState.PLAYERSELECT))   
                        {
                            if(unit == BattleManager.inst.currentUnit)
                            {
                                if(ActionMenu.inst.currentState == ActionMenu.ActionMenuState.ROAM){
                                    return;
                                }
                                if(!ActionMenu.inst.open)
                                {ActionMenu.inst.Show(this); }
                            }
                            else
                            {SlotInfoDisplay.inst.Apply(this);}
                        }
                    }
                }
                else
                {
                    if(GameManager.inst.checkGameState(GameState.PLAYERSELECT)  && !UnitMover.inst.inCoro)
                    {
                        if(UnitMover.inst.validSlots.Contains(this))
                        {UnitMover.inst.InitializeMove(this); }
                    }else
                        {SlotInfoDisplay.inst.Apply(this);}
                        
                }
            }
           
        }
      
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if(!node.isBlocked || unit != null)
        {
            if(GameManager.inst.checkGameState(GameState.PLAYERSELECT))
            {
                if(UnitMover.inst.validSlots.Contains(this))
                {
                    SlotSelector.inst.Attach(this);
                    List<Node> path = MapManager.inst.aStar.FindPath(UnitMover.inst.selectedSlot.transform.position,
                    SlotSelector.inst.transform.position);
                    UnitMover.inst.NewHover(path);
                }
            }

            if(!GameManager.inst.checkGameState(GameState.PLAYERSELECT) |!GameManager.inst.checkGameState(GameState.PLAYERUI))
            { SlotSelector.inst.Attach(this); }
     
        }
    }


}
