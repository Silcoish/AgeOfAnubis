using UnityEngine;
using System.Collections;
using UnityEngine.UI;


public class UIBank : MonoBehaviour 
{
	public Text m_bankText;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () 
	{
		m_bankText.text = PlayerPrefs.GetInt("BankGold").ToString();
	}
}
