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
    public SoundData error;
    public bool pathAvailable;
    public NavMeshPath navMeshPath;


    void Start(){
        CamFollow.inst.gameObject.SetActive(false);
        navMeshPath = new NavMeshPath();
    }

     public void Organize(){
        Party p = PartyManager.inst.parties[PartyManager.inst.currentParty];
        playerUnits = playerUnits.OrderBy(o=> p.members [ o.battleUnit.character.ID].position).ToList();
        playerUnits.Reverse();
    }

    public void ChangeSelected(OverworldUnit u){
        foreach (var item in playerUnits)
        {item.selectedSignifier.gameObject. SetActive(false);}
        foreach (var item in PortraitManager.inst.dict)
        {
           Portrait p = item.Value;
           p.interactor.gameObject.SetActive(false);
        }
         u.ToggleSelected();
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

                Vector3 v = new Vector3();
                 NavMeshHit filter;
                if(!NavMesh.SamplePosition(hit.point, out filter, 50, NavMesh.AllAreas))
                {
                    firstclick = true;
                    v = filter.position;
                }
                else{
                    v = hit.point;
                 
                }
                if(PathIsValid(filter.position)){
                    ClickArrows.inst.Move(new Vector3(v.x,v.y + .1f,v.z));
                }
                else
                {
                    AudioManager.inst.GetSoundEffect().Play(error);
                   
                }
               
            }
        }

        if(Input.GetMouseButton(1))
        { 
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
          
            if(Physics.Raycast(ray, out hit))
            { 
                NavMeshHit filter;
                bool b = NavMesh.SamplePosition(hit.point, out filter, 100, -1);
                if(b)
                {
                    if(PathIsValid(filter.position)){
                        firstclick = true;
                        newPos = filter.position;
                    }
                    else{
                        Debug.Log("Path is not valid");
                    }
                   
                }
            } 
        }
    }
    
    bool PathIsValid(Vector3 target) 
    {
        leader.agent.CalculatePath(target, navMeshPath);
        return navMeshPath.status == NavMeshPathStatus.PathComplete;
    }
}