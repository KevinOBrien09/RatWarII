using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.Events;
using System.Linq;
public class InspectionResult : Singleton<InspectionResult>
{
    public TextMeshProUGUI topText;
    public List< TextMeshProUGUI> textMeshes = new List<TextMeshProUGUI>();
    public RectTransform continueArrow;
    public CanvasGroup cg;
    public Vector2 arrowPoints;
    public bool detectClick;
    public SoundData find,clickSFX;
    public Image image;
    UnityAction unityAction;
    void Start(){
        cg.alpha = 0;
    }
    public void LoadItems(List<Item> items, UnityAction a){
        if(items.Count > 0)
        {
            image.enabled = true;
            image.sprite = items[0].icon;

            foreach (var item in textMeshes)
            { item.enabled = false; }
            Dictionary<string,List<Item>> d = new Dictionary<string, List<Item>>();
            foreach (var item in items)
            {
                
                if(d.ContainsKey(item.ID))
                { d[item.ID].Add(item); }
                else
                {
                    d.Add(item.ID,new List<Item>());
                    d[item.ID].Add(item);
                }
            }
            
            for (int i = 0; i < d.Count; i++)
            {
                textMeshes[i].enabled = true;
                textMeshes[i].text = d.ElementAt(i).Value[0].itemName[GameManager.inst.language] +" x"+ d.ElementAt(i).Value.Count;
            }

            topText.text = PartyController.inst.selected.battleUnit.character.characterName.firstName + " Found Something!";
        }
        else
        {
            topText.text = "Found Nothing...";
            foreach (var item in textMeshes)
            { item.enabled = false; }
            image.enabled = false;
        }
        
        continueArrow.gameObject.SetActive(false);
        //continueArrow.anchoredPosition = new Vector2(0,-270);
        AudioManager.inst.GetSoundEffect().Play(find);
        cg.DOFade(1,.5f).OnComplete(()=>{
            
            StartCoroutine(q());
            IEnumerator q(){
                yield return new WaitForSeconds(.2f);
                continueArrow.gameObject.SetActive(true);
                ContinueArrowLoop();
                detectClick = true;
                unityAction = a;
            }
        });
    }

    void Update(){
        if(detectClick){
            if(InputManager.inst.player.GetAnyButtonDown()){
                if(unityAction!= null){
                     AudioManager.inst.GetSoundEffect().Play(clickSFX);
                    unityAction.Invoke();
                    detectClick = false;
                    cg.DOFade(0,.5f);
                }
            }
        }
    }

    public void ContinueArrowLoop(){
        continueArrow.DOAnchorPosX(arrowPoints.y,.3f).OnComplete(()=>{

            continueArrow.DOAnchorPosX(arrowPoints.x,.3f).OnComplete(()=>{
            ContinueArrowLoop();
            });
        });
    }
}