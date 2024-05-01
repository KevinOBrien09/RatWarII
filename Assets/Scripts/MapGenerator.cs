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
    public LocationInfo testing;
    protected override void Awake()
    {
        base.Awake();
        genGrid.CreateGrid();
    }
    
    public void BeginGeneration(LocationInfo LI)
    {
        if(LI != null){
            genGrid.ResizeGrid(LI.mapSize);
            MapManager.inst.map.ResizeGrid(LI.mapSize);
            generating = true;
            brain = Instantiate(LI.brain);
       
            brain.Generate(LI);
        }
        // else if(brain != null){
        //     generating = true;
        //     brain = Instantiate(brain);
        //     brain.Generate();
        // }
        else
        {
            Debug.LogAssertion("CANNOT GENERATE NO MAP DATA!!!");
        }
   
    }

    public void WrapUp()
    {
        generating = false;
        GameManager.inst.GameSetUp();
        // Destroy(brain.gameObject);
        // Destroy(gameObject);
    }
}