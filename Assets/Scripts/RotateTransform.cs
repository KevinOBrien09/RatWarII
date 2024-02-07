using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateTransform : MonoBehaviour
{
    public float speed;
    public Vector3 rot = new Vector3 (0,20,0);
    void Update()
    {
        transform.Rotate (rot * speed * Time.deltaTime);
    }
}

