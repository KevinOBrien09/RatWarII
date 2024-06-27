
// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
// using UnityEngine.EventSystems;
// using DG.Tweening;

// public class ZoomCameraSwap : Singleton<ZoomCameraSwap>
// {
//     public Vector3 offSet;
//     void Start(){
//         Attach(OverworldCamera.inst.gameObject);
//     }
//     public void Attach(GameObject parent){
//         transform.SetParent(null);
//         transform.position = Vector3.zero;
//         transform.SetParent(parent.transform);
//         transform.localPosition = offSet;
//     }

// }