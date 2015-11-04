using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
public class OverwriteFirstSelected : MonoBehaviour 
{
	public EventSystem m_es;


	void Awake()
	{
		m_es = gameObject.GetComponent<EventSystem>();

	}

	void Update () 
	{

		m_es.firstSelectedGameObject = m_es.currentSelectedGameObject;
	}
}
