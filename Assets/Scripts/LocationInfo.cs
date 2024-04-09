using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

[CreateAssetMenu(menuName = "WorldTile")]
public class LocationInfo :ScriptableObject
{
    public string locationName;
    public LocationStage stage;
    public Sprite locationPic;
    public WorldLocationDeco decoPrefab;
    public AudioClip locationMusic;
    public MapGeneratorBrain brain;
    public Vector2 mapSize = new Vector2(250,250);
    public SoundData travelSting;
    [TextArea(10,10)] public string desc;


 
}