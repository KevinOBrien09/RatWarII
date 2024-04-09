using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using DG.Tweening;
public class SettingsMenu : MonoBehaviour
{
    public GameObject menu,settings,mainHolder;
    public Button gear;
    public bool open;
    public void Open()
    {
        mainHolder.SetActive(true);
        settings.SetActive(false);
        menu.SetActive(true);
        open = true;
    }   

    public void Leave(){
        mainHolder.SetActive(true);
        settings.SetActive(false);
        menu.SetActive(false);
        open = false;
    }

    public void QuitToDesktop(){
        Application.Quit();
    }

    public void QuitToTitleScreen()
    {
        HubCharacterDisplay.inst.fandfLogo.gameObject.SetActive(true);
        HubCharacterDisplay.inst.fandfLogo.DOFade(1,2f);
        MusicManager.inst.FadeToSilence();
        GameManager.inst.Wipe();
        BlackFade.inst.FadeInEvent(()=>
        {
            StartCoroutine(q());
            IEnumerator q(){
                
                yield return new WaitForSeconds(.5f);
                MusicManager.inst.ResetVol();
                SceneManager.LoadScene("MainMenu");
            }
        });

    }

    public void OpenOptionsMenu(){
        mainHolder.SetActive(false);
        settings.SetActive(true);
    }

    public void ReturnFromSettings(){
        mainHolder.SetActive(true);
        settings.SetActive(false);
    }

    void Update()
    {
        // if(InputManager.inst.player.GetButtonDown("Pause"))
        // {
        //    Toggle();

        // }

    }

    public void Toggle()
    {
        if(open)
        { Leave(); }
        else
        { Open(); }

    }
}
