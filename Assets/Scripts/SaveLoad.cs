using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;
using System;



public static class SaveLoad
{
    public const string saveSlot = "/saveSlot.json";
    public const string mainSave = "/mainSave.json";
    public const string settings = "/settings.txt";
    public static string savePath = Application.persistentDataPath+"/saves/";


    public static void Save(int slot,SaveData sd = null,bool takePhoto = false)
    {
        if(!SaveAlreadyExists(savePath + slot.ToString()))
        {
            CreateSaveFile(slot,"THIS SHOULDN'T HAPPEN IN BUILD LOL");
        }
        
        string mainDataPath = savePath + slot + mainSave;
        int currentScene = SceneManager.GetActiveScene().buildIndex;
        SaveData mainData =  null;
        if(sd != null)
        {mainData = sd;}
        else
        {mainData =  GameManager.inst.Save();}
        string mainDataToJSON = JsonUtility.ToJson(mainData,true);
        File.WriteAllText(mainDataPath , mainDataToJSON); 

        string saveSlotPath =  savePath + slot + saveSlot;
        string saveSlotDataJSON = File.ReadAllText(saveSlotPath);
        SaveSlotData saveSlotData = JsonUtility.FromJson<SaveSlotData>(saveSlotDataJSON);
        FileInfo f = new FileInfo(saveSlotPath);
        saveSlotData.date = GetTime(f);
        if(takePhoto){
 saveSlotData.screenShotPath = TakeScreenShot(slot.ToString());
        }
       
        saveSlotData.lastGameLoc = LocationManager.inst.locName;
        string saveSlotDataBackToJSON = JsonUtility.ToJson(saveSlotData,true);
        File.WriteAllText(saveSlotPath, saveSlotDataBackToJSON); 
        
        Debug.Log("Saved to : "+ mainDataPath);
    }

    public static string TakeScreenShot(string saveName)
    {
        string sneed = savePath + saveName + "/" + saveName + "_SaveDisplay.png";
        ScreenCapture.CaptureScreenshot(sneed);
        return sneed;
    }

    public static string GetTime(FileInfo f)
    {
        string d = f.LastWriteTimeUtc.ToShortDateString();
        string t = f.LastWriteTime.ToLongTimeString();
        string td = d + " : " + t;
        return td;
    }
    
    public static SaveData Load(int path)
    {
        string dir = savePath + path.ToString() + mainSave;
        if(File.Exists(dir))
        {
            
            string json = File.ReadAllText(dir);
            var Data = JsonUtility.FromJson<SaveData>(json);
            Debug.Log("Loaded from : "+ path);
            return Data;
        }
        else
        {
            Debug.LogError("File Not Found.");
            return null;
        }
    }

    public static bool AtLeastOneSave()
    { 
        if(Directory.Exists(savePath))
        { 
            var directories = Directory.GetDirectories(savePath);
            if(directories.Length > 0)
            {return true;}
        }
        return false; 
    }

    public static bool SaveHolderExists(){
        return Directory.Exists(savePath);
    }

    public static bool SaveAlreadyExists(string dir)
    { return Directory.Exists(dir); }

    public static SaveSlotData CreateSaveFile(int slotNumber,string saveName)
    {
        if(!Directory.Exists(savePath)) //Folder that holds all save files.
        {Directory.CreateDirectory(savePath);}
        
        string dir = savePath+slotNumber.ToString(); //Instance of a save file.

        if(!Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
            string mainPath = dir + mainSave;
            string saveSlotPath = dir + saveSlot;
            string settingPath = dir+settings;
            
            File.Create(mainPath).Dispose();
            SaveData mainSaveData = new SaveData();
            string mainSaveToJSON = JsonUtility.ToJson(mainSaveData,true);
            File.WriteAllText(mainPath, mainSaveToJSON);

            File.Create(settingPath).Dispose();
            PlayerSettings playerSettingData = new PlayerSettings();
            string playerSettingDataToJSON = JsonUtility.ToJson(playerSettingData,true);
            File.WriteAllText(settingPath,playerSettingDataToJSON);
            
            File.Create(saveSlotPath).Dispose();
            FileInfo f = new FileInfo(saveSlotPath);
            
            string sceneName = SceneManager.GetActiveScene().name;

            SaveSlotData ssd = new SaveSlotData(slotNumber,GetTime(f),TakeScreenShot(slotNumber.ToString()));
            ssd.saveName = saveName;
            string saveSlotDataToJSON = JsonUtility.ToJson(ssd,true);
            File.WriteAllText(saveSlotPath, saveSlotDataToJSON);
       
            Debug.Log("Save made at:" + dir);
          
            return ssd;
        }
        else
        {
            Debug.LogWarning("Save slot at " + dir + " already exists! New save has not been created.");
            return null;
        }
    }

    public static List<SaveSlotData> GetSaveSlots()
    {
        List<SaveSlotData> slotDatas = new List<SaveSlotData>();
        if(Directory.Exists(savePath))
        {
            DirectoryInfo directoryInfo = new DirectoryInfo(savePath);
            string[] folders = Directory.GetDirectories(savePath);
            if(folders.Length > 0)
            {
                foreach (var item in folders)
                {
                    string d = Path.GetFileName( Path.GetDirectoryName(item + "/"));
                    string dPath = savePath + d  + saveSlot;
                    if(File.Exists(dPath))
                    {
                        string json = File.ReadAllText(dPath);
                        SaveSlotData data = JsonUtility.FromJson<SaveSlotData>(json);
                        slotDatas.Add(data);
                    }
                    else
                    {Debug.LogAssertion("No slot data found");}
                }
            }
            else
            { Debug.LogWarning("No save slots found!"); }
        }

        return slotDatas;
    }

    public static Texture2D LoadPNG(string filePath) {

        Texture2D tex = null;
        byte[] fileData;

        if (File.Exists(filePath)) 	{
            fileData = File.ReadAllBytes(filePath);
            tex = new Texture2D(2, 2);
            tex.LoadImage(fileData); //..this will auto-resize the texture dimensions.
        }
        return tex;
    }
}