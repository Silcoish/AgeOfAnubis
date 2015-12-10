using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;


[System.Serializable]
public struct WeaponPrefabHolder
{
    public GameObject[] level1;
    public GameObject[] level2;
    public GameObject[] level3;
    public GameObject[] level4;
    public GameObject[] level5;
}

[System.Serializable]
public struct StatUI
{
    public Text current;
    public Text difference;
}

[System.Serializable]
public struct CompareIcon
{
    public Image imageWeapon;
    public Image imageEffect;
    public StatUI level;
    public StatUI damage;
    public Text effectType;
    public StatUI effectDamage;
    public StatUI effectDuration;
}

public class Shop : MonoBehaviour
{
    public static Shop Inst;

	public GameObject m_shopCanves;
	//public GameObject m_lastObjectSelected;

    public EventSystem m_es;
    public ShopIcon m_selected;
    public GameObject m_playersWeapon;
    public GameObject m_activationArea;
    public Sprite m_spriteNone;

    public bool m_needsUpdateing = true;

    public bool m_isActive = true;

    //[SerializeField]
    //WeaponPrefabHolder m_allWeaponPrefabs;

    [Header("New Item UI Objects")]
    [SerializeField]
    public ShopIcon m_newItem1;
    [SerializeField]
    public ShopIcon m_newItem2;
    [SerializeField]
    public ShopIcon m_newItem3;

    [Header("Last Chance Item UI Objects")]
    [SerializeField]
    public ShopIcon m_lastChanceItem1;
    [SerializeField]
    public ShopIcon m_lastChanceItem2;
    [SerializeField]
    public ShopIcon m_lastChanceItem3;

    [Header("Comparison Weapon UI Objects")]
    [SerializeField]
    public CompareIcon m_CompareNewWeapon;
    [SerializeField]
    public CompareIcon m_CompareCurWeapon;

    [Header("Weapon Icons")]
    public Sprite m_iconDagger;
    public Sprite m_iconSword;
    public Sprite m_iconAxe;

    [Header("Effect Icons")]
    public Sprite m_iconPoison;
    public Sprite m_iconFire;
    public Sprite m_iconFreeze;
	public Sprite m_iconBleed;
    public Sprite m_iconNone;

    private Button[] m_allButtons;
    private Animator m_anim;


    void Awake()
    {
		if (Inst == null)
			Inst = this;
		else
			Destroy(gameObject);

		m_anim = m_shopCanves.GetComponent<Animator>();
		m_allButtons = m_shopCanves.GetComponentsInChildren<Button>();
    }
    // Use this for initialization
    void Start()
    {
        InitializeShop();
		//m_shopCanves.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
		if (m_shopCanves.activeSelf == true)
		{
			if (m_es.currentSelectedGameObject != null && m_isActive)
			{
				if (Input.GetButtonDown("Cancel"))
					OnCloseShop();
			}

			if (Input.GetKeyDown(KeyCode.P))
			{
				ProgressIcons();
			}
			if (Input.GetKeyDown(KeyCode.O))
			{
				ActivateShop();
			}
			if (Input.GetKeyDown(KeyCode.I))
			{
				m_needsUpdateing = true;
				UpdateShop();
			}
			if (Input.GetKeyDown(KeyCode.U))
			{
				UpdateAllIcons();
			}

			if (m_es.currentSelectedGameObject == null)
			{
				m_es.SetSelectedGameObject(m_newItem1.gameObject);
			}

			if (m_selected == null)
			{
				m_selected = m_es.currentSelectedGameObject.GetComponent<ShopIcon>();
				//m_lastObjectSelected = m_es.currentSelectedGameObject;
			}

			if ( m_es.currentSelectedGameObject != m_selected.gameObject)
			{
				if (m_es.currentSelectedGameObject != null)
				{
					m_selected = m_es.currentSelectedGameObject.GetComponent<ShopIcon>();
				}

				UpdateCompareIcons();
			}

			if (m_playersWeapon != PlayerInventory.Inst.m_currentWeapon)
			{
				m_playersWeapon = PlayerInventory.Inst.m_currentWeapon;

				UpdateCompareIcons();
			}

		}

    }

	public void OnOpenSHop()
	{
		m_shopCanves.SetActive(true);
		GameManager.inst.player.GetComponent<Player>().m_isShopOpen = true;
		UpdateShop();

		
	}

