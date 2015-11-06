/* Copyright (c) Handsome Dragon Games
*  http://www.handsomedragongames.com
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

	void Start()
	{
		if (File.Exists(Application.persistentDataPath + "Savegame.fiin"))
		{
			print("LOADING");
			Load();
		}
		else
		{
			Save();
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
		if(Input.GetKeyDown(KeyCode.L))
		{
			print(m_exp);
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
		SaveLoad.Save(SaveLoad.currentFilePath, this);
	}

	public void Load()
	{
		SaveLoad.Load();
		PlayerInventory.Inst.UpdateUIElements();
	}
}