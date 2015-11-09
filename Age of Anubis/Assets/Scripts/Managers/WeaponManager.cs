using UnityEngine;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public enum BaseWeaponPrefab { NONE, SWORD, AXE, DAGGER };

[System.Serializable]
public struct WeaponData
{
    public int level;
    public string name;
    public BaseWeaponPrefab basePrefab;
    public int attackStrength;
    public int knockback;
    public DamageType effectType;
    public int effectStrength;
    public int effectDuration;
    public float rarity;
    public int goldCost;
}

public class WeaponManager : MonoBehaviour 
{
    public static WeaponManager inst;

    public TextAsset m_weaponList;

    public GameObject m_baseDagger;
    public GameObject m_baseSword;
    public GameObject m_baseAxe;

    public List<WeaponData> m_weaponData;

	void Awake()
    {
        if (WeaponManager.inst == null)
        {
            WeaponManager.inst = this;
            DontDestroyOnLoad(gameObject);
        }
        else
            Destroy(gameObject);
    }

    void Start()
    {
        m_weaponData = new List<WeaponData>();

        if(!m_weaponList)
        {
            Debug.Log("Missing Weapon List CSV - Cannot generate weapons!");
        }
        else
        {
            // Split csv into lines
            string[] weapons = m_weaponList.text.Split('\n');

            //Debug.Log("Weapons Found: " + weapons.Length);

            Debug.Log("Loading Weapons Data");
            // First entry in array is the column titles
            // Last entry is a single whitespace, skipping last entry
            for(int i = 1; i < weapons.Length - 1; i++)
            {
                //Debug.Log(weapons[i]);
                string[] fields = weapons[i].Split(';');

                if(fields.Length != 10)
                {
                    Debug.Log("Invalid number of fields. Weapon skipped!");
                }
                else
                {
                    //Debug.Log("Level: " + fields[0] + ", Name: " + fields[1] + ", Damage: " + fields[3]);
                    WeaponData data = new WeaponData();
                    data.level = Int32.Parse(fields[0]);
                    data.name = fields[1];
                    data.basePrefab = GetPrefabType(fields[2]);
                    data.attackStrength = Int32.Parse(fields[3]);
                    data.knockback = Int32.Parse(fields[4]);
                    data.effectType = GetDamageType(fields[5]);
                    data.effectStrength = Int32.Parse(fields[6]);
                    data.effectDuration = Int32.Parse(fields[7]);
                    data.rarity = float.Parse(fields[8]);
                    data.goldCost = Int32.Parse(fields[9]);

                    m_weaponData.Add(data);
                }
            }
            Debug.Log("Weapons Loaded: " + m_weaponData.Count);
        }
    }

    BaseWeaponPrefab GetPrefabType(string s)
    {
        if (s == "Sword")
            return BaseWeaponPrefab.SWORD;
        else if (s == "Axe")
            return BaseWeaponPrefab.AXE;
        else if (s == "Dagger")
            return BaseWeaponPrefab.DAGGER;
        else
        {
            Debug.Log("Failed to match Base Prefab Name: " + s);
            return BaseWeaponPrefab.NONE;
        }
    }

    DamageType GetDamageType(string s)
    {
        if (s == "None")
            return DamageType.NONE;
        else if (s == "Burn")
            return DamageType.BURN;
        else if (s == "Poison")
            return DamageType.POISON;
        else if (s == "Mud")
            return DamageType.MUD;
        else if (s == "Freeze")
            return DamageType.FREEZE;
        else if (s == "Bleed")
            return DamageType.BLEED;
        else if (s == "Blind")
            return DamageType.BLIND;
        else
        {
            Debug.Log("Failed to match Base Prefab Name: " + s);
            return DamageType.NONE;
        }
    }

    GameObject GenerateWeapon(List<WeaponData> weaponDataShortlist)
    {
        int id = UnityEngine.Random.Range(1, weaponDataShortlist.Count);

        WeaponData wepData = weaponDataShortlist[id];
        GameObject weapon;

        switch(wepData.basePrefab)
        {
            case BaseWeaponPrefab.AXE:
                weapon = Instantiate(m_baseAxe);
                break;
            case BaseWeaponPrefab.SWORD:
                weapon = Instantiate(m_baseSword);
                break;
            case BaseWeaponPrefab.DAGGER:
                weapon = Instantiate(m_baseDagger);
                break;
            default:
                weapon = Instantiate(m_baseDagger);
                Debug.Log("Invalid BaseWeaponPrefab when generating weapon!");
                return weapon;
        }

        weapon.GetComponent<Weapon>().ApplyWeaponData(wepData);

        return weapon;
    }

    public GameObject GenerateWeapon(int level)
    {
        List<WeaponData> shortlist = new List<WeaponData>();

        foreach(WeaponData data in m_weaponData)
        {
            if(data.level == level)
            {
                shortlist.Add(data);
            }
        }

        return GenerateWeapon(shortlist);
    }
}
