using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
public class CharacterGraphic : MonoBehaviour
{ 
    public Character character;
    public GenericDictionary<Job,GameObject> jobParents = new GenericDictionary<Job, GameObject>();
    public GameObject eyeLiner;
    public GenericDictionary<Job, GenericDictionary<ColourVarient,GameObject>> varients = new GenericDictionary<Job,GenericDictionary<ColourVarient, GameObject>>();
    public GenericDictionary<SpriteRenderer,int> rendDict = new GenericDictionary<SpriteRenderer,int>();
    public Camera cam;
   
    public void Init(Character c)
    {
      
        
        Orginize(c);
        CharacterGraphic iconClone = Instantiate(this);
        iconClone.Orginize(c);
      
        iconClone.gameObject.name = character.characterName.fullName() + ": IconClone";
        cam = iconClone.cam;
        IconGraphicHolder.inst.Take(iconClone);
       
     
    }
    public void Orginize(Character c)
    {
        List<GameObject> fuckYou = new List<GameObject>();
        GameObject jobParent = jobParents[c.job];
        jobParent.SetActive(true);
        foreach (var item in jobParents)
        {
            if(item.Key != c.job)
            {fuckYou.Add(item.Value.gameObject);}
        }

        if(c.gender == Gender.FEMALE)
        {eyeLiner.SetActive(true); }
        else
        {fuckYou.Add(eyeLiner.gameObject);}

        GameObject accessory = varients[c.job][c.colourVarient];
        accessory.SetActive(true);

        foreach (var item in varients[c.job])
        {
            if(item.Key != c.colourVarient)
            {fuckYou.Add(item.Value.gameObject);}
        }


        foreach (var item in GetComponentsInChildren<SpriteRenderer>().ToList())
        {
            if(!fuckYou.Contains(item.gameObject))
            {
                if(!rendDict.ContainsKey(item)){
                    rendDict.Add(item,item.sortingOrder);
                }
              
            }
         
     
        }
       
        foreach (var item in fuckYou)
        {Destroy(item);}

          RenderTexture texture = new RenderTexture(250,250,16);
        texture.Create();
        cam.targetTexture = texture;   
    }

    public void ChangeSpriteSorting(int yAxis)
    {
        foreach (var item in rendDict)
        {item.Key.sortingOrder = item.Value - yAxis;}
    }

}
