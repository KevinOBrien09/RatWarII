using UnityEngine;
using System;

[Serializable]
public class SaveSlotData
{
    public string saveName;
    public int slotNumber;
    public string date;
    public string lastGameLoc;
    public string screenShotPath;

 
    public SaveSlotData(int newSlotNumber,string newDate,string screenShot)
    {
        slotNumber = newSlotNumber;
        date = newDate;
        screenShotPath = screenShot;
     
    }
    
}