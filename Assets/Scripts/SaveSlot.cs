using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;
public class SaveSlot : MonoBehaviour
{
    public TextMeshProUGUI saveName,location,date;
    public RawImage savePic;
    public GameObject empty,hasData;
    int slotNumber;
    SaveSlotData saveSlotData;
    public Button button;
    public Texture2D screenShotFallback;
    public void Init(SaveSlotData ssd){
        hasData.SetActive(true);
        empty.SetActive(false);
        slotNumber = ssd.slotNumber;
        saveName.text = "<size=150%>"+ ssd.slotNumber +":</size> "+ ssd.saveName;
        location.text = ssd.lastGameLoc;
      
        date.text = ssd.date;
        Texture2D screenShot =  SaveLoad.LoadPNG(ssd.screenShotPath); 
        if(screenShot != null){
            savePic.texture = screenShot;
        }
        else{
            savePic.texture = screenShotFallback;
        }
       
        saveSlotData = ssd;
        if(TitleScreen.inst.currentState == TitleScreen.TitleScreenState.NEWGAME)
        {
            button.interactable = false;
        }
        else
        {
            button.interactable = true;
        }
    }

    public void InitEmpty(int i){
        hasData.SetActive(false);
        empty.SetActive(true);
        slotNumber = i+1;
     
        if(TitleScreen.inst.currentState == TitleScreen.TitleScreenState.NEWGAME)
        {
            button.interactable = true;
        }
        else
        {
            button.interactable = false;
        }
    }

    public void Click()
    {
       

        
        if(saveSlotData == null){
            Debug.Log("Save is empty!");
           
            TitleScreen.inst.NameSavePopup(slotNumber);
            
        }
        else{
            if(TitleScreen.inst.currentState == TitleScreen.TitleScreenState.NEWGAME)
            {
                Debug.Log("Save holds save :" + slotNumber + "Override?");
            }
            else{
                TitleScreen.inst.LoadScene(slotNumber);
 Debug.Log("Save holds save :" + slotNumber +" Now load");
            }
        }

    }
}