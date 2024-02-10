using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : Singleton<AudioManager>
{
    [SerializeField] GameObject soundEffectPrefab;
    [SerializeField] int howManyObjects;
    public List<SoundEffect> soundEffects = new List<SoundEffect>();
    public Queue<SoundEffect> soundEffectQ = new Queue<SoundEffect>();
    
    protected override void Awake()
    {
        base.Awake();
        for (int i = 0; i < howManyObjects; i++)
        {
            GameObject g = Instantiate(soundEffectPrefab,transform);
            SoundEffect sfx = g.GetComponent<SoundEffect>();
            soundEffects.Add(sfx);
            soundEffectQ.Enqueue(sfx);
        }
    }

    public SoundEffect GetSoundEffect()
    {
        if(soundEffectQ.Count > 0)
        {return soundEffectQ.Dequeue();}
        else
        {
            soundEffectQ.Clear();
            foreach (var item in soundEffects)
            {soundEffectQ.Enqueue(item);}

           return soundEffectQ.Dequeue();
        }
    }
    
}
