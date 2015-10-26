using UnityEngine;
using System.Collections;

public class LoadNextSceneOnDelay : MonoBehaviour 
{
	public string m_nextScene;
	public float m_delayTime = 5;
	public bool m_showLoadingScreen = false;
	private float m_timer = 0;

	
	// Update is called once per frame
	void Update () 
	{
		m_timer += Time.deltaTime;

		if (m_timer > m_delayTime)
			LoadingManager.Inst.LoadLevel(m_nextScene, m_showLoadingScreen);
	}
}
