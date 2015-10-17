using UnityEngine;
using System.Collections;
using UnityEngine.Audio;


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
	private AudioSource[] m_sources;
	private int m_curSource = 0;

	[Header("Tracks")]
	public AudioClip a_coin;
	public AudioClip a_poison;
	public AudioClip a_burnt;
	public AudioClip a_bleed;
	public AudioClip a_cut;
	public AudioClip a_doorOpen;
	public AudioClip a_doorShut;
	public AudioClip a_frozen;
	public AudioClip a_giveDamage;
	public AudioClip a_takeDamage;
	public AudioClip a_lowHealth;
	public AudioClip a_purchaseItem;
	public AudioClip a_stab;
	public AudioClip a_thump;
	public AudioClip a_pickupWeapon;

	void Awake()
	{
		if (Inst == null)
			Inst = this;
		else
			Destroy(gameObject);
	}

	// Use this for initialization
	void Start () 
	{
		m_sources = new AudioSource[m_numberOfSources];

		for (int i = 0; i < m_numberOfSources; i ++)
		{
			m_sources[i] = gameObject.AddComponent<AudioSource>();
		}

		foreach (var s in m_sources)
		{
			s.outputAudioMixerGroup = g_fx;
			s.playOnAwake = false;
			s.loop = false;
		}
	}
	
	// Update is called once per frame
	void Update () 
	{
		//Testing Hacks
		if (Input.GetKeyDown(KeyCode.Z))
			AudioManager.Inst.FadeMusic(AudioManager.Inst.s_fight);
		if (Input.GetKeyDown(KeyCode.X))
			AudioManager.Inst.FadeMusic(AudioManager.Inst.s_idle);

		if (Input.GetKeyDown(KeyCode.C))
			AudioManager.Inst.PlaySFX(AudioManager.Inst.a_coin);
		if (Input.GetKeyDown(KeyCode.V))
			AudioManager.Inst.PlaySFX(AudioManager.Inst.a_poison);
	}

	public void FadeMusic(AudioMixerSnapshot snap)
	{
		FadeMusic(snap, m_standardTransitionTime);
	}


	public void FadeMusic(AudioMixerSnapshot snap, float duration)
	{
		snap.TransitionTo(duration);
	}

	public void PlaySFX(AudioClip au)
	{
		m_sources[m_curSource].clip = au;
		m_sources[m_curSource].Play();
		NextSource();
	}

	void NextSource()
	{
		m_curSource++;

		if (m_curSource == m_numberOfSources)
			m_curSource = 0;
	}
}
