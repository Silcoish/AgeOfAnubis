using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class LoadingManager : MonoBehaviour 
{
	public static LoadingManager Inst;

	public GameObject m_GUI;
	public Image m_loadingBar;

	public float m_fadeTime = 3;

	public Image m_vingette;
	public Image m_blackout;

	float m_fadeAmount = 0;//0 = off 1 = on;

	enum LoadingState { Idle, LoadScene, FadeIn, FadeOut}
	LoadingState m_curState = LoadingState.Idle;

	string m_nextScene = null;
	bool m_showLoadingScreen = true;



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
		switch(m_curState)
		{
			case LoadingState.Idle:
				break;
			case LoadingState.FadeIn:
				m_fadeAmount += Time.deltaTime / m_fadeTime;
				SetAlpha();

				if (m_fadeAmount >= 1)
				{
					m_fadeAmount = 1;
					if (m_showLoadingScreen)
					{
						m_GUI.SetActive(m_showLoadingScreen);
						ChangeStates(LoadingState.FadeOut);
						m_loadingBar.fillAmount = 0;
					}
					else
					{
						if (m_nextScene != null)
						{
							Application.LoadLevel(m_nextScene);
							ChangeStates(LoadingState.FadeOut);
						}
						else
						{
							m_GUI.SetActive(false);
							ChangeStates(LoadingState.FadeOut);

						}
					}
				}

				break;
			case LoadingState.FadeOut:
				m_fadeAmount -= Time.deltaTime / m_fadeTime;
				SetAlpha();

				if (m_fadeAmount <= 0)
				{
					m_fadeAmount = 0;

					if (m_showLoadingScreen)
					{
						m_async = Application.LoadLevelAsync(m_nextScene);
						ChangeStates(LoadingState.LoadScene);
					}
					else
					{
						ChangeStates(LoadingState.Idle);
					}
				}
				break;
			case LoadingState.LoadScene:
				if (m_async != null)
				{
					m_loadingBar.fillAmount = m_async.progress;

					if (m_async.isDone)
					{
						ChangeStates(LoadingState.FadeIn);
						m_loadingBar.fillAmount = 1;
						m_async = null;
						m_showLoadingScreen = false;
						m_nextScene = null;
					}
				}
				break;

		}


		//if (Input.GetKeyDown(KeyCode.M))
		//{
		//	LoadLevel("ShopScene", false);
		//}


	}

	public void LoadLevel(string levelName, bool showLoadingScreen)
	{
		m_nextScene = levelName;
		m_showLoadingScreen = showLoadingScreen;
		ChangeStates(LoadingState.FadeIn);

		AudioManager.Inst.FadeMusic(AudioManager.Inst.s_none, 4.0f);
	}


	void SetAlpha()
	{
		Color bo = new Color(1, 1, 1, (m_fadeAmount ));
		Color vi = new Color(1, 1, 1, (m_fadeAmount ));

		m_blackout.color = bo;
		m_vingette.color = vi;
	}

	void ChangeStates(LoadingState ls)
	{
		m_curState = ls;

		switch(ls)
		{
			case LoadingState.Idle:
				break;
			case LoadingState.FadeIn:
				break;
			case LoadingState.LoadScene:
				break;
			case LoadingState.FadeOut:
				break;
		}

	}

}
