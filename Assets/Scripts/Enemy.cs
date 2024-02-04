using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "Enemy")]
public class Enemy : GUIDScriptableObject
{
    public List<string> names = new List<string>();
    public string tagLine;
    public StartingStats startingStats;
    public CharacterGraphic characterGraphic;
    public Texture icon;
}