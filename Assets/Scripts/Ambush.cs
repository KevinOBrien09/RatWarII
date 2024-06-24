using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Ambush")]
public class Ambush : ScriptableObject

{
    public List<AmbushData> datas = new List<AmbushData>();
}

[System.Serializable]
public class AmbushData{
    public BattlePosition battlePosition;
    public DefinedCharacter enemy;
}