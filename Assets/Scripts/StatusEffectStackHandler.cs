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
    public GenericDictionary<StatusEffectEnum,Sprite> statusEffectSprites = new GenericDictionary<StatusEffectEnum, Sprite>();
    Dictionary<StatusEffectEnum,StatusEffectStack> d = new Dictionary<StatusEffectEnum, StatusEffectStack>();
    public void Spawn(Unit u)
    {
        foreach (var l in  u.statusEffects)
        {
            foreach (var item in l.Value)
            {
    
                if(d.ContainsKey(item.statusEffectEnum))
                {
                    d[item.statusEffectEnum].Stack();
                }
                else
                {
                    StatusEffectStack statusEffectStack = Instantiate(stackPrefab,statusEffectHolder);
                    statusEffectStack.Init(statusEffectSprites[item.statusEffectEnum]);
                    stackList.Add(statusEffectStack);
                    d.Add(item.statusEffectEnum,statusEffectStack);
                }
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