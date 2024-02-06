using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using DG.Tweening;


public class KnockbackCast : SkillCastBehaviour
{
  
    public override void Go(CastArgs args)
    {
        StartCoroutine(q());
        IEnumerator q()
        {
         
            int casterX = args.caster.slot.node.iGridX;
            int casterY = args.caster.slot.node.iGridY;
            int targetX = args.target.slot.node.iGridX;
            int targetY = args.target.slot.node.iGridY;

            bool casterAbove = casterY > targetY;
            bool casterBelow = casterY < targetY;
            bool casterRight = casterX > targetX;
            bool casterLeft = casterX < targetX;

            int direction = 5;

            if(casterAbove && !casterRight && !casterLeft) //DOWN
            { direction = 2; }
            else if(casterBelow && !casterRight && !casterLeft) //UP
            { direction = 8; }
            if(casterAbove && casterRight && !casterLeft) //DOWN LEFT
            { direction = 1; }
            if(casterAbove && !casterRight && casterLeft) //DOWN RIGHT
            { direction = 3; }
            else if(casterBelow && casterRight && !casterLeft) //UP LEFT
            { direction = 7; }
            else if(casterBelow && !casterRight && casterLeft) //UP RIGHT
            { direction = 9; }
            else if(!casterAbove && !casterBelow) 
            {
                if(casterRight) //LEFT
                { direction = 4; }
                else if(casterLeft) //RIGHT
                { direction = 6; }
            }


            Vector2 v = new Vector2();
     
            switch (direction)
            {
                case 7:
                v = new Vector2(targetX-1,targetY+1);
                break;
                case 8:
                v = new Vector2(targetX,targetY+1);
                break;
                case 9:
                v = new Vector2(targetX+1,targetY+1);
                break;
                case 4:
                v = new Vector2(targetX-1,targetY);
                break;
                case 5:
                Debug.LogAssertion("SAME SLOT");
                break;
                case 6:
                v = new Vector2(targetX+1,targetY);
                break;
                case 1:
                v = new Vector2(targetX-1,targetY-1);
                break;
                case 2:
                v = new Vector2(targetX,targetY-1);
                break;
                case 3:
                v = new Vector2(targetX+1,targetY-1);
                break;
            }

            bool validKnockback = false;
            int maxX = MapManager.inst.grid.iGridSizeX-1;
            int maxY = MapManager.inst.grid.iGridSizeY-1;

            bool inXRange = v.x <= maxX && v.x >= 0;
            bool inYRange = v.y <= maxY && v.y >= 0;
            validKnockback = inXRange && inYRange;
            Debug.Log(targetX+ ":" + targetY);
            Debug.Log(v);
          
            if(validKnockback)
            {  
                Slot newSlot = MapManager.inst.grid.NodeArray[(int)v.x,(int)v.y].slot;
                if(newSlot != null)
                {
                    if(newSlot.unit == null)
                    {
                        Unit u = args.target; 
                        float y  = u.transform.position.y;
                        Vector3 p = newSlot.transform.position;
                        Vector3 overShoot = new Vector3();
                        switch (direction)
                        {
                            case 7:
                            overShoot = new Vector3(p.x-2.5f,y,p.z+2.5f);
                            break;
                            case 8:
                            overShoot = new Vector3(p.x,y,p.z+2.5f);
                            break;
                            case 9:
                            overShoot = new Vector3(p.x+2.5f,y,p.z+2.5f);
                            break;
                            case 4:
                            overShoot = new Vector3(p.x-2.5f,y,p.z);
                            break;
                            case 5:
                            Debug.LogAssertion("SAME SLOT");
                            break;
                            case 6:
                             overShoot = new Vector3(p.x+2.5f,y,p.z);
                            break;
                            case 1:
                            overShoot = new Vector3(p.x-2.5f,y,p.z-2.5f);
                            break;
                            case 2:
                            overShoot = new Vector3(p.x,y,p.z-2.5f);
                            break;
                            case 3:
                            v = new Vector2(targetX+1,targetY-1);
                            overShoot = new Vector3(p.x+2.5f,y,p.z-2.5f);
                            break;
                        }
                        u.Reposition(newSlot);
                        u.transform.DOMove(overShoot,.2f).OnComplete(()=>{
                        u.transform.DOMove(new Vector3(p.x,y,p.z),.2f);


                        });
                        Debug.Log("Valid Knockback"); 

                        yield return new WaitForSeconds(.3f);
                    }
                    else{
                        Debug.Log("Double Stun! (Unit)");
                    }
                 
                }
                else
            { Debug.Log("Stun! (Wall)"); }
            
            }
            else
            { Debug.Log("Stun! (Map Border)"); }

       
        
            
            yield return new WaitForSeconds(.2f);
            SkillAimer.inst.Skip();
        }
    }

  


}