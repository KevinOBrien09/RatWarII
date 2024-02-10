using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SoundData
{
    public AudioClip audioClip;
    public Vector2 pitchRange = new Vector2();
    public float volume;
}


public class SoundEffect : MonoBehaviour
{
    [SerializeField] AudioSource source;
    public void Play(SoundData sd)
    {
        if(sd != null)
        {
            source.clip = sd.audioClip;
            source.pitch = GetPitch(sd.pitchRange.x,sd.pitchRange.y);
            source.volume = sd.volume;
            source.Play();
        }
        else
        {
            Debug.LogWarning("No SoundEffect");
        }
     
    }

    public float GetPitch(float low,float high)
    {
        return Random.Range(low,high);
    }
}
