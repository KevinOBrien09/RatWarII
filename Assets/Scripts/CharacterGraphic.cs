using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.Events;
using DG.Tweening;
public class CharacterGraphic : MonoBehaviour
{ 
    public Character character;
    public GenericDictionary<Job,GameObject> jobParents = new GenericDictionary<Job, GameObject>();
    public GameObject eyeLiner;
    public GenericDictionary<Job, GenericDictionary<ColourVarient,GameObject>> varients = new GenericDictionary<Job,GenericDictionary<ColourVarient, GameObject>>();
    public GenericDictionary<SpriteRenderer,int> rendDict = new GenericDictionary<SpriteRenderer,int>();
    public Camera cam;
    public Unit unit;
   
    public void Init(Character c)
    {
        Orginize(c);
        CharacterGraphic iconClone = Instantiate(this);
        iconClone.Orginize(c);
      
        iconClone.gameObject.name = character.characterName.fullName() + ": IconClone";
        cam = iconClone.cam;
        IconGraphicHolder.inst.Take(iconClone);
    }

    public void EnemyInit(Enemy e){
        
       // CharacterGraphic iconClone = Instantiate(this);
        foreach (var item in GetComponentsInChildren<SpriteRenderer>().ToList())
        {
            if(!rendDict.ContainsKey(item))
            {rendDict.Add(item,item.sortingOrder);}
        }
      
      

        // RenderTexture texture = new RenderTexture(250,250,16);
        // texture.Create();
        // cam.targetTexture = texture;   
        // cam.gameObject.SetActive(false);

        // iconClone.gameObject.name = character.characterName.fullName() + ": IconClone";
        // cam = iconClone.cam;

        // IconGraphicHolder.inst.Take(iconClone);
        // cam.gameObject.SetActive(false);
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
        cam.gameObject.SetActive(false);
    }

    public void RedFlash(bool dead,UnityAction action){
        StartCoroutine(q());
        IEnumerator q()
        {
            Vector3 ogPos = transform.position;
            Tween t =  transform.DOShakePosition(.3f).OnComplete(()=>
            {
                if(!dead && !unit.inKnockback){
                transform.position = ogPos;
                }
               
            });
            foreach (var item in rendDict)
            {
                item.Key.DOColor(Color.red,0);
            }
            if(!dead){
             action.Invoke();
            }
           
             
            yield return new WaitForSeconds(.25f); //This value matters, BattleZoomer:109
        
            if(!dead)
            {
                foreach (var item in rendDict)
                {
                    item.Key.DOColor(Color.white,1f);
                }
            }
            else{
                  action.Invoke();
                t.Kill();
            }
       
        }
        
    }

    public void GreenFlash(UnityAction action)
    {
        StartCoroutine(q());
        IEnumerator q()
        {
            foreach (var item in rendDict)
            {
                item.Key.DOColor(Color.green,0);
            }
            action.Invoke();
            yield return new WaitForSeconds(0);
            foreach (var item in rendDict)
            {
                item.Key.DOColor(Color.white,1f);
            }

        }

    }

    public void WhiteFlash(UnityAction a){

        StartCoroutine(q());
        IEnumerator q()
        {

            foreach (var item in rendDict)
            {
                item.Key.material = unit.whiteFlash;
            }  

            yield return new WaitForSeconds(.1f);
            a.Invoke();
            foreach (var item in rendDict)
            {
                item.Key.material = unit.spriteDefault;
            } 
        }
    }

    public void ChangeSpriteSorting(int yAxis)
    {
        foreach (var item in rendDict)
        {item.Key.sortingOrder = item.Value - yAxis;}

        if(unit.shieldGraphic != null){
            unit.shieldGraphic.sortingOrder = unit.slot.node.iGridY * 10;
        }
    }

}
