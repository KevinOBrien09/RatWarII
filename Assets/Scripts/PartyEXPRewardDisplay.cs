using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;


public class PartyEXPRewardDisplay:MonoBehaviour
{
    public Image beastPic,expBarFill;
    public TextMeshProUGUI level,exp,gainedExp,levelUptxt,beastName;
    public RectTransform levelUpTextRT;
    public SoundData expTick,levelUpTick;
   Character character;
    public void Init(Character c){
        //    levelUptxt.DOFade(0,0);
        // BeastData data = b.scriptableObject.beastData;
        // beastPic.sprite = data.uiPicture;
        // if(data.facingRight)
        // {
        //     beastPic.transform.rotation = Quaternion.Euler(0,0,0);
        // }
        // else{
        //     beastPic.transform.rotation = Quaternion.Euler(0,180,0);
        // }
        // expBarFill.DOFillAmount((float)b.exp.currentExp/(float)b.exp.targetExp,0);
        // beastName.text = data.beastName;
        // level.text = "Lvl." + b.exp.level.ToString();
        // exp.text = "NEXT:"  + b.exp.targetExp;
        // beast = b;
    }

    public void SimulateAddExp(int newEXP,EXP expCopy)
    {
        int l = newEXP;
        int o = expCopy.currentExp;
        int oldLvl = expCopy.level;
        bool playSound = true;
        StartCoroutine(add());
        StartCoroutine(sound());
      
        IEnumerator add()
        {
            yield return null;
           
            gainedExp.text = "+"+ l.ToString(); 
            int NEXT = expCopy.targetExp-o;
            exp.text = "NEXT:"  + NEXT;
            o++;
            l--;
            expCopy .AddExp(1);
            expBarFill.fillAmount = (float) expCopy.currentExp/(float)expCopy.targetExp;
            if(expCopy.level > oldLvl )
            {
                AudioManager.inst.GetSoundEffect().Play(levelUpTick);
                levelUptxt.DOFade(1,.25F);
                levelUpTextRT.DOAnchorPosY(40,.5f).OnComplete(()=>
                {
                    StartCoroutine(d());
                    IEnumerator d()
                    {
                        yield return new WaitForSeconds(.5f);
                        levelUptxt.DOFade(0,.25F).OnComplete(()=>
                        {levelUpTextRT.DOAnchorPosY(-9,0);});
                    }
                });

                oldLvl = expCopy.level;
                level.text = "Lvl." + expCopy .level.ToString();
                o=0;
                Debug.Log("level up");
            }
           
            //DOFillAmount((float)beast.exp.currentExp/(float)beast.exp.targetExp,0);
            if(l >= 0){
                StartCoroutine(add());
            } 
            else{
                playSound = false;
                    
            }
        }

        IEnumerator sound()
        {
            AudioManager.inst.GetSoundEffect().Play(expTick);
            yield return new WaitForSeconds(expTick.audioClip.length+.1f);
            expTick.pitchRange.x = expTick.pitchRange.x +.01f;
            expTick.pitchRange.y = expTick.pitchRange.y +.01f;
            expTick.pitchRange.x =Mathf.Clamp(expTick.pitchRange.x ,0,1);
                 expTick.pitchRange.y =Mathf.Clamp(expTick.pitchRange.y ,0,1);
          if(playSound){
    StartCoroutine(sound());
          }
          else{
expTick.pitchRange.x = .1f;
expTick.pitchRange.y = .1f;
          }
        
        }
        
       
     
    }

}