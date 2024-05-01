using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

public class BattleZoomer : Singleton<BattleZoomer>
{
    public Transform left,right,center;
    public HealthBar leftHp,rightHP,centerHP;
    public GameObject UI,aimThing;
    public CanvasGroup group;
    public RectTransform handle;
    public Image rightIMG,leftIMG,handleIMG;
    public List<SoundData> negativeSounds = new List<SoundData>();

    public void ZoomIn(CastArgs args,UnityAction action,bool end)
    {
        
        float leftY,rightY,casterY,targetY;
        CamFollow.inst.STOPMOVING = true; 
        Transform casterParent = args.caster.transform.parent;
        Transform targetParent = args.caster.transform.parent;
        Cursor.lockState = CursorLockMode.Locked;
        (Unit left,Unit right) u = leftMostUnit(args.target,args.caster);
        leftY = u.left.transform.position.y;
        rightY = u.right.transform.position.y;
        targetY = args.target.transform.position.y;
        casterY = args.caster.transform.position.y;
        u.left.transform.SetParent(BattleZoomer.inst.left);
        u.right.transform.SetParent(BattleZoomer.inst.right);
        u.left.transform.DOLocalMove(Vector3.zero,.1f);
        u.right.transform.DOLocalMove(Vector3.zero,.1f);
        AudioManager.inst.GetSoundEffect().Play(negativeSounds[0])   ;
        BattleManager.inst.ToggleHealthBars(false);
       
        rightIMG.color = Color.black;
        leftIMG.color = Color.black;
        if(u.right.side == Side.PLAYER)
        {
            rightIMG.color = Color.white;
            handle.transform.DORotate(Vector3.zero,.25f);
        }
        if(u.left.side ==Side.PLAYER)
        {leftIMG.color = Color.white;
        handle.transform.DOLocalRotate(new Vector3(0,0,180),.25f);}
        if(args.caster.side == Side.PLAYER)
        {handleIMG.color = Color.white;}
        else
        {handleIMG.color = Color.black;}

        UnityAction leftHpAction = null;
        if(u.left.isEntity())
        {
            leftHp.gameObject.SetActive(true);
            leftHp.health = u.left.health;
            leftHp.Refresh();
            leftHpAction = ()=> {leftHp.Refresh();};
            u.left.health.onHit.AddListener(leftHpAction);  
            u.left.healthBar.transform.parent. localScale = new Vector3(1,1,1);
            u.left.transform.localScale = Vector3.one;
            foreach (var item in u.left.graphic.allRenderers)
            {item.sortingLayerName = "Zoom";}
        }
        else
        {
            leftHp.gameObject.SetActive(false);
            if(u.left.CheckType<BreakableSlot>())
            {
               u.left.transform.localRotation = Quaternion.Euler(10,0,0);
            }
           
        }
        
        UnityAction rightHpAction = null;
        if(u.right.isEntity())
        {
            rightHP.gameObject.SetActive(true);
            rightHP.health = u. right.health;
            rightHP.Refresh();
            rightHpAction = ()=> {rightHP.Refresh();};
            u. right.health.onHit.AddListener(rightHpAction);
            u.right.healthBar.transform.parent. localScale = new Vector3(-1,1,1);
            u.right.transform.localScale =   new Vector3(-1,1,1);
            foreach (var item in u.right.graphic.allRenderers)
            {item.sortingLayerName = "Zoom";}
        }
        else
        {
            rightHP.gameObject.SetActive(false);
            if(u.right.CheckType<BreakableSlot>())
            {
                u.right.transform.localRotation = Quaternion.Euler(10,0,0);
            }
           
        }
       

       
        
        centerHP.gameObject.SetActive(false);

        aimThing.gameObject.SetActive(true);
        handle.gameObject.SetActive(true);
        UI.gameObject.SetActive(true);
        group.DOFade(1,.1f);
     

       
    


        args.caster.activeUnitIndicator.gameObject.SetActive(false);

      
       
       // CamFollow.inst.ForceFOV(45);
        StartCoroutine(BeginMove());
      
        IEnumerator BeginMove()
        {
            Transform targetHolder,attackerHolder;
            Vector3 originalPos = new Vector3(); 
            bool left = true;
            targetHolder = u.left.transform.parent;
            attackerHolder = u.right.transform.parent;

            if(u.right == args.target)
            {
                attackerHolder = u.left.transform.parent;
                targetHolder = u.right.transform.parent;
                left = false;
            
            }
            originalPos = attackerHolder.localPosition;
            float a = 0;
            float b = 0;
            if(!left)
            {
                a = -7;
                b = 2f;
            }
            else{
                a = 7;
                b = -2f;
            }

        yield return new WaitForSeconds(.3f);

            attackerHolder.DOLocalMoveX(a,.15f).OnComplete(()=>
            {
                attackerHolder.DOLocalMoveX(b,.1f).OnComplete(()=>
                {
                    action.Invoke();
                
                    handle.transform.DOLocalRotate(new Vector3(0,0,-90),.25f);
                   
                    attackerHolder.DOLocalMove(originalPos,.2f).OnComplete(()=>
                    {
                        StartCoroutine(Reset());
                        IEnumerator Reset()
                        {   
                            yield return new WaitForSeconds(.5f); //This value matters, CharacterGraphic:106
                            Unit target = args.target;
                            Unit caster = args.caster;
                            bool targetAlive = false;
                            if(target.isEntity()){
                            targetAlive =   target.health.currentHealth > 0;
                            }
                          
                            bool casterAlive = caster.health.currentHealth > 0;
                       
                            AudioManager.inst.GetSoundEffect().Play(negativeSounds[1])   ;
                            if(casterAlive)
                            {
                                caster.graphic.ChangeSpriteSorting( caster.slot.node);
                                caster.transform.SetParent(casterParent);
                                caster.transform.DOMove(new Vector3(caster.slot.transform.position.x,casterY,caster.slot.transform.position.z) ,.2f);
                            }
                            if(targetAlive)
                            {
                                target.graphic.ChangeSpriteSorting( target.slot.node);
                                target.transform.SetParent(targetParent);
                                target.transform.DOMove(new Vector3(target.slot.transform.position.x,targetY,target.slot.transform.position.z) ,.2f);
                            }
                            
                            CamFollow.inst.Focus(args.caster.slot.transform,()=>
                            { CamFollow.inst.ChangeCameraState(CameraState.LOCK); });
                            //CamFollow.inst.ForceFOV( CamFollow.inst.baseFOV);
                            yield return new WaitForSeconds(.45f);
                            if(rightHpAction != null){
                                u. right.health.onHit.RemoveListener(rightHpAction);
                            }
                           
                            if(leftHpAction != null){
                                u. left.health.onHit.RemoveListener(leftHpAction);
                            }
                       
                            leftHp.health = null;
                            rightHP.health = null;
                            rightIMG.color = Color.black;
                            leftIMG.color = Color.black;  
                            group.DOFade(0,.2f).OnComplete(()=>
                            {UI.gameObject.SetActive(false);});
                          
                            if(end){
                            SkillAimer.inst.Finish();          
                            }
                           
                        }
                    });
                });
                
            });
        }
        
        (Unit left,Unit right) leftMostUnit(Unit u1,Unit u2){
            if(u1.slot.node.iGridX > u2.slot.node.iGridX)
            {
                return(u2,u1) ;
            }
            else
            { return(u1,u2) ;}
        }
    }



