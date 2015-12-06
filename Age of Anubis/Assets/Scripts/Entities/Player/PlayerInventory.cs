using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerInventory : MonoBehaviour 
{
	public static PlayerInventory Inst;
    public int m_currentGold;
	public int m_bankGold;
    private int m_savedGold;
    private float m_multiplier = 1F;
	public float m_multiplierIncrease = 0.1f;
    public int m_playerLevel = 1;
	public int m_savedPlayerLevel = 1;
    public int m_currentXP = 0;
    private int m_savedXP = 0;
    public int[] m_XPToLVL;
    public int m_hpGainPerLVL = 50;

	public int m_healthDrop;

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
			print("Current Gold: " + m_currentGold);
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
    }

	public void ChangeGold(int amount)
	{
		m_currentGold += (int)(amount * m_multiplier);
        //UpdateUIElements();
        UIManager.Inst.UpdateCoinTotal(m_currentGold);
	}

    public void ChangeXP(int amount)
    {
        m_currentXP += amount;
        CheckForLevelUp();
        UIManager.Inst.UpdateXPBar(GetCurrentLevelPercent());
        //UpdateUIElements();
    }

	public void ChangeMultiplier(float amount)
	{
		m_multiplier += amount;
        UIManager.Inst.UpdateCoinMultiplier(m_multiplier);
	}

	public void IncreaseMultiplier()
	{
		m_multiplier += m_multiplierIncrease;
        UIManager.Inst.UpdateCoinMultiplier(m_multiplier);
        //UpdateUIElements();
	}
    
    public void UpdateUIElements()
    {
        UIManager.Inst.UpdateCoinTotal(m_currentGold);
        UIManager.Inst.UpdateXPBar(GetCurrentLevelPercent());
        UIManager.Inst.UpdateCoinMultiplier(m_multiplier);
        UIManager.Inst.UpdateCoinMultiplier(m_multiplier);
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

			AudioManager.Inst.PlaySFX(AudioManager.Inst.a_player_levelUp);
        }
    }

    public void CheckWeaponValidity()
    {
        // Check for weapon in primary slot, move secondary or generate default if not.
        if (m_currentWeapon == null)
        {
            Debug.Log("Checking Weapon Valididy: curwep = " + m_currentWeapon);
            if (m_secondaryWeapon != null)
            {
                Debug.Log("Checking Weapon Valididy: secwep = " + m_secondaryWeapon);
                m_currentWeapon = m_secondaryWeapon;
                m_secondaryWeapon = null;
            }
            else
            {
                m_currentWeapon = m_defaultWeapon;
                Debug.Log("Checking Weapon Validity: default = " + m_defaultWeapon);
            } 
        }
    }
}
