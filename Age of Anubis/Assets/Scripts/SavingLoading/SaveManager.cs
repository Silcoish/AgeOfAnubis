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
	public int m_weapon1;
	public int m_weapon2;

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

		/*if (PlayerInventory.Inst.m_currentWeapon != null)
			m_weapon1 = PlayerInventory.Inst.m_currentWeapon.GetComponent<Weapon>().m_itemID;
		else
			m_weapon1 = 0;
		if (PlayerInventory.Inst.m_secondaryWeapon != null)
			m_weapon2 = PlayerInventory.Inst.m_secondaryWeapon.GetComponent<Weapon>().m_itemID;
		else
			m_weapon2 = 0;*/

		SaveLoad.Save();
	}

	public void NewGame()
	{
		m_gold = 0;
		m_exp = 0;
		m_currentLevel = 1;
		m_weapon1 = 1;
		m_weapon2 = 1;
		SaveLoad.Save(SaveLoad.currentFilePath, this);
	}

	public void Load()
	{
		SaveLoad.Load();
		PlayerInventory.Inst.UpdateUIElements();

		PlayerInventory.Inst.m_currentWeapon = WeaponManager.inst.GenerateWeaponFromID(m_weapon1);
		PlayerInventory.Inst.m_secondaryWeapon = WeaponManager.inst.GenerateWeaponFromID(m_weapon2);
		GameManager.inst.player.GetComponent<Player>().UpdateEquippedWeapon(PlayerInventory.Inst.m_currentWeapon);
		
	}
}