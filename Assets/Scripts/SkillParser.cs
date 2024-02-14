using System.Collections.Generic;
using UnityEngine;


public static class SkillParser
{

    public static string Parse(string raw,Unit u)
    {
        string s = raw;
        
        if(s.Contains("{STR"))
        {
            string poo =  Between(s,"{","}");
            string foo = poo;
            foo = foo.Replace("{",string.Empty);
            foo = foo.Replace("}",string.Empty);
            foo = foo.Replace("STR",string.Empty);
            foo = foo.Replace("%",string.Empty);
            int percent = int.Parse(foo);
            int amount = (int) MiscFunctions.GetPercentage(u.stats().strength,percent); 
            s = s.Replace(poo,"<color=orange>" + amount.ToString() + "</color>");
        }

       

        return s;
    }

    public static string Between(string STR , string FirstString, string LastString)
    {       
        string FinalString;     
        int Pos1 = STR.IndexOf(FirstString) + FirstString.Length-1;
        int Pos2 = STR.IndexOf(LastString)+1;
        FinalString = STR.Substring(Pos1, Pos2 - Pos1);
        return FinalString;
    }

}