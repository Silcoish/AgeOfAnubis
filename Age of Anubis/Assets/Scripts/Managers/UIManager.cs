using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UIManager : MonoBehaviour {

	public static UIManager Inst;

	public Image m_healthBar;
	public Image m_xPBar;

    public Image m_healthBarSecondary;
    public Image m_xPBarSecondary;

    public Image m_weaponMain;
    public Image m_weaponSecondary;

    public Image m_weaponMainEffect;
    public Image m_weaponSecondaryEffect;

    //public Text m_weaponMainLevel;
    //public Text m_weaponSecondaryLevel;

    public float m_fillSpeed = 10.0f;
    public float m_coinSpeed = 30.0f;

    float m_playerGold = 0;
    float m_displayGold = -10;
    int m_displayLevel = -10;

	public Text m_coins;
	public Text m_multiplier;
    public Text m_playerLevel;

    public GameObject m_prefabCoin;

    public Sprite m_iconAxe;
    public Sprite m_iconDagger;
    public Sprite m_iconSword;

    public Sprite m_iconEffectPoison;
    public Sprite m_iconEffectBurn;
    public Sprite m_iconEffectBleed;

    public Sprite m_iconNone;

    private GameObject m_displayMain;
    private GameObject m_displaySecondary;

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


        if (m_displayLevel != PlayerInventory.Inst.m_playerLevel)
        {
            m_displayLevel = PlayerInventory.Inst.m_playerLevel;
            m_playerLevel.text = m_displayLevel.ToString();

        }

       // if (m_displayMain != PlayerInventory.Inst.m_currentWeapon)
        {
            m_displayMain = PlayerInventory.Inst.m_currentWeapon;

            Weapon wp = null;

            if (m_displayMain != null)
                wp = m_displayMain.GetComponent<Weapon>();

            if (wp != null)
            {
                SetWeaponIcon(m_weaponMain, wp.m_swingType);
                SetEffectIcon(m_weaponMainEffect, wp.m_attack.m_effectType);
                //m_weaponMainLevel.text = "lvl" + wp.m_level;
            }
            else
            {
                m_weaponMain.sprite = m_iconNone;
                m_weaponMainEffect.sprite = m_iconNone;
                //m_weaponMainLevel.text = "";
            }


        }

        //if (m_displaySecondary != PlayerInventory.Inst.m_secondaryWeapon)
        {
            m_displaySecondary = PlayerInventory.Inst.m_secondaryWeapon;

            Weapon wp = null;

            if (m_displaySecondary != null)
                wp = m_displaySecondary.GetComponent<Weapon>();

            if (wp != null)
            {
                SetWeaponIcon(m_weaponSecondary, wp.m_swingType);
                SetEffectIcon(m_weaponSecondaryEffect, wp.m_attack.m_effectType);
                //m_weaponSecondaryLevel.text = "lvl" + wp.m_level;
            }
            else
            {
                m_weaponSecondary.sprite = m_iconNone;
                m_weaponSecondaryEffect.sprite = m_iconNone;
               // m_weaponSecondaryLevel.text = "";
            }
        }

    }

    void SetWeaponIcon(Image im, WeaponSwing ws)
    {
        switch (ws)
        {
            case WeaponSwing.LIGHT:
                im.sprite = m_iconDagger;
                break;
            case WeaponSwing.MEDIUM:
                im.sprite = m_iconSword;
                break;
            case WeaponSwing.HEAVY:
                im.sprite = m_iconAxe;
                break;
            default:
                break;
        }
    }

    void SetEffectIcon(Image im, DamageType dt)
    {
        switch (dt)
        {
            case DamageType.NONE:
                im.sprite = m_iconNone;
                break;
            case DamageType.POISON:
                im.sprite = m_iconEffectPoison;
                break;
            case DamageType.BURN:
                im.sprite = m_iconEffectBurn;
                break;
            case DamageType.BLEED:
                im.sprite = m_iconEffectBleed;
                break;
            default:
                break;
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

        Vector2 pos = Player.Inst.gameObject.transform.position;

        pos = Camera.main.WorldToScreenPoint(pos);

        GameObject temp =  Instantiate(m_prefabCoin, pos, m_prefabCoin.transform.rotation) as GameObject;

        temp.transform.SetParent(gameObject.transform);

        temp.transform.position = pos;
        temp.transform.localScale = Vector3.one;

	}

	public void UpdateCoinMultiplier(float amount)
	{
		m_multiplier.text = ("x" + amount.ToString("0.0"));
	}
}