    void OnCloseShop()
    {
		m_shopCanves.SetActive(false);
        GameManager.inst.player.GetComponent<Player>().m_isShopOpen = false;
        m_activationArea.SetActive(true);
    }

    public void BuySelected()
    {
		int gold = PlayerPrefs.GetInt("BankGold");

		if (gold >= m_selected.weapon.GetComponent<Weapon>().m_goldCost)
        {
			gold -= m_selected.weapon.GetComponent<Weapon>().m_goldCost;

            if (PlayerInventory.Inst.m_secondaryWeapon == null)
            {
                PlayerInventory.Inst.m_secondaryWeapon = m_selected.weapon;
                UIManager.Inst.BuySecondary();
            }
            else
            {
                PlayerInventory.Inst.m_currentWeapon = m_selected.weapon;
                UIManager.Inst.BuyMain();
            }

            m_es.currentSelectedGameObject.GetComponent<Button>().interactable = false;

            foreach (var b in m_allButtons)
            {
                if (b.interactable == true)
                    m_es.SetSelectedGameObject(b.gameObject);
            }


            GameManager.inst.player.GetComponent<Player>().UpdateEquippedWeapon(PlayerInventory.Inst.m_currentWeapon);
			AudioManager.Inst.PlaySFX(AudioManager.Inst.a_ui_purchase);
        }
		else
		{
			AudioManager.Inst.PlaySFX(AudioManager.Inst.a_ui_cancel);
		}

		PlayerPrefs.SetInt("BankGold", gold);

        PlayerInventory.Inst.UpdateUIElements();

        //OnCloseShop();
    }

    void UpdateCompareIcons()
    {
        UpdateCompareStats(m_CompareNewWeapon, m_selected.weapon);
        UpdateCompareStats(m_CompareCurWeapon, PlayerInventory.Inst.m_currentWeapon);

        UpdateCompareDifferences(m_selected.weapon.GetComponent<Weapon>(), PlayerInventory.Inst.m_currentWeapon.GetComponent<Weapon>());
    }

    void UpdateCompareStats(CompareIcon ci, GameObject weaponObject)
    {
        Weapon wp = weaponObject.GetComponent<Weapon>();

        if (wp == null)
            Debug.LogError("Weapon prefab is not added.", weaponObject);
        else
        {
            switch (wp.m_swingType)
            {
                case WeaponSwing.LIGHT:
                    ci.imageWeapon.sprite = m_iconDagger;
                    break;
                case WeaponSwing.MEDIUM:
                    ci.imageWeapon.sprite = m_iconSword;
                    break;
                case WeaponSwing.HEAVY:
                    ci.imageWeapon.sprite = m_iconAxe;
                    break;
            }

        }

        ci.level.current.text = wp.m_level.ToString("0");
        ci.damage.current.text = wp.m_attack.m_attackStrength.ToString("0");


        ci.effectType.text = wp.m_attack.m_effectType.ToString();
        ci.imageEffect.sprite = GetEffectSprite(wp.m_attack.m_effectType);
        ci.effectDamage.current.text = wp.m_attack.m_effectStrength.ToString("0");
        ci.effectDuration.current.text = wp.m_attack.m_effectDuration.ToString("0");
    }

    Sprite GetEffectSprite(DamageType dt)
    {
        switch (dt)
        {
            case DamageType.FREEZE:
                return m_iconFreeze;
            case DamageType.BURN:
                return m_iconFire;
            case DamageType.POISON:
                return m_iconPoison;
			case DamageType.BLEED:
				return m_iconBleed;
        }

        return m_iconNone;
    }

