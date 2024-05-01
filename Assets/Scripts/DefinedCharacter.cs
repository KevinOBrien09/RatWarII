using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "DefinedCharacter")]
public class DefinedCharacter : GUIDScriptableObject
{
    public List<string> names = new List<string>();
    public string tagLine;
    public StartingStats startingStats;
    public CharacterGraphic characterGraphic;
    public Texture2D icon;
    public Sprite corpseHead;
    public Color32 bloodGradient;
    public Material bloodSplatMat;
    public CharacterAI charAI;
    public CharacterSounds sounds;
    public SkillResource.Catagory skillResCat;
}