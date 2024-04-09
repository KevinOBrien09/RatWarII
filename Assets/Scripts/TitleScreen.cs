using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;
using UnityEngine.SceneManagement;
public class TitleScreen : Singleton<TitleScreen>
{
    public enum TitleScreenState{TITLEFADEIN,PRESS_START,MENU,NEWGAME,LOADGAME,CREDITS,NAMING_SAVE}
    public TitleScreenState currentState;
    public AudioClip intro;
    public SoundData bang;
    public AudioClip endSection;
    public AudioClip fullSong;
    public bool pressedStart;
    public RectTransform logo,map;
    public Image mapHider;
    public Vector3 logoStartPos,logoEndPos;
    public float mapStart,mapEnd;
    public GameObject pressStartText,optionHolder,saveHolder,nameSavePopup,creditsGO;
    public Button loadGameButton,returnButton,creditButton;
    public SaveSlotHandler saveSlotHandler;
    Tween t;
    public float logoTravelTime,mapTravelTime;
    public TMP_InputField inputField;

    public int saveToLoad;
    public CanvasGroup loadingScreen;
    void Start()
    {  
        GameManager.inst.Wipe();
        saveToLoad = -1;
        saveHolder.SetActive(false);
        optionHolder.SetActive(false);
        nameSavePopup.SetActive(false);
        pressStartText.SetActive(false);
        creditsGO.SetActive(false);
        returnButton.gameObject.SetActive(false);
        MusicManager.inst.ChangeMusic(fullSong);
        logo.DOAnchorPos3D(logoStartPos,0);
        mapHider.DOFade(1,0);
       
        t = logo.DOAnchorPos3D(logoEndPos,logoTravelTime).OnComplete(()=>{
            AudioManager.inst.GetSoundEffect().Play(bang);
            mapHider.DOFade(.85f,.5f);
            MapLoop();
            pressStartText.SetActive(true);
            currentState = TitleScreenState.PRESS_START;
            MusicManager.inst.FadeAndChange(endSection);
         
            pressedStart = true;
           
         
        });
    }

    public void MapLoop(){
        map.DOAnchorPosX(mapStart,0);
        map.DOAnchorPosX(mapEnd,mapTravelTime);
    }

    public void SkipZoom(){
        pressedStart = true;
        t.Complete();
        
    }

    public void Update(){
      
        if(InputManager.inst.player.GetAnyButtonDown() )
        {
        if(currentState == TitleScreenState.TITLEFADEIN)
        {
            if(!pressedStart)
            {SkipZoom();}
        }
        else if(currentState == TitleScreenState.PRESS_START){
            BringUpOptions();
        }
                
             
            
        }
    }

    public void BringUpOptions()
    {
        optionHolder.SetActive(true);
        pressStartText.SetActive(false);
        currentState = TitleScreenState.MENU;

        

        if(SaveLoad.AtLeastOneSave())
        {
            loadGameButton.gameObject.SetActive(true);
            creditButton.gameObject.SetActive(true);
        }
        else{
            loadGameButton.gameObject.SetActive(false);
            creditButton.gameObject.SetActive(false);
        }
    }

    public void QuitToDesktop(){
        Application.Quit();
    }

    public void NewGame()
    {
        saveHolder.SetActive(true);
        optionHolder.SetActive(false);
        returnButton.gameObject.SetActive(true);
        currentState = TitleScreenState.NEWGAME;
        saveSlotHandler.NewGame();
        
        Debug.Log("new game");
    }

    public void LoadSave(){
        saveHolder.SetActive(true);
        optionHolder.SetActive(false);
        returnButton.gameObject.SetActive(true);
        currentState = TitleScreenState.LOADGAME;
        saveSlotHandler.LoadGame();
        Debug.Log("load game");
    }

    public void LoadScene(int i){
        GameManager.inst.saveSlotIndex = i;
        float fadeTime = 1;
        MusicManager.inst.FadeToSilence(fadeTime);
        loadingScreen.DOFade(1,.5f).OnComplete(()=>{
            StartCoroutine(q());
            IEnumerator q(){
               yield return new WaitForSeconds(1+.5f);
               SceneManager.LoadScene("Hub");
            }
            Debug.Log("Load");
        });
    }

    public void Credits(){
        saveHolder.SetActive(true);
        optionHolder.SetActive(false);
        returnButton.gameObject.SetActive(true);
        creditsGO.SetActive(true);
        currentState = TitleScreenState.CREDITS;
    }

    public void ReturnButton(){
        switch (currentState)
        {
            case TitleScreenState.NEWGAME:
            saveHolder.SetActive(false);
            optionHolder.SetActive(true);      
            creditsGO.SetActive(false);
            returnButton.gameObject.SetActive(false);
            saveSlotHandler.Exit();
            currentState = TitleScreenState.MENU;

            return;

            case TitleScreenState.NAMING_SAVE:
            nameSavePopup.SetActive(false);      
            creditsGO.SetActive(false);
            saveToLoad = -1;
            currentState = TitleScreenState.NEWGAME;
            break;

            case TitleScreenState.LOADGAME:
            saveHolder.SetActive(false);
            optionHolder.SetActive(true);      creditsGO.SetActive(false);
            returnButton.gameObject.SetActive(false);
            saveSlotHandler.Exit();
            currentState = TitleScreenState.MENU;

            return;

            case TitleScreenState.CREDITS:
            saveHolder.SetActive(false);
            optionHolder.SetActive(true);
            creditsGO.SetActive(false);
            returnButton.gameObject.SetActive(false);
            currentState = TitleScreenState.MENU;
            break;
            
            default:


            return;
        }
    }   


    public void SubmitNewSave(){
        if(inputField.text != string.Empty)
        {
            GameManager.inst.saveSlotIndex = saveToLoad;
            SaveLoad.CreateSaveFile(saveToLoad,inputField.text);
            StartCoroutine(q());
            IEnumerator q(){
                yield return new WaitForSeconds(.1f);
                SaveLoad.Save(GameManager.inst.saveSlotIndex);
                LoadScene(GameManager.inst.saveSlotIndex);
            }
            Debug.Log("LOAD GAME NOW!!!");
        }
        else{
            Debug.Log("Error noise");
        }
       
    }

    public void NameSavePopup(int saveIndex){
        saveToLoad = saveIndex;
        currentState = TitleScreenState.NAMING_SAVE;
        nameSavePopup.SetActive(true);
    }
    
   

    // public void LoadGame()
    // {
    //     Debug.Log("new game");
    // }

    
}
