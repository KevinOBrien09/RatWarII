using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using DG.Tweening;



public class PerlinGeneratorBrain : MapGeneratorBrain
{
     public GenericDictionary<Vector2,int>  perlinGrid = new GenericDictionary<Vector2, int>();
    public float scale = 30;
    public virtual   void GeneratePerlin(Vector2 mainGrid)
    {
 
        // Set up the texture and a Color array to hold pixels during processing.
      
        float randomorg = Random.Range(0, 100);
        int Y =  (int) mainGrid.y;
        int X = (int) mainGrid.x;
      //  
        int howManyTiles = MapManager.inst.map.NodeArray.Length;
        var dampener = Y;
        if(X<Y){
            dampener = X;
        }
        foreach (var item in MapManager.inst.map.NodeArray)
        {
            
            float xCoord = randomorg + (float)item.iGridX/dampener*scale;
            float yCoord = randomorg +  (float)item.iGridY/dampener*scale;
            float sample = Mathf.PerlinNoise(xCoord, yCoord);
            if (sample == Mathf.Clamp(sample, 0, 0.5f))
            perlinGrid.Add(new Vector2(item.iGridX,item.iGridY),0);
            else if (sample == Mathf.Clamp(sample, 0.5f, 0.7f))
            perlinGrid.Add(new Vector2(item.iGridX,item.iGridY),1);
            else if (sample == Mathf.Clamp(sample, 0.7f, 1f))
            perlinGrid.Add(new Vector2(item.iGridX,item.iGridY),2);
            else
            perlinGrid.Add(new Vector2(item.iGridX,item.iGridY),3);
        }
    }
}