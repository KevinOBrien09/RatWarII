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
    bool firstclick;

    void Start(){
        CamFollow.inst.gameObject.SetActive(false);
    }
    public void GrabUnits()
    { 
        //playerUnits = new List<Unit>(BattleManager.inst.playerUnits);
    }

    public void Organize(){
        Party p = PartyManager.inst.parties[PartyManager.inst.currentParty];
        playerUnits = playerUnits.OrderBy(o=> p.members [ o.battleUnit.character.ID].position).ToList();
        playerUnits.Reverse();
    }

    public void TakeControl()
    {
    
        Cursor.lockState = CursorLockMode.Confined;
        leader = playerUnits[0];
        float baseSpeed = playerUnits[0].agent.speed;
        float newSpeed = baseSpeed;
        for (int i = 1; i < playerUnits.Count; i++)
        {
            playerUnits[i].agent.stoppingDistance = 10;
            newSpeed = baseSpeed - .5f;
            playerUnits[i].agent.speed = baseSpeed;
        }
       OverworldCamera.inst.target = leader.transform;
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
            }
        }

        if(Input.GetKeyDown(KeyCode.Q)){
            BattleManager.inst.Begin();
        }
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
                if(onNavmesh(hit.point))
                {
                    Vector3 v = hit.point;
                    ClickArrows.inst.Move(new Vector3(v.x,v.y + .1f,v.z));
                }
            }
        }

        if(Input.GetMouseButton(1))
        { 
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
          
            if(Physics.Raycast(ray, out hit))
            { 
                if(onNavmesh(hit.point))
                {
                    firstclick = true;
                    newPos = hit.point;
                }
            } 
        }

        bool onNavmesh(Vector3 point)
        {
            NavMeshHit hit;
            return  NavMesh.SamplePosition(point, out hit, 1.0f, NavMesh.AllAreas);
        }
    }
}