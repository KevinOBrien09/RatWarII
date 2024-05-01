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
    public GenericDictionary<StatusEffectEnum,Texture2D> statusEffectSprites = new GenericDictionary<StatusEffectEnum,Texture2D>();
    Dictionary<StatusEffectEnum,StatusEffectStack> d = new Dictionary<StatusEffectEnum, StatusEffectStack>();
    public void Spawn(Unit u)
    {
        foreach (var l in  u.statusEffects)
        {
            if(l.Key != StatusEffectEnum.STATMOD){
 foreach (var item in l.Value)
            {
    
                if(d.ContainsKey(item.statusEffectEnum))
                {
                    d[item.statusEffectEnum].Stack();
                }
                else
                {
                    StatusEffectStack statusEffectStack = Instantiate(stackPrefab,statusEffectHolder);
                    string info = StatusEffects.StatusEffectInfo(item.statusEffectEnum,u);
                    string n = MiscFunctions.FirstLetterToUpper( item.statusEffectEnum.ToString());
                    statusEffectStack.Init(statusEffectSprites[item.statusEffectEnum],null,n,info);
                    stackList.Add(statusEffectStack);
                    d.Add(item.statusEffectEnum,statusEffectStack);
                }
            }
            }
            else{
                
            }
           
            
        }
    }

    public void SlotContents(List<SlotContents> slotContents)
    {
        Dictionary<SlotContents,StatusEffectStack> c = new Dictionary<SlotContents,StatusEffectStack>();
        foreach (var l in  slotContents)
        {
            if(c.ContainsKey(l))
            {  c[l].Stack(); }
            else
            {   StatusEffectStack statusEffectStack = Instantiate(stackPrefab,statusEffectHolder);
                statusEffectStack.Init(l.picture,l);
                stackList.Add(statusEffectStack);
                c.Add(l,statusEffectStack);
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