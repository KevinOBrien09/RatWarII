using System.Collections;
using UnityEngine;

public class QuickfireCast : SkillCastBehaviour
{
    public int baseDamage = 5;
    public int tileTravelDamage;
    public GameObject projectilePrefab;
    public SlotContents arrowSlotContent;
    bool startMoving;
    Vector3 _startPosition,targetPos;
    float _stepScale;
    public float arrowSpeed;float _progress;public float arcHeight;
    public Transform _arrow;
    CastArgs castArgs;
    int tilesTraveled;
    public override void Go(CastArgs args)
    {
        args.caster.Flip(args.targetSlot.transform.position);
        GameObject arrow = Instantiate(projectilePrefab);
        arrow.transform.position = args.caster.transform.position;
        
        CamFollow.inst.target = arrow.transform;
        if(args.target != null){
             if(args.target.healthBar  != null){
 args.target.healthBar.gameObject.transform.parent.gameObject. SetActive(true);
             } 
       
        }
        
        CamFollow.inst.Focus(arrow.transform,(()=>{
        startMoving = true;
        PlaySound(0,args.skill);
        CamFollow.inst.ChangeCameraState(CameraState.LOCK);
        }));
       
        if(args.targetSlot.node.iGridY == args.caster.slot.node.iGridY)
        {
            if(args.caster.facingRight)
            {
                arrow.transform.rotation = Quaternion.Euler(20,0,-90);
            }
            else{
                arrow.transform.rotation = Quaternion.Euler(20,0,90);
            }
        }
        else
        {
            if(args.targetSlot.node.iGridY < args.caster.slot.node.iGridY)
            {
                arrow.transform.rotation = Quaternion.Euler(90,-90,90);
              
            }
            else{
                arrow.transform.rotation = Quaternion.Euler(90,90,90);
                    
            }
        }

        
  _startPosition =   arrow.transform.position;
    castArgs = args;
    float distance = Vector3.Distance(args.caster.slot.transform.position, args.targetSlot.transform.position);
    tilesTraveled =  (int)distance/5;
    arcHeight = arcHeight * distance;
    // This is one divided by the total flight duration, to help convert it to 0-1 progress.
    _stepScale = arrowSpeed / distance;
    targetPos = new Vector3(args.targetSlot.transform.position.x,-1.4f,args.targetSlot.transform.position.z);
    _arrow = arrow.transform;
   
    //    arrow.transform.DOMove(new Vector3(args.targetSlot.transform.position.x,-1.5f,args.targetSlot.transform.position.z) ,.25f);

      

       
    }

    void Update()
    {
        if(startMoving){
            _progress = Mathf.Min(_progress + Time.deltaTime * _stepScale, 1.0f);

            // Turn this 0-1 value into a parabola that goes from 0 to 1, then back to 0.
            float parabola = 1.0f - 4.0f * (_progress - 0.5f) * (_progress - 0.5f);

            // Travel in a straight line from our start position to the target.        
            Vector3 nextPos = Vector3.Lerp(_startPosition, targetPos, _progress);

            // Then add a vertical arc in excess of this.
            nextPos.y += parabola * arcHeight;

            // Continue as before.
         //_arrow.LookAt(targetPos,  _arrow.right);
             _arrow.position = nextPos;
        }
        

      
        if(_progress == 1.0f){
            Arrived();
        }
           
    }

    public void Arrived(){
        startMoving = false;
        BattleManager.inst.   StartCoroutine(q());
        SkillAimer.inst.skillCastBehaviour = null;
        Destroy(gameObject);
        IEnumerator q()
        {
            
            if(castArgs.target == null){
               yield return new WaitForSeconds(.75f);
                CamFollow.inst.Focus(castArgs.caster.slot.transform,()=>
                { 
                    CamFollow.inst.target = castArgs.caster.transform;
                    CamFollow.inst.ChangeCameraState(CameraState.LOCK); 
                });  
            }
            else{
                int damage = baseDamage;
                for (int i = 0; i < tilesTraveled; i++)
                {
                    damage += tileTravelDamage;
                }
                PlaySound(1,castArgs.skill);
                castArgs.target.Hit(damage,castArgs);
                yield return new WaitForSeconds(.75f);
                CamFollow.inst.Focus(castArgs.caster.slot.transform,()=>
                { 
                    CamFollow.inst.target = castArgs.caster.transform;
                    CamFollow.inst.ChangeCameraState(CameraState.LOCK); 
                });  
            }
          
            yield return new WaitForSeconds(.25f);
            if(castArgs.target != null)
            {
                if(castArgs.target.healthBar != null){
                castArgs.target.healthBar.gameObject.transform.parent.gameObject. SetActive(false);
                }
      
                Destroy(_arrow.gameObject);
            }
            else
            {
                if(castArgs.targetSlot.isWater)  
                {
                    Destroy(_arrow.gameObject);
                }
                else{
                    castArgs.targetSlot.cont.AddContent(arrowSlotContent);
                    foreach (var item in _arrow.gameObject.GetComponentsInChildren<SpriteRenderer>())
                    {
                        item.sortingLayerName = "Default";
                        
                    }
                }
                
            }
            SkillAimer.inst.Finish();
        }
      
    }

}