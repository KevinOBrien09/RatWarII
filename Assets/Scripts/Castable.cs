using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Castable : GUIDScriptableObject
{
    public GenericDictionary<Language,string> itemName = new GenericDictionary<Language, string>();
    public GenericDictionary<Language,TextAreaString> itemDesc = new GenericDictionary<Language,TextAreaString>();
    public SkillCastBehaviour skillCastBehaviour;
    public List<int> value = new List<int>();
    public Sprite statusEffectIcon;
    public List<SoundData> sounds = new List<SoundData>();

    public string GetName()
    {
        if(GameManager.inst != null)
        {
            if(itemName.ContainsKey(GameManager.inst.language))
            {return itemName[GameManager.inst.language];}   
            else
            {
                Debug.LogAssertion(GameManager.inst.language + " IS NOT IN THE DICTIONARY FOR" + name);
                return "EMPTY";
            }
        }
        else{
            Debug.LogAssertion("GM IS NULL CANNOT WORK!");
            return itemName[Language.ENG];
        }
    }

    public string GetDesc(){
        if(GameManager.inst != null)
        {
            if(itemDesc.ContainsKey(GameManager.inst.language))
            {return itemDesc[GameManager.inst.language].str;}   
            else
            {
                Debug.LogAssertion(GameManager.inst.language + " IS NOT IN THE DICTIONARY FOR" + name);
                return "EMPTY";
            }
        }
        else{
            Debug.LogAssertion("GM IS NULL CANNOT WORK!");
            return itemDesc[Language.ENG].str;
        }
    }
}
[System.Serializable]
public class TextAreaString{
    [TextArea(10,10)] public string str;
}