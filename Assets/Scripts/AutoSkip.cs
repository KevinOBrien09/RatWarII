using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using TMPro;

public class AutoSkip : Singleton<AutoSkip>
{
    public TextMeshProUGUI text;
    public bool autoSkip;
    public SoundData toggle;
    public GameObject arrows;
    void Start(){
        text.gameObject.SetActive(false);
        autoSkip = false;
        arrows.SetActive(false);
    }
    public void Update()
    {
        if( InputManager.inst.player.GetButtonDown("AutoSkip")){
            if(autoSkip)
            {
                text.gameObject.SetActive(false);
                AudioManager.inst.GetSoundEffect().Play(toggle);
                autoSkip = false;
                arrows.SetActive(false);
            }
            else
            {
                text.gameObject.SetActive(true);
                AudioManager.inst.GetSoundEffect().Play(toggle);
                autoSkip = true;
                arrows.SetActive(true);
            }
        }
       
    }
}