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
        List<SaveSlotData> l = new List<SaveSlotData>();
        for (int i = 0; i < amountOfSaveSlotsInPage; i++)
        {
            l.Add(null);
        }
        foreach (var item in saves)
        {
            if(item.slotNumber != 999){
   l.Insert(item.slotNumber,item);
            }
         
            
           
        }
      
        for (int i = 0; i < amountOfSaveSlotsInPage; i++)
        {
            SaveSlot ss = Instantiate(saveSlotPrefab,holder);
            if(l.ElementAtOrDefault(i) == null){
              
                ss.InitEmpty(i);
            }
            else{
             
                ss.Init(l[i]);
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