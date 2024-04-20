using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.ProBuilder;
using UnityEngine.ProBuilder.MeshOperations;
public class MapBounds : MonoBehaviour

{
    public ProBuilderMesh shape;
    public MeshRenderer mesh;
    public GenericDictionary<int,Transform> backdropAnchors = new GenericDictionary<int, Transform>();
    public void Init(Vector2 v,Material m)
    {
        transform.localScale = new Vector3(v.x,1,v.y);
        mesh.material = m;
    }

}