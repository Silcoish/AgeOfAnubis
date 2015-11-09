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
	public EventSystem m_es;
	public ShopIcon m_selected;
    public GameObject m_playersWeapon;
    public GameObject m_activationArea;

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
    public Sprite m_iconNone;




	// Use this for initialization
	void Start () 
	{
		InitializeShop();
	}
	
	// Update is called once per frame
	void Update () 
	{
        if (Input.GetButtonDown("Cancel"))
            OnCloseShop();

		if (m_selected == null)
		{
			m_selected = m_es.currentSelectedGameObject.GetComponent<ShopIcon>();
		}

		if (m_es.currentSelectedGameObject != m_selected.gameObject)
		{
			m_selected = m_es.currentSelectedGameObject.GetComponent<ShopIcon>();

			UpdateCompareIcons();
		}


        if (m_playersWeapon != PlayerInventory.Inst.m_currentWeapon)
        {
            m_playersWeapon = PlayerInventory.Inst.m_currentWeapon;

            UpdateCompareIcons();
        }
	}


    void OnCloseShop()
    {
        gameObject.SetActive(false);
        GameManager.inst.player.GetComponent<Player>().m_isShopOpen = false;
        m_activationArea.SetActive(true);
    }

    public void BuySelected()
    {
        //PlayerInventory.Inst.m_currentGold -= m_selected.weapon.GetComponent<Weapon>().m_price;

        PlayerInventory.Inst.m_currentWeapon = m_selected.weapon;
        GameManager.inst.player.GetComponent<Player>().UpdateEquippedWeapon(PlayerInventory.Inst.m_currentWeapon);

        OnCloseShop();
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

		ProgressShopItems();
	}

	public void ProgressShopItems()
	{
		m_lastChanceItem1.weapon = m_newItem1.weapon;
		m_lastChanceItem2.weapon = m_newItem2.weapon;
		m_lastChanceItem3.weapon = m_newItem3.weapon;

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

		UpdateAllIcons();

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
		Weapon wp = sIcon.weapon.GetComponent<Weapon>();

		if (wp == null)
			Debug.LogError("Weapon prefab is not added.", sIcon.weapon);
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
