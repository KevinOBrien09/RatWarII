using UnityEngine;
using System.Collections;

public class CameraShake : Singleton<CameraShake>
{
    public AnimationCurve curve;

 
    
    public void Shake(float dur,float strength){
        StartCoroutine(q());
        IEnumerator q(){
            Vector3 s = transform.localPosition;
            float elapsedTime = 0;
            while(elapsedTime < dur){
                elapsedTime += Time.deltaTime;
                
                //curve.Evaluate(elapsedTime / dur);
                transform.localPosition = s + Random.insideUnitSphere * strength;
                yield return null;
            }

            transform.localPosition = s;
        }
    }
}