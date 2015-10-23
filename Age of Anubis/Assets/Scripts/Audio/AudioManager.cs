using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Audio;

[System.Serializable]
public struct AudioStruct
{
	public AudioClip clip;
	public float volume;
	public bool preview;
};

public class AudioManager : MonoBehaviour 
{
	public static AudioManager Inst;
	public AudioMixer am;
	[Header("Music")]
	public AudioMixerGroup g_fx;
	public AudioMixerGroup g_music;

	public AudioMixerSnapshot s_fight;
	public AudioMixerSnapshot s_idle;
	public AudioMixerSnapshot s_shop;
	[Range(0f,10f)]
	public float m_standardTransitionTime = 1.0f;



	[Header("SoundFX")]
	public int m_numberOfSources = 5;
	public List<AudioSource> m_sources;
	private int m_curSource = 0;

	[Header("Tracks")]
	public AudioStruct a_coin;
	public AudioStruct a_poison;
	public AudioStruct a_burnt;
	public AudioStruct a_bleed;
	public AudioStruct a_cut;
	public AudioStruct a_doorOpen;
	public AudioStruct a_doorShut;
	public AudioStruct a_frozen;
	public AudioStruct a_giveDamage;
	public AudioStruct a_takeDamage;
	public AudioStruct a_lowHealth;
	public AudioStruct a_purchaseItem;
	public AudioStruct a_stab;
	public AudioStruct a_thump;
	public AudioStruct a_pickupWeapon;

	void Awake()
	{
		if (Inst == null)
			Inst = this;
		else
			Destroy(gameObject);
	}

	// Use this for initialization
	public void Start () 
	{
		if (m_sources.Count > 0)
			ClearSources();
		m_sources = new List<AudioSource>();

		for (int i = 0; i < m_numberOfSources; i ++)
		{
			m_sources.Add(gameObject.AddComponent<AudioSource>());
		}

		foreach (var s in m_sources)
		{
			s.outputAudioMixerGroup = g_fx;
			s.playOnAwake = false;
			s.loop = false;
		}
	}

	public void FadeMusic(AudioMixerSnapshot snap)
	{
		FadeMusic(snap, m_standardTransitionTime);
	}


	public void FadeMusic(AudioMixerSnapshot snap, float duration)
	{
		snap.TransitionTo(duration);
	}

	public void PlaySFX(AudioStruct au)
	{
		m_sources[m_curSource].clip = au.clip;
		m_sources[m_curSource].volume = au.volume;
		m_sources[m_curSource].Play();
		NextSource();
	}

	public void PreviewSounds()
	{
		List<AudioStruct> allStructs = new List<AudioStruct>();
		allStructs.Add(a_coin);
		allStructs.Add(a_poison);
		allStructs.Add(a_burnt);
		allStructs.Add(a_bleed);
		allStructs.Add(a_cut);
		allStructs.Add(a_doorOpen);
		allStructs.Add(a_doorShut);
		allStructs.Add(a_frozen);
		allStructs.Add(a_giveDamage);
		allStructs.Add(a_takeDamage);
		allStructs.Add(a_lowHealth);
		allStructs.Add(a_purchaseItem);
		allStructs.Add(a_stab);
		allStructs.Add(a_thump);
		allStructs.Add(a_pickupWeapon);


		foreach (var st in allStructs)
		{
			if (st.preview)
				PlaySFX(st);
		}
	}

	public void ClearSources()
	{
		for (int i = 0; i < m_sources.Count; i++)
		{
			AudioSource temp = m_sources[i];
			m_sources.Remove(temp);
			DestroyImmediate(temp);
			i--;
		}
	}

	void NextSource()
	{
		m_curSource++;

		if (m_curSource == m_numberOfSources)
			m_curSource = 0;
	}
}
