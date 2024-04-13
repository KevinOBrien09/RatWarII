using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeatherManager : Singleton<WeatherManager>
{
    public ParticleSystem snow;
    public ParticleSystem rain;
    public Light mainLight;

    public void Snowing(Vector2 v){
     
        snow.gameObject.SetActive(true);
        var shapeModule = snow.shape;
        shapeModule.scale = new Vector3(v.x+50,100,v.y+50);
         snow.Play();

    }

    public void Raining(Vector2 v){
     
        rain.gameObject.SetActive(true);
        var shapeModule = rain.shape;
        shapeModule.scale = new Vector3(v.x+50,v.y+50,100); //y is actually z on this one
        rain.Play();

    }
}
