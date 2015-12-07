using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class EffectManager : MonoBehaviour 
{
    public static EffectManager Inst;


    
    public GameObject m_damageEffect_None;
    public GameObject m_damageEffect_Poison;
    public GameObject m_damageEffect_Burn;
	public GameObject m_damageEffect_Bleed;

    void Awake()
    {
        if (Inst == null)
            Inst = this;

    }

    public void CreateDamageableEffect(Transform trans, DamageType type, int amount)
    {
        GameObject temp;

        switch (type)
        {
            case DamageType.POISON: temp = Instantiate(m_damageEffect_Poison, trans.position, m_damageEffect_Poison.transform.rotation) as GameObject; break;
            case DamageType.NONE: temp = Instantiate(m_damageEffect_None, trans.position, m_damageEffect_None.transform.rotation) as GameObject; break;
            case DamageType.BURN: temp = Instantiate(m_damageEffect_Burn, trans.position, m_damageEffect_Burn.transform.rotation) as GameObject; break;
			case DamageType.BLEED: temp = Instantiate(m_damageEffect_Bleed, trans.position, m_damageEffect_Bleed.transform.rotation) as GameObject; break;
            default: temp = Instantiate(m_damageEffect_None, trans.position, m_damageEffect_None.transform.rotation) as GameObject; break;
            
        }

        temp.transform.SetParent(trans);

        temp.GetComponentInChildren<Text>().text = "-" + amount.ToString();

        Destroy(temp, 1);
    }

}
