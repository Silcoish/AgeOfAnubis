using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class UIButtons : MonoBehaviour 
{
	public static UIButtons Inst;

	List<GameObject> m_previousMenus;
	public GameObject m_curMenu;
	public UnityEngine.EventSystems.EventSystem m_es;

	public UnityEngine.UI.Button continueButton;
	public SaveManager m_saveManager;

	public Slider m_sfx;
	public Slider m_music;

	float counter;
	public float disableTime = 10.0f;

	void Awake()
	{
		if (Inst == null)
			Inst = this;
		else
			Destroy(this);

		m_previousMenus = new List<GameObject>();
	}

	void Start()
	{
		//Disable continue if no savefile
		if(!m_saveManager.saveExists)
		{
			continueButton.interactable = false;
		}

		LoadSoundVolumes();
		
	}

	void Update()
	{
		counter += Time.deltaTime;
		if (counter <= disableTime)
		{
			if (m_es.currentSelectedGameObject != null)
				m_es.SetSelectedGameObject(null);
			return;
		}

		if(m_es.currentSelectedGameObject == null)
			m_es.SetSelectedGameObject(continueButton.gameObject);

		if (Input.GetButtonDown("Cancel"))
		{
			print("Back");
			AudioManager.Inst.PlaySFX(AudioManager.Inst.a_ui_cancel);
			Back();
		}

	}

	void LoadSoundVolumes()
	{
		OnChangeSFXVolume(PlayerPrefs.GetFloat("sfxVol"));
		OnChangeMusicVolume(PlayerPrefs.GetFloat("musicVol"));

		m_sfx.value = PlayerPrefs.GetFloat("sfxVol");

		m_music.value = PlayerPrefs.GetFloat("musicVol");
	}



	public void NewGame()
	{
		AudioManager.Inst.PlaySFX(AudioManager.Inst.a_ui_confirm);

		m_saveManager.NewGame();
		LoadingManager.Inst.LoadLevel("Tutorial", true);
		gameObject.SetActive(false);

	}

	public void Continue()
	{
		AudioManager.Inst.PlaySFX(AudioManager.Inst.a_ui_confirm);

		LoadingManager.Inst.LoadLevel("ShopScene", true);
		gameObject.SetActive(false);
	}


	public void ChangeMenus(GameObject targetMenu)
	{
		AudioManager.Inst.PlaySFX(AudioManager.Inst.a_ui_confirm);

		m_curMenu.SetActive(false);
		targetMenu.SetActive(true);
		m_previousMenus.Add(m_curMenu);
		m_curMenu = targetMenu;
	}

	public void Quit()
	{
		AudioManager.Inst.PlaySFX(AudioManager.Inst.a_ui_confirm);

		Application.Quit();
	}

	public void Back()
	{
		if (m_previousMenus.Count > 0)
		{
			
			m_curMenu.SetActive(false);
			m_previousMenus[m_previousMenus.Count - 1].SetActive(true);
			m_curMenu = m_previousMenus[m_previousMenus.Count - 1];
			m_previousMenus.Remove(m_curMenu);

			AudioManager.Inst.PlaySFX(AudioManager.Inst.a_ui_cancel);
		}

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

	public void OnSelected()
	{
		AudioManager.Inst.PlaySFX(AudioManager.Inst.a_ui_select);
	}


}


