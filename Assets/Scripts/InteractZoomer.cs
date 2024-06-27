using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System.Linq;
using UnityEngine.Events;
using DG.Tweening;
using UnityEngine.SceneManagement;

public class InteractZoomer : Singleton<InteractZoomer>
{
    public Transform center;
    public  void Zoom(GameObject target,OverworldUnit overworldUnit){
        PartyController.inst.run = false;
        overworldUnit.agent.enabled = false;
        OverworldCamera.inst.enabled = false;
        overworldUnit.transform.SetParent(center);
        overworldUnit.transform.DOLocalMove(Vector3.zero,.2f);
    }
}