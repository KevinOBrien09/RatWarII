using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraBoundManager : Singleton<CameraBoundManager>
{
    public GenericDictionary<Direction,Transform> dict = new GenericDictionary<Direction, Transform>();


    public void Init(Vector2 v)
    {
        Transform up = dict[Direction.UP];
        up.transform.position = new Vector3(0,up.transform.position.y,v.y/2) ;
        up.transform.localScale = new Vector3(v.x,up.transform.localScale.y,up.transform.localScale.z) ;

        Transform down = dict[Direction.DOWN];
        float dz = v.y/2;
        down.transform.position = new Vector3(0,down.transform.position.y,-dz) ;
        down.transform.localScale = new Vector3(v.x,down .transform.localScale.y,down .transform.localScale.z) ;

        Transform right = dict[Direction.RIGHT];

        right.transform.position = new Vector3(v.x/2-10,right.transform.position.y,0) ;
        right.transform.localScale = new Vector3(  right.transform.localScale.x,    right.transform.localScale.y,v.y) ;

        Transform left = dict[Direction.LEFT];
        float lx = v.x/2;
        left.transform.position = new Vector3(-lx+10,left.transform.position.y,0) ;
        left.transform.localScale = new Vector3(left.transform.localScale.x,left.transform.localScale.y,v.y) ;

		int offset =  MiscFunctions.GetPercentage((int) v.y,10);
		transform.position = new Vector3(0,31.5f,-offset);
    }
}