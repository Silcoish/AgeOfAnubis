using UnityEngine;
using System.Collections.Generic; 

public class PauseMenu : MonoBehaviour
{

	public UnityEngine.EventSystems.EventSystem m_event;
	public GameObject startingSelected;
	public List<GameObject> objectsToEnable;

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
		if(!GameManager.inst.isPaused)
		{
			GameManager.inst.PauseGame(true);
			for(int i = 0; i < objectsToEnable.Count; i++)
			{
				objectsToEnable[i].SetActive(true);
			}
			m_event.SetSelectedGameObject(startingSelected);
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
}