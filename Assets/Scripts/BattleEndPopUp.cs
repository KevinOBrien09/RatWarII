using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.Events;
using DG.Tweening;
using UnityEngine.SceneManagement;

public class BattleEndPopUp : Singleton<BattleEndPopUp>
{
    public GameObject holder;
    public AudioClip music;
    void Start()
    {
        holder.SetActive(false);
    }
    
    public void Show()
    {
        holder.SetActive(true);
        MusicManager.inst.FadeAndChange(music);
    }

    public void ReturnToOverworld()
    {
        holder.SetActive(false);
        BattleManager.inst.LeaveBattle();
    }
}