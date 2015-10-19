using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class LoadingManager : MonoBehaviour 
{
	public static LoadingManager Inst;

	public GameObject m_GUI;
	public Image m_loadingBar;


	private AsyncOperation m_async;

	void Awake()
	{
		if (Inst == null)
		{
			Inst = this;
		}
		else
		{
			Destroy(gameObject);
		}
		DontDestroyOnLoad(gameObject);
	}

	
	// Update is called once per frame
	void Update () 
	{
		if (m_async != null)
		{
			m_loadingBar.fillAmount = m_async.progress;

			if (m_async.isDone)
			{
				m_GUI.SetActive(false);
				m_async = null;
			}
		}
	}

	public void LoadLevel(string levelName)
	{
		m_GUI.SetActive(true);

		m_async = Application.LoadLevelAsync(levelName);
	}

}
