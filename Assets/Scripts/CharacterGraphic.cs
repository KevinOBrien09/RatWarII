using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.Events;
using DG.Tweening;
public class CharacterGraphic : MonoBehaviour
{ 
    public Character character;
    // public GenericDictionary<Species,Sprite> female = new GenericDictionary<Species, Sprite>();
    // public GenericDictionary<Species,GenericDictionary<Job,List<Sprite>>> classVarients = new GenericDictionary<Species, GenericDictionary<Job, List<Sprite>>>();
    public GenericDictionary<Species,Vector3> camPositionDict = new GenericDictionary<Species, Vector3>();
    public List<SpriteRenderer> allRenderers = new List<SpriteRenderer>();
    public Camera cam;
    public Unit unit;
    public CharacterGraphic iconClone;
    public void Init(Character c)
    {
        Orginize(c);
        character = c;
   
       
        
    }

    public void EnemyInit(DefinedCharacter e){}

    public void Orginize(Character c)
    {
        if(c.gender == Gender.FEMALE)
        {
            if(c.job == Job.KNIGHT)
            {
                if(c.species != Species.FROG)
                {
                    if(c.spriteVarient == 3){
                        allRenderers[1].gameObject.SetActive(false);   //Female BucketHelm Knights do not have lashes"
             
                        goto spriteSetUp;
                    }
                }

            }
            allRenderers[1].gameObject.SetActive(true);
            allRenderers[1].sprite = CharacterBuilder.inst.female[c.species];
        }
        else
        {allRenderers[1].gameObject.SetActive(false);}

        spriteSetUp:
    
        allRenderers[0].sprite = CharacterBuilder.inst.classVarients[c.species][c.job][c.spriteVarient];
     
    }
   //this is stupid
    public CharacterGraphic MakeCamClone(){
    return Instantiate(this);
    }
    //this is stupid
   public void CameraShit(CharacterGraphic original)
    {
        foreach (var item in allRenderers)
        {item.gameObject.layer = 8;}
        Orginize(original. character);
        cam.transform.localPosition = camPositionDict[original.character.species];
        gameObject.name = character.characterName.fullName() + ": IconClone";
        RenderTexture texture = new RenderTexture(250,250,16);
        texture.Create();
        cam.targetTexture = texture;   
        IconGraphicHolder.inst.Take(this);
        original.cam = cam;
        original.iconClone = this;
    }
   //this is stupid
    

    public void RedFlash(UnityAction action){
        StartCoroutine(q());
        IEnumerator q()
        {
            Vector3 ogPos = transform.position;
            Tween t =  transform.DOShakePosition(.3f).OnComplete(()=>
            {
                if(!unit.inKnockback && unit.health.currentHealth > 0)
                {transform.position = ogPos;}
               
            });
            foreach (var item in allRenderers)
            {
                item.DOColor(Color.red,0);
            }
            if(unit.health.currentHealth > 0){
             action.Invoke();
            }
           
             
            yield return new WaitForSeconds(.25f); //This value matters, BattleZoomer:109
        
            if(unit.health.currentHealth > 0)
            {
                foreach (var item in allRenderers)
                {
                    item.DOColor(Color.white,1f);
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
            foreach (var item in allRenderers)
            {
                item.DOColor(Color.green,0);
            }
            action.Invoke();
            yield return new WaitForSeconds(0);
            foreach (var item in allRenderers)
            {
                item.DOColor(Color.white,1f);
            }

        }

    }

    public void WhiteFlash(UnityAction a){

        StartCoroutine(q());
        IEnumerator q()
        {

            foreach (var item in allRenderers)
            {
                item.material = unit.whiteFlash;
            }  

            yield return new WaitForSeconds(.1f);
            a.Invoke();
            foreach (var item in allRenderers)
            {
                item.material = unit.spriteDefault;
            } 
        }
    }

    public void ChangeSpriteSorting(Node n)
    {
        foreach (var item in allRenderers)
        { item.sortingLayerName = "Unit"; }
        
        foreach (var item in allRenderers)
        { item.sortingOrder = -n.iGridY; }
        
        if(unit.shieldGraphic != null)
        { unit.shieldGraphic.sortingOrder = -n.iGridY; }
    }

}
