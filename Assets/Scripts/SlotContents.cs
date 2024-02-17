using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "SlotContents")]
public class SlotContents : ScriptableObject
{
    public string contentName;
   [TextArea(10,10)] public string contentDesc;
    public Texture2D picture;
    public bool unStackable;

}