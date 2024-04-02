
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public enum Aligment{GOOD,NEUT,BAD,NA}

[CreateAssetMenu(menuName = "Trait/BaseTrait")]
public class Trait : GUIDScriptableObject
{
    public string traitName;
    public Aligment aligment;
  [TextArea]  public string traitDesc;
    
}