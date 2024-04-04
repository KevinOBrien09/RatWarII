using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;
using System.Linq;
public class SaveSlotHandler : MonoBehaviour
{
    public int amountOfSaveSlotsInPage;
    public TextMeshProUGUI instruction;
    public List<SaveSlot> saveSlotInstances = new List<SaveSlot>();
    public SaveSlot saveSlotPrefab;
    public Transform holder;
    public void NewGame()
    {
        Exit();
        instruction.text = "Create a save!";
        SpawnSlots();
    }

    public void LoadGame(){
        Exit();
        instruction.text = "Load a save.";
        SpawnSlots();
    }

    public void SpawnSlots(){
      
        List<SaveSlotData> saves = SaveLoad.GetSaveSlots(); //redo
      
        for (int i = 0; i < amountOfSaveSlotsInPage; i++)
        {
            SaveSlot ss = Instantiate(saveSlotPrefab,holder);
            if(saves.ElementAtOrDefault(i) == null){
              
                ss.InitEmpty(i);
            }
            else{
             
                ss.Init(saves[i]);
            }
            saveSlotInstances.Add(ss);
            
        }
    }

    public void Exit()
    {
        foreach (var item in saveSlotInstances)
        {Destroy(item.gameObject); }
        saveSlotInstances.Clear();
    }
}