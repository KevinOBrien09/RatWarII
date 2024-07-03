using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System.Linq;
public enum E_Action{SKILL,RUN_AWAY,RUN_TOWARD,REPOS,END}
[System.Serializable]
public class EnemyAction
{
    public E_Action action;
    public Castable castable;
}


