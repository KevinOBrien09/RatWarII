using UnityEngine;
using System;

[Serializable]
public class SaveSlotData
{
    public int slotNumber;
    public string date;
    public string screenShotPath;

 
    public SaveSlotData(int newSlotNumber,string newDate,string screenShot)
    {
        slotNumber = newSlotNumber;
        date = newDate;
        screenShotPath = screenShot;
     
    }
    
}