    public void SoloZoom(CastArgs args,UnityAction action)
    {
        CamFollow.inst.STOPMOVING = true;
        CamFollow.inst.target = args.caster.slot.transform;
        Unit u = args.caster;
        float  ogY = u.transform.position.y;
        Transform ogParent = u.transform.parent;
        u.transform.SetParent(BattleZoomer.inst.center);
        u.transform.DOLocalMove(Vector3.zero,.1f);
        u.activeUnitIndicator.gameObject.SetActive(false);

        centerHP.health = u.health;
        centerHP.Refresh();
        UnityAction HpAction = ()=> {centerHP.Refresh();};

        rightHP.gameObject.SetActive(false);
        leftHp.gameObject.SetActive(false);
        centerHP.gameObject.SetActive(true);
        aimThing.gameObject.SetActive(false);
        handle.gameObject.SetActive(false);
        UI.gameObject.SetActive(true);
        group.DOFade(1,.1f);

        foreach (var item in u.graphic.allRenderers)
        {item.sortingLayerName = "Zoom";}
        //CamFollow.inst.ForceFOV(45);
        StartCoroutine(BeginMove());
        IEnumerator BeginMove()
        {

            yield return new WaitForSeconds(.2f);
            action.Invoke();
            centerHP.Refresh();
            yield return new WaitForSeconds(.5f);


            u.transform.SetParent(ogParent);
            u.transform.DOMove(new Vector3(u.slot.transform.position.x,ogY,u.slot.transform.position.z) ,.2f).OnComplete(()=>{

              
                u.graphic.ChangeSpriteSorting(u.slot.node);
            });

            CamFollow.inst.Focus(u.slot.transform,()=>
            { CamFollow.inst.ChangeCameraState(CameraState.LOCK); });
           // CamFollow.inst.ForceFOV( CamFollow.inst.baseFOV);
            yield return new WaitForSeconds(.45f);
    
            u. health.onHit.RemoveListener(HpAction);
            // if(args.skill.ID == "35433542-3142-45a9-b3d4-93096ef99883")
            // {
            //     ParticleSystemRenderer r =   u.transform.Find("shield") .GetComponent<ParticleSystemRenderer>();
            //     r.sortingLayerName = "Unit";
            //     r.sortingOrder = u.slot.node.iGridY * 10;
            
            // }
           
            centerHP.health = null;
            group.DOFade(0,.2f).OnComplete(()=>
            {UI.gameObject.SetActive(false);});
            SkillAimer.inst.Finish();




        }


    }


}