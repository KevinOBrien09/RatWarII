using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.Events;
using DG.Tweening;
using UnityEngine.SceneManagement;

public class ClickArrows : Singleton<ClickArrows>
{
    public ParticleSystem pSystem;
    public void Move(Vector3 point){
        pSystem.gameObject.SetActive(true);
        transform.position = point;
        pSystem.Stop();
        pSystem.Play();
        
    }

    
}