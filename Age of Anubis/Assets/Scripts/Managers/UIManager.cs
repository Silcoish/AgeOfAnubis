using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UIManager : MonoBehaviour {

	public static UIManager Inst;

	public Image m_healthBar;
	public Image m_xPBar;

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

	public void UpdateHealthBar(float percentage)
	{
		m_healthBar.fillAmount = percentage;
	}

	public void UpdateXPBar(float percentage)
	{
		m_xPBar.fillAmount = percentage;
	}

	public void UpdateCoinTotal(int amount)
	{
		m_coins.text = amount.ToString();
	}

	public void UpdateCoinMultiplier(float amount)
	{
		m_multiplier.text = ("x" + amount.ToString("0.0"));
	}
}
