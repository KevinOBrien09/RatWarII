using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System.Linq;
using UnityEngine.Events;
using DG.Tweening;
using UnityEngine.SceneManagement;

public class PartyController : Singleton<PartyController>
{
    public List<OverworldUnit> playerUnits = new List<OverworldUnit>();
    public OverworldUnit leader;
    public bool run;
    public Vector3 newPos;
    public SoundData footstep;
    bool firstclick;
    float timestamp;
   public float footstepFreq = .2f;
   public List<Transform> partyTransforms = new List<Transform>();
   public OverworldUnit selected;
    void Start(){
        CamFollow.inst.gameObject.SetActive(false);
    }

     public void Organize(){
        Party p = PartyManager.inst.parties[PartyManager.inst.currentParty];
        playerUnits = playerUnits.OrderBy(o=> p.members [ o.battleUnit.character.ID].position).ToList();
        playerUnits.Reverse();
    }

    public void ChangeSelected(OverworldUnit u){
        foreach (var item in playerUnits)
        {item.selectedSignifier.SetActive(false);}
        u.selectedSignifier.SetActive(true);
        selected = u;
    }

    public void TakeControl()
    {
    
        Cursor.lockState = CursorLockMode.Confined;
        leader = playerUnits[0];
        leader.agent.stoppingDistance = 0;
        float baseSpeed = 15;
        float newSpeed = baseSpeed;
        for (int i = 1; i < playerUnits.Count; i++)
        {
            playerUnits[i].agent.stoppingDistance = 7.5f;
            newSpeed = baseSpeed - .5f;
            playerUnits[i].agent.speed = baseSpeed;
        }
        
        OverworldCamera.inst.target = leader.transform;
        partyTransforms.Clear();
        foreach (var item in playerUnits)
        {
            partyTransforms.Add(item.transform);
        }
        if(selected == null){
            ChangeSelected(leader);
        }
        
      //  CamFollow.inst.ChangeCameraState(CameraState.LOCK);
            run = true;
    }

    void Update()
    {
        if(run)
        {
            if(!BattleManager.inst.inBattle)
            {
                GetInput();
                if(firstclick)
                {
                    leader.Move(newPos);
                    OverworldUnit lastGuy = leader;
                    for (int i = 1; i < playerUnits.Count; i++)
                    {
                        OverworldUnit u = playerUnits[i];
                        u.followTarget = lastGuy;
                        u.Move(lastGuy.transform.position);
                        lastGuy = u;
                    }
                }
                foreach (var item in playerUnits)
                {
                    
                
                    if(timestamp < Time.time && item.agent.velocity.magnitude > .1f){
                        AudioManager.inst.GetSoundEffect().Play(footstep);
                        timestamp = Time.time + footstepFreq;
                    }
                }
            }
        }
        if(partyTransforms.Count > 0){
            Vector3 center = MiscFunctions.FindCenterOfTransforms(partyTransforms);
            transform.position = new Vector3(center.x,0,center.z);
        }
       

        if(Input.GetKeyDown(KeyCode.Q)){
            BattleManager.inst.Begin();
        }
    }

    public void Kill(Unit u)
    {
        OverworldUnit ou = null;
        foreach (var item in playerUnits)
        {
            if(item.battleUnit == u){
                ou = item;
                break;
            }
        }

        playerUnits.Remove(ou);
        Destroy(ou.gameObject);
      
    }

    public void GetInput()
    {   
        if(Menu.inst.open){
            return;
        }
        RaycastHit hit; 

        if(Input.GetMouseButtonDown(1))
        { 
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if(Physics.Raycast(ray, out hit))
            { 
                // if(onNavmesh(hit.point))
                // {
                    Vector3 v = hit.point;
                    ClickArrows.inst.Move(new Vector3(v.x,v.y + .1f,v.z));
                //}
            }
        }

        if(Input.GetMouseButton(1))
        { 
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
          
            if(Physics.Raycast(ray, out hit))
            { 
                // if(onNavmesh(hit.point))
                // {
                    firstclick = true;
                    newPos = hit.point;
               // }
            } 
        }

        bool onNavmesh(Vector3 point)
        {
            NavMeshHit hit;
            return  NavMesh.SamplePosition(point, out hit, 1.0f, NavMesh.AllAreas);
        }
    }
}