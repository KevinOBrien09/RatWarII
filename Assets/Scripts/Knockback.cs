using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using DG.Tweening;


public static class Knockback
{
    public static bool Hit(int howManyTiles,Unit attacker,Unit victim,bool stun)
    {
            int casterX = attacker.slot.node.iGridX;
            int casterY = attacker.slot.node.iGridY;
            int targetX = victim.slot.node.iGridX;
            int targetY =victim.slot.node.iGridY;

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
                v = new Vector2(targetX-howManyTiles,targetY+howManyTiles);
                break;
                case 8:
                v = new Vector2(targetX,targetY+howManyTiles);
                break;
                case 9:
                v = new Vector2(targetX+howManyTiles,targetY+howManyTiles);
                break;
                case 4:
                v = new Vector2(targetX-howManyTiles,targetY);
                break;
                case 5:
                Debug.LogAssertion("SAME SLOT");
                break;
                case 6:
                v = new Vector2(targetX+howManyTiles,targetY);
                break;
                case 1:
                v = new Vector2(targetX-howManyTiles,targetY-howManyTiles);
                break;
                case 2:
                v = new Vector2(targetX,targetY-howManyTiles);
                break;
                case 3:
                v = new Vector2(targetX+howManyTiles,targetY-howManyTiles);
                break;
            }

            bool validKnockback = MapManager.inst.nodeIsValid(v);
            Unit u = victim; 
            if(validKnockback)
            {    Slot newSlot =  MapManager.inst.map.NodeArray[(int)v.x,(int)v.y].slot;
                if(newSlot != null)
                {
                    
                  
                    if(newSlot.cont.wall)
                    {
                        Stun(stun);
                        return false;
                    }
                    else if(newSlot.cont.unit != null)
                    {
                        newSlot.cont.unit.Stun();      
                        Stun(stun);
                        return false;
                    }
                    else
                    {
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
                            
                            overShoot = new Vector3(p.x+2.5f,y,p.z-2.5f);
                            break;
                        }
                        u.Reposition(newSlot);
                        victim.inKnockback = true;
                        u.transform.DOMove(overShoot,.2f).OnComplete(()=>{
                        u.transform.DOMove(new Vector3(p.x,y,p.z),.2f);});
                        Debug.Log("Valid Knockback"); 
                        return true;
                    }
                }
                else{
                       Stun(stun);   return false;
                }

            }
            else
            {   Debug.Log("Stun! (Map Border)"); 

                Stun(stun);
                return false;
            }


        void Stun(bool stun)
        {
            Vector3 overShoot = new Vector3();
            Vector3 overShoot2 = new Vector3();
            Vector3  p = u.slot. transform.position;
            float y  = u.transform.position.y;
            float kb = 2.5f;
            switch (direction)
            {
                case 7:
                overShoot = new Vector3( p.x-kb,y,p.z+kb);
                break;
                case 8:
                overShoot = new Vector3(p.x,y,p.z+kb);
                break;
                case 9:
                overShoot = new Vector3(p.x+kb,y,p.z+kb);
                break;
                case 4:
                overShoot = new Vector3(p.x-kb,y,p.z);
                break;
                case 5:
                Debug.LogAssertion("SAME SLOT");
                break;
                case 6:
                overShoot = new Vector3(p.x+kb,y,p.z);
                break;
                case 1:
                overShoot = new Vector3(p.x-kb,y,p.z-kb);
                break;
                case 2:
                overShoot = new Vector3(p.x,y,p.z-kb);
                break;
                case 3:
                overShoot = new Vector3(p.x+kb,y,p.z-kb);
                break;
            }



                switch (direction)
                {
                    case 7:
                    overShoot2 = new Vector3(p.x+kb,y,p.z-kb); //
                    break;
                    case 8:
                        overShoot2 = new Vector3(p.x,y,p.z-kb);
                    break;
                    case 9:
                    overShoot2 = new Vector3(p.x-kb,y,p.z-kb);//
                    break;
                    case 4:
                        overShoot2 = new Vector3(p.x+kb,y,p.z);  //
                    break;
                    case 5:
                    Debug.LogAssertion("SAME SLOT");
                    break;
                    case 6:
                    overShoot2 = new Vector3(p.x-kb,y,p.z);//
                    break;
                    case 1:
                    overShoot2 = new Vector3(p.x+kb,y,p.z+kb);  //
                    break;
                    case 2:
                    overShoot2 = new Vector3(p.x,y,p.z+kb);
                    break;
                    case 3:
                        overShoot2 = new Vector3( p.x-kb,y,p.z+kb);//
                    break;
                }


                

                u.transform.DOMove(overShoot,.1f).OnComplete(()=>
                { 
                    BattleManager.inst.StartCoroutine(s());
                    IEnumerator s()
                    {
                        if(stun)
                        {
                            u.Stun();
                            yield return new WaitForSeconds(.1f); //wallslam
                        }
                    
                    
                    u.transform.DOMove(overShoot2,.2f).OnComplete(()=>
                    {
                        u.transform.DOMove(new Vector3(u.slot.transform.position.x,y,u.slot.transform.position.z) ,.2f);
                    });
                    
                    
                    
                    
                            
                    }
                });

    }


    }

}