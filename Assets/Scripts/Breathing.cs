using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
public class Breathing : MonoBehaviour
{
    public float full = 1;
    public float min = .95f;
    public float inhaleTime,pauseTime,exhaleTime;
    IEnumerator Start()
    {
        yield return new WaitForSeconds(Random.Range(.1f,1.5f));
        bool b =    Random.Range(0,2) == 1;
        if(b){
    Breathe();
        }
        else{
            transform.DOScaleY(min,0).OnComplete(()=>
            {
                StartCoroutine(q());
                IEnumerator q(){
                    yield return new WaitForSeconds(inhaleTime);
                    transform.DOScaleY(full,inhaleTime).OnComplete(()=>
                    {
                        StartCoroutine(w());
                        IEnumerator w(){
                            yield return new WaitForSeconds(exhaleTime);
                            Breathe();
                        }
                    });
                }
            });
        }
    
    }

    public void Breathe()
    {
        
        transform.DOScaleY(min,exhaleTime).OnComplete(()=>
        {
            StartCoroutine(q());
            IEnumerator q(){
                yield return new WaitForSeconds(inhaleTime);
                transform.DOScaleY(full,inhaleTime).OnComplete(()=>
                {
                    StartCoroutine(w());
                    IEnumerator w(){
                        yield return new WaitForSeconds(exhaleTime);
                        Breathe();
                    }
                });
            }
        });
    }

  
}
