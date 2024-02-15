using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "Skill/Projectile")]
public class ProjectileSkill : Skill
{
   
    public enum ProjectilePathShape{PLUS,X,HORI,VERT,ASTERISK}
    public bool passThrough,goThroughWalls;
    public ProjectilePathShape projectilePath;
    public int howManyTiles;
}