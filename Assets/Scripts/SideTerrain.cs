using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.EventSystems;
using System.Linq;
using UnityEngine.Rendering;
public class SideTerrain : MonoBehaviour
{
    public GameObject mainBush;
    public GameObject flowers;
    public List<GameObject> deco = new List<GameObject>();
    public void Init()
    {
        mainBush.transform.rotation = Quaternion.Euler(  mainBush.transform.rotation .eulerAngles.x,Random.Range(0,360), mainBush.transform.rotation .eulerAngles.z);
        if(MiscFunctions.FiftyFifty()){
            flowers.SetActive(true);
              flowers.transform.rotation = Quaternion.Euler(   flowers.transform.rotation.eulerAngles.x,Random.Range(0,360),  flowers.transform.rotation.eulerAngles.z);
        }
        if(Random.Range(0,100) <= 40)
        {
            int i = Random.Range(0,deco.Count);
            GameObject g = deco[i];
            g.SetActive(true);
            if(i == 5){
                mainBush.SetActive(false);
            }
            g.transform.rotation = Quaternion.Euler(  g.transform.rotation.eulerAngles.x,Random.Range(0,360), g.transform.rotation.eulerAngles.z);
        }
    }
}