using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System.Linq;
public class MantisAI : CharacterAI
{
    public Skill reave,heal;
    public override void ConductTurn()
    {
        StartCoroutine(q());
        IEnumerator q()
        { 
            loc = GetSlot();
            yield return new WaitForSeconds(.1f);
            if(enemyState == EnemyState.CHASING)
            {
                if(canCast(reave))
                {
                    CastSkill(reave);
                }
                else
                {
                    Debug.Log("Not In Range");
                    Move();
                    while(aiMove)
                    {yield return null;}
                    yield return new WaitForSeconds(.1f);
                    if(canCast(reave))
                    {
                        CastSkill(reave);
                    }
                    else{
                        BattleManager.inst. UnitIteration();
                    }

                    
                }
            }
            else if(enemyState == EnemyState.RETREAT)
            {
                Move();
                while(aiMove)
                {yield return null;}
                yield return new WaitForSeconds(.1f);
                if(canCast(heal))
                {
                    CastSkill(heal);
                }
                else{
                    BattleManager.inst. UnitIteration();
                }


            }
            
        }
    }

}