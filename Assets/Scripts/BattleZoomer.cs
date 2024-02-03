using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;
 

public class BattleZoomer : Singleton<BattleZoomer>
{
    public Transform left,right;
    public HealthBar leftHp,rightHP;
    public GameObject UI;
    public CanvasGroup group;
    public RectTransform handle;
    public Image rightIMG,leftIMG,handleIMG;

    public  void ZoomIn(CastArgs args,UnityAction action)
    {
        float leftY,rightY,casterY,targetY;
        CamFollow.inst.STOPMOVING = true; 
        
        (Unit left,Unit right) u = leftMostUnit(args.target,args.caster);
        leftY = u.left.transform.position.y;
        rightY = u.right.transform.position.y;
        targetY = args.target.transform.position.y;
        casterY = args.caster.transform.position.y;
        u.left.transform.SetParent(BattleZoomer.inst.left);
        u.right.transform.SetParent(BattleZoomer.inst.right);
        u.left.transform.DOLocalMove(Vector3.zero,.1f);
        u.right.transform.DOLocalMove(Vector3.zero,.1f);

        BattleManager.inst.ToggleHealthBars(false);
        leftHp.gameObject.SetActive(true);
        rightIMG.color = Color.black;
        leftIMG.color = Color.black;
        if(u.right.side ==Side.PLAYER)
        {
            rightIMG.color = Color.white;
            handle.transform.DORotate(Vector3.zero,.25f);
        }
        if(u.left.side ==Side.PLAYER)
        {leftIMG.color = Color.white;handle.transform.DOLocalRotate(new Vector3(0,0,180),.25f);}
        if(args.caster.side == Side.PLAYER)
        {handleIMG.color = Color.white;}
        else
        {handleIMG.color = Color.black;}
        leftHp.health = u.left.health;
        leftHp.Refresh();
        u.left.health.onHit.AddListener(()=>
        {leftHp.Refresh();});
        rightHP.health = u. right.health;
        rightHP.Refresh();
        u. right.health.onHit.AddListener(()=>
        { rightHP.Refresh();});
      
        UI.gameObject.SetActive(true);
        group.DOFade(1,.1f);
     
        u.right.transform.localScale =   new Vector3(-1,1,1);
        u.right.healthBar.transform.parent. localScale = new Vector3(-1,1,1);
        u.left.healthBar.transform.parent. localScale = new Vector3(1,1,1);
        u.left.transform.localScale = Vector3.one;

        args.caster.activeUnitIndicator.gameObject.SetActive(false);

        foreach (var item in u.left.graphic.rendDict)
        {item.Key.sortingLayerName = "Zoom";}
        foreach (var item in u.right.graphic.rendDict)
        {item.Key.sortingLayerName = "Zoom";}
        CamFollow.inst.ForceFOV(45);
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
                            
                            bool targetAlive = args.target.health.currentHealth > 0;
                            bool casterAlive = args.caster.health.currentHealth > 0;
                            Unit target = args.target;
                            Unit caster = args.caster;
                   
                            if(casterAlive)
                            {
                                foreach (var item in caster.graphic.rendDict)
                                {item.Key.sortingLayerName = "Unit";}
                                caster.graphic.ChangeSpriteSorting( caster.slot.node.iGridY);
                                caster.transform.SetParent(null);
                                caster.transform.DOMove(new Vector3(caster.slot.transform.position.x,casterY,caster.slot.transform.position.z) ,.2f);
                            }
                            if(targetAlive)
                            {
                          
                                foreach (var item in target.graphic.rendDict)
                                {item.Key.sortingLayerName = "Unit";}
                                target.graphic.ChangeSpriteSorting( target.slot.node.iGridY);
                                target.transform.SetParent(null);
                                target.transform.DOMove(new Vector3(target.slot.transform.position.x,targetY,target.slot.transform.position.z) ,.2f);
                            }
                            
                            CamFollow.inst.Focus(args.caster.slot.transform,()=>
                            { CamFollow.inst.ChangeCameraState(CameraState.LOCK); });
                            CamFollow.inst.ForceFOV( CamFollow.inst.baseFOV);
                            yield return new WaitForSeconds(.45f);
                            leftHp.health = null;
                            rightHP.health = null;
                            rightIMG.color = Color.black;
                            leftIMG.color = Color.black;  group.DOFade(0,.2f).OnComplete(()=>
                            {UI.gameObject.SetActive(false);});
                          

                            SkillAimer.inst.Finish();
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



}