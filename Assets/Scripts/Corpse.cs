using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;
using UnityEngine.Rendering.Universal;

public class Corpse : PoolableObject
{

    public SlotContents contents;
    public SpriteRenderer head;
    public ParticleSystem bloodExplosion;
    public DecalProjector decal;
    public SoundData goreSFX;
    public Sprite empty;
    public GenericDictionary<Species,Sprite> dict = new GenericDictionary<Species, Sprite>();
    Slot slot;
    public void Spawn(Unit u,Slot s)
    {
        gameObject.SetActive(true);
   
        slot = s;
        transform.position = slot.transform.position;
        if(u.side == Side.ENEMY)
        {
            if(u.enemy.corpseHead == null){
      head.sprite = empty;
            }
            else{
      head.sprite = u.enemy.corpseHead;
            }
      
            // var v = bloodExplosion.main;
            // v.startColor = new ParticleSystem.MinMaxGradient(u.enemy.bloodGradient);
            // decal.material = u.enemy.bloodSplatMat;
            
        }
        else
        {
            head.sprite = dict[u.character.species];
        }
           head.sprite = empty;
        slot.cont.slotContents.Add(contents);
        //slot.tempTerrain = this;
        bloodExplosion.Play();
        AudioManager.inst.GetSoundEffect().Play(goreSFX);
    
    }

}