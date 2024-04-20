using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.EventSystems;
public class Island : Room
{
    public List<BoatSlot> coastalBoatSlots = new List<BoatSlot>();


    public BoatSlot RandomBoatSlot()
    {
        SwampGeneratorBrain swampGeneratorBrain = MapGenerator.inst.brain as SwampGeneratorBrain;
        System.Random rng = new System.Random();
        List<BoatSlot> shuff =  swampGeneratorBrain.waterGrid.mainSea.OrderBy(_ => rng.Next()).ToList();
        for (int i = 0; i < shuff.Count; i++)
        {
            BoatSlot q = shuff[i];
            if(coastalBoatSlots.Contains(q))
            {return q;}
        }
        Debug.LogAssertion("CANNOT FIND A VALID RANDOM BOAT SLOT!!!");
        return coastalBoatSlots[Random.Range(0,coastalBoatSlots.Count)];
    }
}