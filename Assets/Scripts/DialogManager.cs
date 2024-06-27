using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System.Linq;
using UnityEngine.Events;
using DG.Tweening;
using UnityEngine.SceneManagement;

public class DialogManager : Singleton<DialogManager>
{
    public GameObject holder;

    void Start(){
        holder.SetActive(false);
    }

}