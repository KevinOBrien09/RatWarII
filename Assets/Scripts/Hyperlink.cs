using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;
public class Hyperlink : MonoBehaviour
{
   
    public void OpenURL(string link){
         Application.OpenURL(link);
    }


}