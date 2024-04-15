using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleUIVisibility : MonoBehaviour
{
   public CanvasGroup group;
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.F1)){
            if(group.alpha ==0){
                group.alpha = 1;
            }
            else{
                group.alpha = 0;
            }
        }
    }


}
