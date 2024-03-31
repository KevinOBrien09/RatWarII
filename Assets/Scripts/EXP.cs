using System.Collections;
using System.Collections.Generic;
using UnityEngine;



[System.Serializable]
public class EXPSave
{
    public int level = 0;
    public int currentEXP = 0;
    public int targetEXP = 0;


}

[System.Serializable]
public class EXP 
{
 
    public int level;
    public int currentExp;
    public int targetExp;
     //Character character;
    public float a = 100.0f; //300.0f
    public float b = 1.5f; //2.0f
    public float c = 7.0f; //7.0f


    
    public EXPSave Save()
    {
        EXPSave save = new EXPSave();
        save.level = level;
        save.currentEXP = currentExp;
        save.targetEXP = targetExp;
        return save;
    }

    public void Load(EXPSave save)
    {
        if(save != null)
        {
            level = save.level;
            targetExp = save.targetEXP;
            Init(save.currentEXP);
            // currentExp = save.currentEXP;
            //
        }
        else
        {
            level = 1;
            currentExp = 0;
            Init(currentExp);
        }
     
       
    }

    public void PsudeoLevel(int fakeLevel,Character ch)
    { 
        //character = ch;
        int c = GetExpForCurrentLevel(fakeLevel);
      
        Init(c);
        currentExp+= Random.Range(2,GetTargetEXP()-1);
      
    }

    public void Init(int c)
    {
        AddExp(c);
       
    }

    public void AddExp(int newExp)
    {
        currentExp += newExp;
        while(currentExp >= targetExp)
        {
            currentExp = currentExp - targetExp;
            level++;
            targetExp = GetTargetEXP();
//             if(beast != null){
//    beast.LevelUp();
//             }
         
        }
    }

    public int GetExpForCurrentLevel(int fakeLevel)
    {
        int totalExpAmount =0;
        
        for (int i = 0; i < fakeLevel; i++)
        {totalExpAmount += p(i);}

        int p (int k)
        {
            int firstPass = 0;
            int secondPass = 0;
            for (int i = 0; i < k; i++)
            {
                firstPass += (int)Mathf.Floor(i + (a * Mathf.Pow(b, i/c)));
                secondPass = firstPass/4;
            }
            return secondPass;
        }
       return totalExpAmount;
    }

    public int GetTargetEXP()
    {
        int firstPass = 0;
        int secondPass = 0;
        for (int i = 0; i < level; i++)
        {
            firstPass += (int)Mathf.Floor(i + (a * Mathf.Pow(b, i/c)));
            secondPass = firstPass/4;
        }
       return secondPass;
  
    }
}