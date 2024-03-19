using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class GoldText : Singleton<GoldText>
{
    public TextMeshProUGUI txt;

    public void Refresh(){
        //txt.text = "G:" + Party.inst.gold.ToString();
    }

}