/* Copyright (c) Dungeon Crawlers
*  Script Created by:
*  Corey Underdown
*/
 
using UnityEngine;
using System.IO;
 
public class SaveManager : MonoBehaviour
{
	public int m_currentLevel;
	public int m_exp;
	public int m_gold;
	//public string m_weapon1;
	//public string m_weapon2;

	public bool saveExists = false;

	void Start()
	{
		if (File.Exists(Application.persistentDataPath + "Savegame.fiin"))
		{
			saveExists = true;
			Load();
		}
	}

	void Update()
	{
		if(Input.GetKeyDown(KeyCode.O))
		{
			Save();
		}
		if(Input.GetKeyDown(KeyCode.P))
		{
			Load();
		}
	}

	public void Save()
	{
		m_gold = PlayerInventory.Inst.m_currentGold;
		m_exp = PlayerInventory.Inst.m_currentXP;
		m_currentLevel = PlayerInventory.Inst.m_playerLevel;
		SaveLoad.Save();
	}

	public void NewGame()
	{
		m_gold = 0;
		m_exp = 0;
		m_currentLevel = 1;
		//m_weapon1 = "";
		//m_weapon2 = "";
		SaveLoad.Save(SaveLoad.currentFilePath, this);
	}

	public void Load()
	{
		SaveLoad.Load();
		PlayerInventory.Inst.UpdateUIElements();
	}
}