    void UpdateCompareDifferences(Weapon wNew, Weapon wCur)
    {
        int difLevel = wNew.m_level - wCur.m_level;
        int difDam = wNew.m_attack.m_attackStrength - wCur.m_attack.m_attackStrength;

        int difEffectDam = (int)(wNew.m_attack.m_effectStrength - wCur.m_attack.m_effectStrength);
        float difEffectDur = wNew.m_attack.m_effectDuration - wCur.m_attack.m_effectDuration;

        if (difLevel == 0)
        {
            m_CompareNewWeapon.level.difference.text = "";

            m_CompareCurWeapon.level.difference.text = "";
        }
        else if (difLevel > 0)
        {
            m_CompareNewWeapon.level.difference.text = "+" + Mathf.Abs(difLevel);
            m_CompareNewWeapon.level.difference.color = Color.green;

            m_CompareCurWeapon.level.difference.text = "-" + Mathf.Abs(difLevel);
            m_CompareCurWeapon.level.difference.color = Color.red;
        }
        else
        {
            m_CompareNewWeapon.level.difference.text = "-" + Mathf.Abs(difLevel);
            m_CompareNewWeapon.level.difference.color = Color.red;

            m_CompareCurWeapon.level.difference.text = "+" + Mathf.Abs(difLevel);
            m_CompareCurWeapon.level.difference.color = Color.green;
        }
        ///////////////////////////////////////////////////////////////////////////
        //DAMAGE
        ///////////////////////////////////////////////////////////////////////////
        if (difDam == 0)
        {
            m_CompareNewWeapon.damage.difference.text = "";

            m_CompareCurWeapon.damage.difference.text = "";
        }
        else if (difDam > 0)
        {
            m_CompareNewWeapon.damage.difference.text = "+" + Mathf.Abs(difDam);
            m_CompareNewWeapon.damage.difference.color = Color.green;

            m_CompareCurWeapon.damage.difference.text = "-" + Mathf.Abs(difDam);
            m_CompareCurWeapon.damage.difference.color = Color.red;
        }
        else
        {
            m_CompareNewWeapon.damage.difference.text = "-" + Mathf.Abs(difDam);
            m_CompareNewWeapon.damage.difference.color = Color.red;

            m_CompareCurWeapon.damage.difference.text = "+" + Mathf.Abs(difDam);
            m_CompareCurWeapon.damage.difference.color = Color.green;
        }
        ///////////////////////////////////////////////////////////////////////////
        //difEffectDam
        ///////////////////////////////////////////////////////////////////////////
        if (difEffectDam == 0)
        {
            m_CompareNewWeapon.effectDamage.difference.text = "";

            m_CompareCurWeapon.effectDamage.difference.text = "";
        }
        else if (difEffectDam > 0)
        {
            m_CompareNewWeapon.effectDamage.difference.text = "+" + Mathf.Abs(difEffectDam);
            m_CompareNewWeapon.effectDamage.difference.color = Color.green;

            m_CompareCurWeapon.effectDamage.difference.text = "-" + Mathf.Abs(difEffectDam);
            m_CompareCurWeapon.effectDamage.difference.color = Color.red;
        }
        else
        {
            m_CompareNewWeapon.effectDamage.difference.text = "-" + Mathf.Abs(difEffectDam);
            m_CompareNewWeapon.effectDamage.difference.color = Color.red;

            m_CompareCurWeapon.effectDamage.difference.text = "+" + Mathf.Abs(difEffectDam);
            m_CompareCurWeapon.effectDamage.difference.color = Color.green;
        }
        ///////////////////////////////////////////////////////////////////////////
        //effectDuration
        ///////////////////////////////////////////////////////////////////////////
        if (difEffectDur == 0)
        {
            m_CompareNewWeapon.effectDuration.difference.text = "";

            m_CompareCurWeapon.effectDuration.difference.text = "";
        }
        else if (difEffectDur > 0)
        {
            m_CompareNewWeapon.effectDuration.difference.text = "+" + Mathf.Abs(difEffectDur).ToString("0.0");
            m_CompareNewWeapon.effectDuration.difference.color = Color.green;

            m_CompareCurWeapon.effectDuration.difference.text = "-" + Mathf.Abs(difEffectDur).ToString("0.0");
            m_CompareCurWeapon.effectDuration.difference.color = Color.red;
        }
        else
        {
            m_CompareNewWeapon.effectDuration.difference.text = "-" + Mathf.Abs(difEffectDur).ToString("0.0");
            m_CompareNewWeapon.effectDuration.difference.color = Color.red;

            m_CompareCurWeapon.effectDuration.difference.text = "+" + Mathf.Abs(difEffectDur).ToString("0.0");
            m_CompareCurWeapon.effectDuration.difference.color = Color.green;
        }


    }

