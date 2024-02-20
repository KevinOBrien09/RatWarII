using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;
using System.Linq;


public class ObjectiveProgressIndicator : Singleton<ObjectiveProgressIndicator>
{
    public TextMeshProUGUI txt;
    public float shown,hidden;
    public RectTransform rt;
    public void Show(string s)
    {
        rt.DOAnchorPosX(shown,.25f).OnComplete(()=>{

            StartCoroutine(q());
            IEnumerator q(){
            yield return new WaitForSeconds(1);
              rt.DOAnchorPosX(hidden,.25f);
            }

        });
        txt.text = s;
    }
}