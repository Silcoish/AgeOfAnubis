using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UIManager : MonoBehaviour {

	public static UIManager Inst;

	public Image m_healthBar;
	public Image m_xPBar;

    public Image m_healthBarSecondary;
    public Image m_xPBarSecondary;

    public float m_fillSpeed = 10.0f;
    public float m_coinSpeed = 30.0f;

    float m_playerGold = 0;
    float m_displayGold = -10;

	public Text m_coins;
	public Text m_multiplier;

	void Awake()
	{
		if (Inst == null)
		{
			Inst = this;
		}
		else
		{
			Destroy(gameObject);
		}
	}

	void Start()
	{
		PlayerInventory.Inst.UpdateUIElements();
	}

    void Update()
    {
        if (m_xPBar.fillAmount != m_xPBarSecondary.fillAmount)
        {
            m_xPBar.fillAmount += m_fillSpeed * Time.deltaTime;

            if (m_xPBar.fillAmount > m_xPBarSecondary.fillAmount)
            {
                m_xPBar.fillAmount = m_xPBarSecondary.fillAmount;
            }
        }

        if (m_healthBar.fillAmount != m_healthBarSecondary.fillAmount)
        {
            m_healthBarSecondary.fillAmount -= m_fillSpeed * Time.deltaTime;

            if (m_healthBarSecondary.fillAmount < m_healthBar.fillAmount)
            {
                m_healthBarSecondary.fillAmount = m_healthBar.fillAmount;
            }
        }

        if (m_displayGold != m_playerGold)
        {
            m_displayGold += m_coinSpeed * Time.deltaTime;

            if (m_displayGold > m_playerGold)
            {
                m_displayGold = m_playerGold;
            }

            m_coins.text = ((int)m_displayGold).ToString();
        }
    }

	public void UpdateHealthBar(float percentage)
	{
		m_healthBar.fillAmount = percentage;
	}

	public void UpdateXPBar(float percentage)
	{
		m_xPBarSecondary.fillAmount = percentage;
	}

	public void UpdateCoinTotal(int amount)
	{
        m_playerGold = (float)amount;
	}

	public void UpdateCoinMultiplier(float amount)
	{
		m_multiplier.text = ("x" + amount.ToString("0.0"));
	}
}
