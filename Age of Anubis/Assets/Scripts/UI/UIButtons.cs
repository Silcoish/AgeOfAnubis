using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UIButtons : MonoBehaviour 
{
	public static UIButtons Inst;

	List<GameObject> m_previousMenus;
	public GameObject m_curMenu;
	public UnityEngine.EventSystems.EventSystem m_es;

	public UnityEngine.UI.Button continueButton;
	public SaveManager m_saveManager;

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
	}

	void Update()
	{
		if(m_es.currentSelectedGameObject == null)
			m_es.SetSelectedGameObject(continueButton.gameObject);

		if (Input.GetButtonDown("Cancel"))
		{
			print("Back");
			Back();
		}

	}


	public void NewGame()
	{
		m_saveManager.NewGame();
		LoadingManager.Inst.LoadLevel("Tutorial", true);
		gameObject.SetActive(false);
	}

	public void Continue()
	{
		LoadingManager.Inst.LoadLevel("ShopScene", true);
		gameObject.SetActive(false);
	}


	public void ChangeMenus(GameObject targetMenu)
	{
		
		m_curMenu.SetActive(false);
		targetMenu.SetActive(true);
		m_previousMenus.Add(m_curMenu);
		m_curMenu = targetMenu;
	}

	public void Quit()
	{
		Application.Quit();
	}

	public void Back()
	{
		if (m_previousMenus.Count > 0)
		{
			m_previousMenus[m_previousMenus.Count - 1].SetActive(true);
			m_curMenu.SetActive(false);
			m_curMenu = m_previousMenus[m_previousMenus.Count - 1];
			m_previousMenus.Remove(m_curMenu);
		}

	}

	public void OnChangeSFXVolume(float value)
	{
		if (value == -40)
			AudioManager.Inst.SetSFXVolume(-80);
		else
			AudioManager.Inst.SetSFXVolume(value);
	}

	public void OnChangeMusicVolume(float value)
	{
		if (value == -40)
			AudioManager.Inst.SetMusicVolume(-80);
		else
			AudioManager.Inst.SetMusicVolume(value);
	}


}


