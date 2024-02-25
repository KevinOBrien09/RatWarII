using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System.Linq;
public class DirectionIndicator : Singleton<DirectionIndicator>
{
  	public LineRenderer lineRenderer;
    public void Reset()
    {
		lineRenderer.positionCount = 0;
		List<Vector3> v = new List<Vector3>();
       	lineRenderer.SetPositions(v.ToArray());
       	lineRenderer.enabled = false;
	}
	
	public void GetLayout(List<Node> nodes)
    {
        Reset();
        lineRenderer.enabled = true;
        List<Vector3> v = new List<Vector3>();
		foreach (var item in nodes)
        { v.Add(new Vector3( item.slot.transform.position.x,-1, item.slot.transform.position.z));}
        lineRenderer .positionCount = v.Count;
       	lineRenderer.SetPositions(v.ToArray());
	}
    

}