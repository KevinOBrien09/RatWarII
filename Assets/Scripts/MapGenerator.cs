using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class MapGenerator : Singleton<MapGenerator>
{
    public AStar generatorAstar;
    public Grid_ genGrid;
    public bool generating;
    public MapGeneratorBrain brain;
    protected override void Awake()
    {
        base.Awake();
        genGrid.CreateGrid();
    }
    
    public void BeginGeneration(MapGeneratorBrain newBrain)
    {
        if(newBrain != null){
            generating = true;
            brain = Instantiate(newBrain);
            brain.Generate();
        }
        else{
            Debug.LogAssertion("CANNOT GENERATE NO MAP DATA!!!");
        }
   
    }

    public void WrapUp()
    {
        generating = false;
        GameManager.inst.GameSetUp();
        Destroy(brain.gameObject);
        Destroy(gameObject);
    }
}