using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;
using UnityEngine.Video;
using UnityEngine.UI;
using System.Linq;
using UnityEngine.EventSystems;
public class SkillWindow : Singleton<SkillWindow>
{
    public TextMeshProUGUI skillName,skilDesc;
    public GenericDictionary<Skill,VideoClip> clipDict = new GenericDictionary<Skill,VideoClip>();
    public VideoPlayer videoPlayer;
    public RawImage renderTexture;
    public RenderTexture videoText;
    public Texture2D placeholder;
    public VideoClip fallback;
    public Image blackFade;
    void Start(){
        Close();
    }

    public void Open(Skill s,Character c){
        DOTween.Kill(gameObject);
        gameObject.SetActive(true);
        skillName.text = s.skillName;
        skilDesc.text = SkillParser.Parse(s.desc,c:c);
       
        blackFade.DOFade(1,0);
        videoPlayer.clip = null;
        StartCoroutine(q());
        IEnumerator q()
        {
            if(clipDict.ContainsKey(s))
            {
          
            videoPlayer.clip = clipDict[s];
            videoPlayer.Play();
            renderTexture.texture = videoText;

            }
            else
            {
                videoPlayer.clip = fallback;
                videoPlayer.Play();
                renderTexture.texture = videoText;
            }
            yield return new WaitForSeconds(.1f);
            blackFade.DOFade(0,1f);
       
        }
      
    }

    public void Close(){
        blackFade.DOFade(1,0);
        gameObject.SetActive(false);
    }

}