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
    public int m_playerLevel = 1;
	public int m_savedPlayerLevel = 1;
    public int m_currentXP = 0;
    private int m_savedXP = 0;
    public int[] m_XPToLVL;
    public int m_hpGainPerLVL = 50;

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
		if(GameManager.inst.m_saveManager != null)
		{
			m_currentGold = GameManager.inst.m_saveManager.m_gold;
			m_savedGold = m_currentGold;
			m_currentXP = (int)GameManager.inst.m_saveManager.m_exp;
			m_savedXP = m_currentXP;
			m_playerLevel = (int)GameManager.inst.m_saveManager.m_currentLevel;
			m_savedPlayerLevel = m_playerLevel;
		}
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
        m_currentGold = m_savedGold;
        m_currentXP = m_savedXP;
		m_playerLevel = m_savedPlayerLevel;
        m_currentWeapon = null;
        m_multiplier = 1;
        CheckWeaponValidity();

		if(GameManager.inst.m_saveManager != null)
		{
			GameManager.inst.m_saveManager.Save();
		}

		if(GameManager.inst.destroyNextScene)
		{
			Destroy(GameManager.inst.gameObject);
		}
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
        UIManager.Inst.UpdateCoinTotal(m_currentGold);
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
        if (m_playerLevel >= m_XPToLVL.Length)
			return;
        if((m_currentXP) >= m_XPToLVL[m_playerLevel - 1])
        {
            m_playerLevel++;
            m_currentXP = 0;

            Damageable player = GameObject.FindGameObjectWithTag("Player").GetComponent<Damageable>();
            if (player)
                player.m_hitPoints += m_hpGainPerLVL;
            else
                Debug.Log("PlayerInventory - Failed to find GameObject with 'Player' tag");
        }
    }

    public void CheckWeaponValidity()
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
