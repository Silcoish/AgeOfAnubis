using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerInventory : MonoBehaviour 
{
	public static PlayerInventory Inst;
	public int m_gold;

	public GameObject m_currentWeapon;
	public GameObject m_secondaryWeapon;

	void Awake()
	{
		if (Inst == null)
		{
			Inst = this;
		}
	}

	public GameObject SwitchWeapon()
	{
		GameObject temp = m_secondaryWeapon;
		m_secondaryWeapon = m_currentWeapon;
		m_currentWeapon = temp;

        //if (m_currentWeapon != null)
        //    m_currentWeapon.gameObject.SetActive(true);
        //if (m_secondaryWeapon != null)
        //    m_secondaryWeapon.gameObject.SetActive(false);

		return m_currentWeapon;
	}


    
}
