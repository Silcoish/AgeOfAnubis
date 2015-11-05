using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerInventory : MonoBehaviour 
{
	public static PlayerInventory Inst;
    public int m_currentGold;
    private int m_savedGold;
    private float m_multiplier = 1F;
	public float m_multiplierIncrease = 0.1f;
    private int m_playerLevel = 1;
    public int m_currentXP = 0;
    private int m_savedXP = 0;
    public int[] m_XPToLVL;

	public GameObject m_currentWeapon;
	public GameObject m_secondaryWeapon;
    public GameObject m_defaultWeapon;

	void Awake()
	{
		if (Inst == null)
		{
			Inst = this;
		}

        CheckWeaponValidity();
	}

    void Start()
    {
		m_currentGold = GameManager.inst.m_saveManager.m_gold;
		m_currentXP = (int)GameManager.inst.m_saveManager.m_exp;
        UpdateUIElements();
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
        m_currentGold = 0;
        m_currentXP = 0;
        m_currentWeapon = null;
        CheckWeaponValidity();
    }

	public void ChangeGold(int amount)
	{
		m_currentGold += (int)(amount * m_multiplier);
        UpdateUIElements();
	}

    public void ChangeXP(int amount)
    {
        m_currentXP += amount;
        CheckForLevelUp();
        UpdateUIElements();
    }

	public void ChangeMultiplier(float amount)
	{
		m_multiplier += amount;
	}

	public void IncreaseMultiplier()
	{
		m_multiplier += m_multiplierIncrease;
        UpdateUIElements();
	}
    
    public void UpdateUIElements()
    {
        UIManager.Inst.UpdateCoinTotal(m_currentGold + m_savedGold);
        UIManager.Inst.UpdateCoinMultiplier(m_multiplier);
        UIManager.Inst.UpdateXPBar(GetCurrentLevelPercent());
    }

    public float GetCurrentLevelPercent()
    {
        float pc = m_currentXP + m_savedXP;

        if(m_playerLevel < 5)
        {
            pc = pc / m_XPToLVL[m_playerLevel - 1];
        }
        else
        {
            pc = 1;
        }
        return pc;
    }

    void CheckForLevelUp()
    {
        if((m_currentXP + m_savedXP) >= m_XPToLVL[m_playerLevel - 1])
        {
            m_playerLevel++;
            m_currentXP = 0;
            m_savedXP = 0;
        }
    }

    void CheckWeaponValidity()
    {
        // Check for weapon in primary slot, move secondary or generate default if not.
        if (!m_currentWeapon)
        {
            if (m_secondaryWeapon)
            {
                m_currentWeapon = m_secondaryWeapon;
                m_secondaryWeapon = null;
            }
            else
                m_currentWeapon = m_defaultWeapon;
        }
    }
}
