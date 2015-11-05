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
		SaveLoad.Save();
	}

	public void Load()
	{
		SaveLoad.Load();
		PlayerInventory.Inst.UpdateUIElements();
	}
}