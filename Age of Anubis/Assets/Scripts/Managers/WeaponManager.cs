using UnityEngine;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public enum BaseWeaponPrefab { NONE, SWORD, AXE, DAGGER };

[System.Serializable]
public struct WeaponData
{
    public int itemID;
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


    public GameObject m_weaponParent;

	void Awake()
    {
        if (WeaponManager.inst == null)
        {
            WeaponManager.inst = this;
            //DontDestroyOnLoad(gameObject);

            m_weaponData = new List<WeaponData>();

            m_weaponParent = Instantiate(new GameObject());
            m_weaponParent.SetActive(false);

            if (!m_weaponList)
            {
                Debug.Log("Missing Weapon List CSV - Cannot generate weapons!");
            }
            else
            {
                // Split csv into lines
                string[] weapons = m_weaponList.text.Split('\n');

                //Debug.Log("Weapons Found: " + weapons.Length);

                //Debug.Log("Loading Weapons Data");
                // First entry in array is the column titles
                // Last entry is a single whitespace, skipping last entry
                for (int i = 1; i < weapons.Length - 1; i++)
                {
                    //Debug.Log(weapons[i]);
                    string[] fields = weapons[i].Split(';');

                    if (fields.Length != 11)
                    {
                        Debug.Log("Invalid number of fields. Weapon skipped!");
                    }
                    else
                    {
                        //Debug.Log("Level: " + fields[0] + ", Name: " + fields[1] + ", Damage: " + fields[3]);
                        WeaponData data = new WeaponData();
                        data.itemID = Int32.Parse(fields[0]);
                        data.level = Int32.Parse(fields[1]);
                        data.name = fields[2];
                        data.basePrefab = GetPrefabType(fields[3]);
                        data.attackStrength = Int32.Parse(fields[4]);
                        data.knockback = Int32.Parse(fields[5]);
                        data.effectType = GetDamageType(fields[6]);
                        data.effectStrength = Int32.Parse(fields[7]);
                        data.effectDuration = Int32.Parse(fields[8]);
                        data.rarity = float.Parse(fields[9]);
                        data.goldCost = Int32.Parse(fields[10]);

                        m_weaponData.Add(data);
                    }
                }
                //Debug.Log("Weapons Loaded: " + m_weaponData.Count);
            }
        }
        else
            Destroy(gameObject);


    }

    void Start()
    {
        
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

    int GetItemID(List<WeaponData> weaponDataShortlist)
    {
        List<int> idList = new List<int>();

        for(int i = 0; i < weaponDataShortlist.Count; i++)
        {
            for(float j = 0; j < weaponDataShortlist[i].rarity; j += 0.01F)
            {
                idList.Add(i);
            }
        }

        return idList[UnityEngine.Random.Range(0, idList.Count)];
    }

    GameObject GenerateWeapon(List<WeaponData> weaponDataShortlist)
    {
        int id = GetItemID(weaponDataShortlist);

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

        weapon.transform.SetParent(m_weaponParent.transform);
        weapon.GetComponent<Weapon>().ApplyWeaponData(wepData);

        return weapon;
    }

    GameObject GenerateWeapon(WeaponData data)
    {
        GameObject weapon;

        switch (data.basePrefab)
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

        weapon.transform.SetParent(m_weaponParent.transform);
        weapon.GetComponent<Weapon>().ApplyWeaponData(data);

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

    public GameObject GenerateWeapon(string name, int level)
    {
        foreach (WeaponData data in m_weaponData)
        {
            if (data.level == level && data.name == name)
            {
                return GenerateWeapon(data);
            }
        }
        Debug.Log("WeaponManager: Failed to generate wepaon with name: " + name +" , and level: " + level);
        return null;
    }

    public GameObject GenerateWeaponFromID(int id)
    {
        foreach (WeaponData data in m_weaponData)
        {
            if(data.itemID == id)
            {
                return GenerateWeapon(data);
            }
        }
        Debug.Log("WeaponManager: Failed to generate wepaon with itemID: " + id);
        return null;
    }
}
