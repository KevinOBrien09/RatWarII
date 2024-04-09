using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.EventSystems;
using System.Linq;
public class Wall : MonoBehaviour
{
    public MeshRenderer mesh;

    public void ChangeMat(Material m){
        mesh.material = m;
    }
}