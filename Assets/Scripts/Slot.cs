using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.EventSystems;
using System.Linq;
public class Slot : MonoBehaviour,IPointerEnterHandler,IPointerClickHandler,IPointerExitHandler
{
    public SlotContainer cont = new SlotContainer();
    public SlotFunctions func = new SlotFunctions();
    public Node node;
    public List<IntrusiveMeshHandler> intrusiveMeshes = new List<IntrusiveMeshHandler>();
    public SpriteRenderer border,hoverBorder,areaIndicator;
    public Room room;
    public Transform rayShooter;
    public Color32 normalColour,moveColour,interactColour,skillColour;
    public bool dormant,marked;
    public IntrusiveMeshHandler meshBelow;
    public MeshRenderer mesh;
    public MeshFilter mf;
    public bool isWater,isBoat;
    void Awake()
    {
        cont.slot = this;
        func.slot = this;
    }

    public void ChangeColour(Color32 color)
    {border.color = color;}

    public void DealWithSurrondingIntrusions()
    {
        if(cont.specialSlot != null)
        {
            if(cont.specialSlot.intrusiveMesh != null)
            {
                Vector2 n = new Vector2(node.iGridX,node.iGridY+1);
                if(MapManager.inst.nodeIsValid(n))
                {
                    Slot item = MapManager.inst.map.NodeArray[(int) n.x,(int) n.y].slot;
                    if(item. hasUnitRightAbove(this.node))
                    {
                        cont.specialSlot.intrusiveMesh.MakeTrans();
                        MapManager.inst.intrustiveSlotsCurrentlyMadeTransparent.Add(this);
                    }
                    else
                    {
                        
                        cont.specialSlot.intrusiveMesh.Reset();
                        
                    }
                }
            }   
        }
    }

    public void ChangeMaterial(Material m){
        mesh.material = m;
    }

    void MouseOverIntrusion(){
        if(!cont.wall){
        Vector2 n = new Vector2(node.iGridX,node.iGridY-1);
        if(MapManager.inst.nodeIsValid(n))
        {  
            if(n.x == node.iGridX)
            {
               
                Slot s = MapManager.inst.map.NodeArray[(int)n.x,(int)n.y].slot;
                if(s != null)
                {
                    if(s. cont.specialSlot != null)
                    {
                        if(s.cont.specialSlot.intrusiveMesh != null)
                        {
                            s.cont.specialSlot.intrusiveMesh.MakeTrans();
                            meshBelow =  s.cont.specialSlot.intrusiveMesh;
                        }
                    }
                }
               
            }
           
        }
        }
   
    }

    public bool hasUnitRightAbove(Node og){

        if(cont.unit != null)
        {return node.iGridY > og.iGridY && node.iGridX == og.iGridX;}

        return false;
        
    }
    
    
   
    public void ActivateAreaIndicator(Color32 color)
    {
        areaIndicator.gameObject.SetActive(true);
        areaIndicator.color = color;
    }
    public void DectivateAreaIndicator()
    { areaIndicator.gameObject.SetActive(false); }
    
    public SpecialSlot MakeSpecial(SpecialSlot specialSlotPrefab)
    {
        cont.specialSlot = Instantiate(specialSlotPrefab,transform);
        if(cont.specialSlot.interactable != null)
        {cont.specialSlot.interactable.slot = this;}
        cont.specialSlot.slot = this;
        cont.specialSlot.Init();
        return cont.specialSlot;
    }

    public void IsWall()
    {
        cont.wall = true;
        cont.invisible = true;
        border.gameObject.SetActive(false);
        hoverBorder.gameObject.SetActive(false);
    }
    
    public virtual void SlotHover()
    {
        if(cont.invisible)
        { return; }
       
        if(GameManager.inst.checkGameState(GameState.PLAYERSELECT))
        {
            MouseOverIntrusion();
            if(UnitMover.inst.validSlots.Contains(this))
            {
                if(cont.walkable())
                {
                    hoverBorderOn();
                    List<Node> path =  MapManager.inst.map.aStar.FindPath(UnitMover.inst.selectedSlot.node,node);
                    UnitMover.inst.NewHover(path);
                }
              
            }
           
            else if(SkillAimer.inst.validSlots.Contains(this))
            { hoverBorderOn(); }
        }
        else if(GameManager.inst.checkGameState(GameState.INTERACT))
        {
            if(InteractHandler.inst.slots.Contains(this))
            { hoverBorderOn();
            SlotInfoDisplay.inst.Apply(this); }
        }
    }
    
    public void SlotSelect()
    {
        
        if(cont.invisible)
        { return; }
        if(SkillAimer.inst.castDecided)
        {return;}
        if(!MapManager.inst.slotBelongsToGrid(this)){
            return;
        }

        switch(GameManager.inst.currentGameState)
        {
            case GameState.PLAYERHOVER:
            foreach (var item in MapManager.inst.allSlots)
            { item. DisableHover();}
            SlotInfoDisplay.inst.Apply(this);
            hoverBorderOn();
            break;
            case GameState.PLAYERSELECT:
            if(SkillAimer.inst.aiming)
            {
                if(this.cont.unit != null)
                { SkillAimer.inst.RecieveSlot(this);
                  DisableHover(); }
                else if(SkillAimer.inst._skill.doesntNeedUnitInSlot)
                { SkillAimer.inst.RecieveSlot(this); 
                  DisableHover();}
            }
            else //moving
            {
                if(GameManager.inst.checkGameState(GameState.PLAYERSELECT)  && !UnitMover.inst.inCoro)
                {
                    if(UnitMover.inst.validSlots.Contains(this))
                    {UnitMover.inst.InitializeMove(this); 
                      DisableHover();}
                }
                else
                {
                    SlotInfoDisplay.inst.Apply(this);
                  DisableHover();}
            }
            break;
         
            case GameState.INTERACT:
            if(InteractHandler.inst.slots.Contains(this)){
                InteractHandler.inst.TakeInteraction(this);
            }
     
            break;

            case GameState.ENEMYTURN:
            return;
            case GameState.UNITMOVE:
            return;
            case GameState.PLAYERUI:
            return;
            default:
            Debug.LogAssertion("default case!!!");
            break;

        }
    }
    
    public void hoverBorderOn()
    {
        if(cont.invisible)
        { return; }
        if(cont.unit == null)
        { hoverBorder.color = Color.white; }
       
        else if(cont.unit != null )
        {
            if(cont.unit.side == Side.ENEMY)
            {hoverBorder.color = Color.red;}
            else
            {hoverBorder.color = Color.blue;}
        }
        else if(cont.specialSlot != null && cont.unit == null)
        {
            if(cont.specialSlot.interactable != null)
            {hoverBorder.color = Color.magenta; }
        }

       
        hoverBorder.gameObject.SetActive(true);
        border.gameObject.SetActive(false);
    }

    public void DisableHover()
    {
        if(dormant)
        {return;}
        if(cont.invisible)
        { return; }
        hoverBorder.gameObject.SetActive(false);
        border.gameObject.SetActive(true);
    }

    public virtual void OnPointerClick(PointerEventData eventData)
    {
        if(eventData.button == PointerEventData.InputButton.Left)
        { SlotSelect();}
    }

    public virtual void OnPointerEnter(PointerEventData eventData)
    {SlotHover();}

    public virtual void OnPointerExit(PointerEventData eventData)
    {
        if(!GameManager.inst.checkGameState(GameState.PLAYERHOVER))
        { DisableHover(); }
        if(meshBelow!= null){
            meshBelow.Reset();
            meshBelow = null;
        }
    }

   
}
