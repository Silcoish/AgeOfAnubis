using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerInventory : MonoBehaviour 
{
	public static PlayerInventory Inst;
	public int m_gold;

	public Weapon m_currentWeapon;
	public Weapon m_secondaryWeapon;

	void Awake()
	{
		if (Inst == null)
		{
			Inst = this;
		}
	}

	public Weapon SwitchWeapon()
	{
		Weapon temp = m_secondaryWeapon;
		m_secondaryWeapon = m_currentWeapon;
		m_currentWeapon = temp;

		if (m_currentWeapon != null)
			m_currentWeapon.gameObject.SetActive(true);
		if (m_secondaryWeapon != null)
			m_secondaryWeapon.gameObject.SetActive(false);

		return m_currentWeapon;
	}


    
}
