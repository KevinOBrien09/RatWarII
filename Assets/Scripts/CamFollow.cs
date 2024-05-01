using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.Events;

public enum Direction{UP,DOWN,LEFT,RIGHT}
public enum CameraState{FREE,FOCUS,LOCK}
public class CamFollow : Singleton<CamFollow>
{
    public Transform target;
    public float downAndRightBarrier,upAndLeftBarrier;
    public float smoothspeed = 0.125f;
    public Vector3 offset;
    public CameraState currentState;
  
    public GenericDictionary<Direction,bool> allowedCameraMovement = new GenericDictionary<Direction, bool>();
    

    public bool STOPMOVING,disableEdgeMovement;
    public float baseFOV;

    void Start()
    {
        baseFOV = Camera.main.fieldOfView;
    }
    
    public void ChangeCameraState(CameraState newState)
    {
        currentState = newState;
    }

    public bool CheckCameraState(CameraState state)
    {
        if(currentState == state)
        {return true;}
        else
        {return false;}
    }

    void Update()
    {
        if(CheckCameraState(CameraState.FREE)|CheckCameraState(CameraState.FOCUS))
        {
            if(InputManager.inst.player.GetButtonDown("Focus"))
            {
                SlotInfoDisplay.inst.Disable();
                Focus(target,()=>{ChangeCameraState(CameraState.FREE);});
            }
            if(!SkillAimer.inst.castDecided){
            STOPMOVING = InputManager.inst.player.GetButton("Focus");
            }
            
          
        }
    }
    
    void LateUpdate()
    {
        if(!CheckCameraState(CameraState.FOCUS))
        {
            if(CheckCameraState(CameraState.FREE))
            {FreeRoam();}
            else if(CheckCameraState(CameraState.LOCK))
            {LockOn();}
            if(!SkillAimer.inst.castDecided){
            Zoom();
            }
           
        }
        
    }

    public void ForceFOV(float fov){
        Camera.main.DOFieldOfView(fov,.25f);
    }

    public void Focus(Transform newTarget,UnityAction a)
    {
        ChangeCameraState(CameraState.FOCUS);
   
        transform.DOMove( newTarget.position + offset,.2f).OnComplete(()=>
        {
            
            //ChangeCameraState(CameraState.LOCK);
            a.Invoke();
           
          
        }) ;
        
    }

    public void ZoomOut()
    {
        Camera.main.DOFieldOfView(baseFOV,.25f);
    }
    
    void Zoom(){
        var fov  = Camera.main.fieldOfView;
 
        float i =  InputManager.inst.player.GetAxis("ScrollWheel")/10;
        fov -=  i * 55;
        fov = Mathf.Clamp(fov, 16, baseFOV);
        Camera.main.fieldOfView = fov;
    }

    void LockOn(){
        if(target != null){
 Vector3 desiredposition = target.position + offset;
        Vector3 smoothedposition = Vector3.Lerp(transform.position, desiredposition, smoothspeed*Time.deltaTime);
        transform.position = smoothedposition;
        }
       
    }


    void FreeRoam()
    {  
        
        if(STOPMOVING)
        {return;}
    
        if(InputManager.inst.AnyWASDKeyHeld())
        {
            
            if(allowedCameraMovement[Direction.UP])
            {
                if(InputManager.inst.player.GetButton("Up"))
                {transform.Translate(Vector3.forward*Time.deltaTime*smoothspeed,Space.World); }
            }
        
            if(allowedCameraMovement[Direction.DOWN])
            {
                if(InputManager.inst.player.GetButton("Down"))
                {transform.Translate(-Vector3.forward*Time.deltaTime*smoothspeed,Space.World);}
            }

            if(allowedCameraMovement[Direction.RIGHT])
            {
                if(InputManager.inst.player.GetButton("Right"))
                {transform.Translate(Vector3.right*Time.deltaTime*smoothspeed,Space.World);}  
            }

            if(allowedCameraMovement[Direction.LEFT])
            {
                if(InputManager.inst.player.GetButton("Left"))
                {transform.Translate(-Vector3.right*Time.deltaTime*smoothspeed,Space.World);}
            }
        }
        else
        {
            if(disableEdgeMovement)
            {return;}


            if(allowedCameraMovement[Direction.UP])
            {
                if(Input.mousePosition.y >= Screen.height * upAndLeftBarrier)
                { transform.Translate(Vector3.forward*Time.deltaTime*smoothspeed,Space.World); }
            }

            if(allowedCameraMovement[Direction.DOWN])
            {
                if(Input.mousePosition.y <= Screen.height * downAndRightBarrier)
                { transform.Translate(-Vector3.forward*Time.deltaTime*smoothspeed,Space.World); }
            }
            
            if(allowedCameraMovement[Direction.RIGHT])
            {
                if(Input.mousePosition.x >= Screen.width * downAndRightBarrier)
                { transform.Translate(Vector3.right*Time.deltaTime*smoothspeed,Space.World); }
            }

            if(allowedCameraMovement[Direction.LEFT])
            {
                if(Input.mousePosition.x <= Screen.width * upAndLeftBarrier)
                { transform.Translate(-Vector3.right*Time.deltaTime*smoothspeed,Space.World); }
            }
        }
       

    }
}
