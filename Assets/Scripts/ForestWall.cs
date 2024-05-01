using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.EventSystems;
using System.Linq;
public class ForestWall : Wall
{
    public List<GameObject> gos = new List<GameObject>();
    public void ApplyDeco(){
        foreach (var item in gos)
        {
            item.SetActive(false);
        }
        gos[Random.Range(0,gos.Count)].SetActive(true);
    }
}