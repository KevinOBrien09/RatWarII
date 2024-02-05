using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StatusEffectStackHandler : MonoBehaviour
{ 
    public RectTransform  statusEffectHolder;
    public StatusEffectStack stackPrefab;
    List<StatusEffectStack> stackList = new List<StatusEffectStack>();
             Dictionary<string,StatusEffectStack> d = new Dictionary<string, StatusEffectStack>();
    public void Spawn(Unit u)
    {
        foreach (var item in u.statusEffects)
        {
   
            if(d.ContainsKey(item.skill.ID))
            {
                d[item.skill.ID].Stack();
            }
            else
            {
                StatusEffectStack statusEffectStack = Instantiate(stackPrefab,statusEffectHolder);
                statusEffectStack.Init(item.skill);
                stackList.Add(statusEffectStack);
                d.Add(item.skill.ID,statusEffectStack);
            }
        }
    }

    public void Kill()
    {
        foreach (var item in stackList)
        {Destroy(item.gameObject);}
        stackList.Clear();
        d.Clear();
    }
}