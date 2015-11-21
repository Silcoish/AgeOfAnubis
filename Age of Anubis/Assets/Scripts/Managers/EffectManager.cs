using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class EffectManager : MonoBehaviour 
{
    public static EffectManager Inst;


    public GameObject m_damageEffect_Poison;
    public GameObject m_damageEffect_None;

    void Awake()
    {
        if (Inst == null)
            Inst = this;

    }

    public void CreateDamageableEffect(Vector3 pos, DamageType type, int amount)
    {
        GameObject temp;

        switch (type)
        {
            case DamageType.POISON: temp = Instantiate(m_damageEffect_Poison, pos, m_damageEffect_Poison.transform.rotation) as GameObject; break;
            case DamageType.NONE: temp = Instantiate(m_damageEffect_None, pos, m_damageEffect_None.transform.rotation) as GameObject; break;
            default: temp = Instantiate(m_damageEffect_None, pos, m_damageEffect_None.transform.rotation) as GameObject; break;
            
        }

        temp.GetComponentInChildren<Text>().text = "-" + amount.ToString();

        Destroy(temp, 1);
    }


	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