    GameObject GetRandomWeapon()
    {
        GameObject temp;

        temp = WeaponManager.inst.GenerateWeapon(PlayerInventory.Inst.m_playerLevel);

        //int rand = 0;
        //switch (PlayerInventory.Inst.m_playerLevel)
        //{ 
        //    case 1:
        //        rand = Random.Range(0, m_allWeaponPrefabs.level1.Length);
        //        temp = m_allWeaponPrefabs.level1[rand];
        //        break;
        //    case 2:
        //        rand = Random.Range(0, m_allWeaponPrefabs.level2.Length);
        //        temp = m_allWeaponPrefabs.level2[rand];
        //        break;
        //    case 3:
        //        rand = Random.Range(0, m_allWeaponPrefabs.level3.Length);
        //        temp = m_allWeaponPrefabs.level3[rand];
        //        break;
        //    case 4:
        //        rand = Random.Range(0, m_allWeaponPrefabs.level4.Length);
        //        temp = m_allWeaponPrefabs.level4[rand];
        //        break;
        //    case 5:
        //        rand = Random.Range(0, m_allWeaponPrefabs.level5.Length);
        //        temp = m_allWeaponPrefabs.level5[rand];
        //        break;
        //    default:
        //        rand = Random.Range(0, m_allWeaponPrefabs.level1.Length);
        //        temp = m_allWeaponPrefabs.level1[rand];
        //        Debug.LogError("Random Weapon: Player Level Not within range");
        //        break;
        //}

        return temp;

    }

    public void InitializeShop()
    {
         if (PlayerPrefs.HasKey("Shop_Weapon_new_1") && PlayerPrefs.HasKey("Shop_Weapon_new_2") && PlayerPrefs.HasKey("Shop_Weapon_new_3"))
        {
            m_newItem1.weapon = WeaponManager.inst.GenerateWeaponFromID(PlayerPrefs.GetInt("Shop_Weapon_new_1"));
            m_newItem2.weapon = WeaponManager.inst.GenerateWeaponFromID(PlayerPrefs.GetInt("Shop_Weapon_new_2"));
            m_newItem3.weapon = WeaponManager.inst.GenerateWeaponFromID(PlayerPrefs.GetInt("Shop_Weapon_new_3"));

            if (PlayerPrefs.HasKey("Shop_Weapon_last_1") && PlayerPrefs.HasKey("Shop_Weapon_last_2") && PlayerPrefs.HasKey("Shop_Weapon_last_3"))
            {
                m_lastChanceItem1.weapon = WeaponManager.inst.GenerateWeaponFromID(PlayerPrefs.GetInt("Shop_Weapon_last_1"));
                m_lastChanceItem2.weapon = WeaponManager.inst.GenerateWeaponFromID(PlayerPrefs.GetInt("Shop_Weapon_last_2"));
                m_lastChanceItem3.weapon = WeaponManager.inst.GenerateWeaponFromID(PlayerPrefs.GetInt("Shop_Weapon_last_3"));
            }
        }
        else
        {
            int count = 0;

            m_newItem1.weapon = GetRandomWeapon();
            do
            {
                m_newItem2.weapon = GetRandomWeapon();
                count++;
                if (count > 200)
                    break;
            } while (m_newItem2.weapon == m_newItem1.weapon);

            do
            {
                m_newItem3.weapon = GetRandomWeapon();
                count++;
                if (count > 200)
                    break;
            } while (m_newItem3.weapon == m_newItem1.weapon || m_newItem3.weapon == m_newItem2.weapon);

            PlayerPrefs.SetInt("Shop_Weapon_new_1", m_newItem1.weapon.GetComponent<Weapon>().m_itemID);
            PlayerPrefs.SetInt("Shop_Weapon_new_2", m_newItem2.weapon.GetComponent<Weapon>().m_itemID);
            PlayerPrefs.SetInt("Shop_Weapon_new_3", m_newItem3.weapon.GetComponent<Weapon>().m_itemID);
        }

        m_needsUpdateing = true;
        UpdateAllIcons();
        //ProgressShopItems();
    }

    public void DeactivateShop()
    {
		if (AudioManager.Inst != null)
			AudioManager.Inst.PlaySFX(AudioManager.Inst.a_ui_cancel);
        foreach (var b in m_allButtons)
        {
            b.interactable = false;
        }
        m_es.SetSelectedGameObject(null);
        m_isActive = false;
    }

    public void ActivateShop()
    {
		AudioManager.Inst.PlaySFX(AudioManager.Inst.a_ui_confirm);
        foreach (var b in m_allButtons)
        {
            b.interactable = true;
        }
        m_es.SetSelectedGameObject(m_es.firstSelectedGameObject);
        m_isActive = true;
    }

