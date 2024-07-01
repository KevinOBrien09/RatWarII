using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using DG.Tweening;
using System.Collections;

public class EatBlackberryCast : SkillCastBehaviour
{
    public int healValue;
    public override void Go(CastArgs args)
    {
        StartCoroutine(Q());
        IEnumerator Q()
        {
            AudioManager.inst.GetSoundEffect().Play(args.caster.sounds.eat);
            yield return new WaitForSeconds(args.caster.sounds.eat.audioClip.length);
            args.caster.Heal(healValue);
            yield return new WaitForSeconds(.5f);
            SkillAimer.inst.Finish(); 
        }
    }
}