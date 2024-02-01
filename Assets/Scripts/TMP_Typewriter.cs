using DG.Tweening;
using System;
using TMPro;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public  class TMP_Typewriter : MonoBehaviour
{
	public bool typing;
	public TMP_Text m_textUI = null;
	string m_parsedText;
	Action m_onComplete;
	[SerializeField] AudioSource source;
	Tween m_tween;

	Queue<char> q = new Queue<char>();
	bool blip;
	
	void Reset()
	{m_textUI = GetComponent<TMP_Text>();}
	
	void OnDestroy()
	{
		m_tween?.Kill();

		m_tween = null;
		m_onComplete = null;
	}
	
	public void Play( string text, float speed, Action onComplete,AudioClip clip = null )
	{
	
		m_textUI.text = text;
		m_onComplete = onComplete;
		typing = true;
		m_textUI.ForceMeshUpdate();
		

	
		m_parsedText = m_textUI.GetParsedText();

		var length = m_parsedText.Length;
		var duration = 1 / speed * length;

		OnUpdate( 0 );

		m_tween?.Kill();
		m_tween = DOTween
			.To( value => OnUpdate( value,clip ), 0, 1, duration )
			.SetEase( Ease.Linear )
			.OnComplete( () => OnComplete() )
		;
	}

	// IEnumerator Voice(AudioClip clip)
	// {
	// 	blip = true;
	// 	if(!typing)
	// 	{yield break;}

		
	// 	yield return new WaitForSeconds(.05f);
	// 	if(clip != null)
	// 	{ Sound.inst.Play(clip,false,.5f); }
	// 	blip = false;	
	// }
	
	public void Skip( bool withCallbacks = true )
	{
		m_tween?.Kill();
		m_tween = null;

		OnUpdate( 1 );

		if ( !withCallbacks ) return;

		m_onComplete?.Invoke();
		m_onComplete = null;
	}
	
	public void Pause()
	{
		m_tween?.Pause();
	}
	public void Resume()
	{
		m_tween?.Play();
	}
	
	void OnUpdate( float value,AudioClip clip = null )
	{	
		var current = Mathf.Lerp( 0, m_parsedText.Length, value );
		var count = Mathf.FloorToInt( current );
		
		// if(clip != null)
		// { 
		// 	StartCoroutine(Voice(clip));
		// }
		
		m_textUI.maxVisibleCharacters = count;
	}
	
	void OnComplete()
	{
		
		typing = false;
		m_tween = null;
		m_onComplete?.Invoke();
		m_onComplete = null;
	}
}