    public void ForceProgressShop()
    {
        if (m_needsUpdateing)
            ProgressIcons();

    }

    public void FlagToPrograssItems()
    {
        m_needsUpdateing = true;
    }

    void UpdateShop()
    {
        if (m_needsUpdateing)
        {
            DeactivateShop();
            m_anim.SetTrigger("ProgressShop");
        }
    }

    public void ProgressIcons()
    {
        Destroy(m_lastChanceItem1.weapon);
        Destroy(m_lastChanceItem2.weapon);
        Destroy(m_lastChanceItem3.weapon);

        m_lastChanceItem1.weapon = WeaponManager.inst.GenerateWeaponFromID(m_newItem1.weapon.GetComponent<Weapon>().m_itemID);
        m_lastChanceItem2.weapon = WeaponManager.inst.GenerateWeaponFromID(m_newItem2.weapon.GetComponent<Weapon>().m_itemID);
        m_lastChanceItem3.weapon = WeaponManager.inst.GenerateWeaponFromID(m_newItem3.weapon.GetComponent<Weapon>().m_itemID);


        PlayerPrefs.SetInt("Shop_Weapon_last_1", m_lastChanceItem1.weapon.GetComponent<Weapon>().m_itemID);
        PlayerPrefs.SetInt("Shop_Weapon_last_2", m_lastChanceItem2.weapon.GetComponent<Weapon>().m_itemID);
        PlayerPrefs.SetInt("Shop_Weapon_last_3", m_lastChanceItem3.weapon.GetComponent<Weapon>().m_itemID);

        UpdateAllIcons();

        int count = 0;

        m_newItem1.weapon = GetRandomWeapon();
        do
        {

            m_newItem2.weapon = GetRandomWeapon();
            count++;
            if (count > 200)
                break;
        } while (m_newItem2.weapon == m_newItem1.weapon);

        do
        {

            m_newItem3.weapon = GetRandomWeapon();
            count++;
            if (count > 200)
                break;
        } while (m_newItem3.weapon == m_newItem1.weapon || m_newItem3.weapon == m_newItem2.weapon);

        PlayerPrefs.SetInt("Shop_Weapon_new_1", m_newItem1.weapon.GetComponent<Weapon>().m_itemID);
        PlayerPrefs.SetInt("Shop_Weapon_new_2", m_newItem2.weapon.GetComponent<Weapon>().m_itemID);
        PlayerPrefs.SetInt("Shop_Weapon_new_3", m_newItem3.weapon.GetComponent<Weapon>().m_itemID);

        UpdateAllIcons();

        m_anim.SetTrigger("FinishedUpdate");
        m_needsUpdateing = false;
    }

    public void ReturnToIdle()
    {
        ActivateShop();
        m_anim.SetTrigger("Idle");
    }

    void UpdateAllIcons()
    {
        UpdateIcon(m_newItem1);
        UpdateIcon(m_newItem2);
        UpdateIcon(m_newItem3);

        UpdateIcon(m_lastChanceItem1);
        UpdateIcon(m_lastChanceItem2);
        UpdateIcon(m_lastChanceItem3);
    }


    void UpdateIcon(ShopIcon sIcon)
    {
        Weapon wp = null;
        if (sIcon.weapon == null)
        {
            sIcon.image.sprite = m_spriteNone;
            sIcon.effect.sprite = m_spriteNone;
            sIcon.cost.text = "";
        }
        else
        {
            wp = sIcon.weapon.GetComponent<Weapon>();

            if (wp == null)
            {
                Debug.LogError("Weapon prefab is not added.", sIcon.weapon);
            }
            else
            {
                switch (wp.m_swingType)
                {
                    case WeaponSwing.LIGHT:
                        sIcon.image.sprite = m_iconDagger;
                        break;
                    case WeaponSwing.MEDIUM:
                        sIcon.image.sprite = m_iconSword;
                        break;
                    case WeaponSwing.HEAVY:
                        sIcon.image.sprite = m_iconAxe;
                        break;
                }

            }
            sIcon.effect.sprite = GetEffectSprite(wp.m_attack.m_effectType);
            sIcon.cost.text = wp.m_goldCost.ToString();
        }

    }

	public void SelectAudio()
	{
		AudioManager.Inst.PlaySFX(AudioManager.Inst.a_ui_select);
	}
}