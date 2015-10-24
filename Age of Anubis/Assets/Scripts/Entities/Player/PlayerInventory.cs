using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerInventory : MonoBehaviour 
{
	public static PlayerInventory Inst;
	int m_coin;
    float m_multiplier = 1F;

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
		if(m_secondaryWeapon)
        {
            GameObject temp = m_secondaryWeapon;
            m_secondaryWeapon = m_currentWeapon;
            m_currentWeapon = temp;
        }

		return m_currentWeapon;
	}

    // Reset temporary values on player death.
    public void DeathReset()
    {
        m_coin = 0;
        m_secondaryWeapon = null;
    }

	public void ChangeGold(int amount)
	{
		m_coin += (int)(amount * m_multiplier);
		UIManager.Inst.UpdateCoinTotal(m_coin);
	}

	public void ChangeMultiplier(float amount)
	{
		m_multiplier += amount;
	}
    
}
