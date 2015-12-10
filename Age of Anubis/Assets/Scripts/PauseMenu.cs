using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{

	public UnityEngine.EventSystems.EventSystem m_event;
	public GameObject startingSelected;
	public List<GameObject> objectsToEnable;

	public Slider m_sfx;
	public Slider m_music;


	void Start()
	{
		LoadSoundVolumes();

	}

	void Update()
	{
		if(m_event.currentSelectedGameObject == null)
		{
			m_event.SetSelectedGameObject(startingSelected);
		}

		if(Input.GetButtonDown("Cancel"))
		{
			Resume();
		}

		if(Input.GetButtonDown("Pause"))
		{
			Pause();
		}
	}

	public void Pause()
	{
		if (GameManager.inst != null)
		{
			if (GameManager.inst.isPaused == false && GameManager.inst.isShopOpen == false)
			{
				GameManager.inst.PauseGame(true);
				for (int i = 0; i < objectsToEnable.Count; i++)
				{
					objectsToEnable[i].SetActive(true);
				}
				m_event.SetSelectedGameObject(startingSelected);
			}
		}
	}

	public void Resume()
	{
		if(GameManager.inst.isPaused)
		{
			GameManager.inst.PauseGame(false);
			for (int i = 0; i < objectsToEnable.Count; i++)
			{
				objectsToEnable[i].SetActive(false);
			}
		}
	}

	public void Quit()
	{
		Application.Quit();
	}


	public void LoadSoundVolumes()
	{
		OnChangeSFXVolume(PlayerPrefs.GetFloat("sfxVol"));
		OnChangeMusicVolume(PlayerPrefs.GetFloat("musicVol"));

		m_sfx.value = PlayerPrefs.GetFloat("sfxVol");

		m_music.value = PlayerPrefs.GetFloat("musicVol");
	}

	public void OnChangeSFXVolume(float value)
	{
		if (value == -40)
			AudioManager.Inst.SetSFXVolume(-80);
		else
			AudioManager.Inst.SetSFXVolume(value);

		PlayerPrefs.SetFloat("sfxVol", value);
	}

	public void OnChangeMusicVolume(float value)
	{
		if (value == -40)
			AudioManager.Inst.SetMusicVolume(-80);
		else
			AudioManager.Inst.SetMusicVolume(value);

		PlayerPrefs.SetFloat("musicVol", value);
	}
}