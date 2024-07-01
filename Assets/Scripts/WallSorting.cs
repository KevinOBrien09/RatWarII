using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;
public class WallSorting : MonoBehaviour
{
    public SortingGroup sortingGroup;
    public Transform bottomFace;
    float dist;
    void Start(){
        bottomFace.SetParent(null);
    }
    public void Update(){
        ChangeSorting();
    }

    public void ChangeSorting(){
 
        if(bottomFace.transform.position.z < PartyController.inst.leader. transform.position.z){
            sortingGroup.sortingOrder = 1;
            Debug.Log("FRONT OF PARTY");
        }
        else{
              sortingGroup.sortingOrder = -10;
              Debug.Log("BEHIND PARTY");
        }
    }
}