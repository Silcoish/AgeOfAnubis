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

	public AudioMixerSnapshot s_none;
	public AudioMixerSnapshot s_fight;
	public AudioMixerSnapshot s_idle;
	public AudioMixerSnapshot s_shop;
    public AudioMixerSnapshot s_boss;
	public AudioMixerSnapshot s_temple;
	public AudioMixerSnapshot s_rumble;
	[Range(0f,10f)]
	public float m_standardTransitionTime = 1.0f;



	[Header("SoundFX")]
	public int m_numberOfSources = 5;
	public List<AudioSource> m_sources;
	private int m_curSource = 0;

	[Header("Tracks")]
    [Header("Misc")]
    public AudioStruct a_bleed;
    public AudioStruct a_burnt;
    public AudioStruct a_coin;
    public AudioStruct a_cut;
    public AudioStruct a_doorOpen;
    public AudioStruct a_doorShut;
    public AudioStruct a_frozen;
    public AudioStruct a_poison;
    public AudioStruct a_stab;
    public AudioStruct a_thump;
	public AudioStruct a_healthPot;
    [Header("Player")]
    public AudioStruct a_player_death;
    public AudioStruct a_player_jump;
    public AudioStruct a_player_land;
	public AudioStruct a_player_takeDamage;
	public AudioStruct a_player_step;
    [Header("Enemy")]
    public AudioStruct a_eny_die;
    public AudioStruct a_eny_spit;
	public AudioStruct a_eny_snake_charge;
    public AudioStruct a_eny_takeDamage;
    [Header("UI")]
    public AudioStruct a_ui_purchase;
    public AudioStruct a_ui_cancel;
    public AudioStruct a_ui_confirm;
    public AudioStruct a_ui_select;
    [Header("Boss")]
    public AudioStruct a_anubis_bash;
	public AudioStruct a_anubis_dashCharge;
    public AudioStruct a_anubis_dash;
    public AudioStruct a_anubis_fireball;
    public AudioStruct a_anubis_intro;
    public AudioStruct a_anubis_takeDamage;



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

	public void Update()
	{
		if(Input.GetKeyDown(KeyCode.Q))
			PlaySFX(a_coin);
	}

	public void FadeMusic(AudioMixerSnapshot snap)
	{
		if (snap != null)
		{
			FadeMusic(snap, m_standardTransitionTime);
		}
	}


	public void FadeMusic(AudioMixerSnapshot snap, float duration)
	{
		if (snap != null)
		{
			snap.TransitionTo(duration);
		}
	}

	public void PlaySFX(AudioStruct au)
	{
		if (au.clip != null)
		{
			m_sources[m_curSource].clip = au.clip;
			m_sources[m_curSource].volume = au.volume;
			m_sources[m_curSource].Play();
			NextSource();
		}
	}

	public void PreviewSounds()
	{
		List<AudioStruct> allStructs = new List<AudioStruct>();
        allStructs.Add(a_bleed);
        allStructs.Add(a_burnt);
        allStructs.Add(a_coin);
        allStructs.Add(a_cut);
        allStructs.Add(a_doorOpen);
        allStructs.Add(a_doorShut);
        allStructs.Add(a_frozen);
        allStructs.Add(a_poison);
        allStructs.Add(a_stab);
        allStructs.Add(a_thump);

        allStructs.Add(a_player_death);
        allStructs.Add(a_player_jump);
        allStructs.Add(a_player_land);
        allStructs.Add(a_player_takeDamage);
       
        allStructs.Add(a_eny_die);
        allStructs.Add(a_eny_spit);
        allStructs.Add(a_eny_takeDamage);
        
        allStructs.Add(a_ui_purchase);
        allStructs.Add(a_ui_cancel);
        allStructs.Add(a_ui_confirm);
        allStructs.Add(a_ui_select);
        
        allStructs.Add(a_anubis_bash);
        allStructs.Add(a_anubis_dash);
        allStructs.Add(a_anubis_fireball);
        allStructs.Add(a_anubis_intro);
        allStructs.Add(a_anubis_takeDamage);



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

	public void SetSFXVolume(float level)
	{
		am.SetFloat("SFXVolume", level);
		PlaySFX(a_coin);
	}

	public void SetMasterVolume(float level)
	{
		am.SetFloat("MasterVolume", level);
	}

	public void SetMusicVolume(float level)
	{
		am.SetFloat("MusicVolume", level);
	}
}
