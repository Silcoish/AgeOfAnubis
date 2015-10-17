using UnityEngine;
using System.Collections;

[System.Serializable]
public struct Attack 
{
	public int m_attackStrength ;
	public DamageType m_effectType;
	public float m_effectDuration;
	public float m_effectStrength;
	public float m_knockbackForce ;

	public Damage GetDamage(Transform transform)
	{
		Damage temp;

		temp.type = m_effectType;
		temp.amount = m_attackStrength;
		temp.knockback = m_knockbackForce;
		temp.fromGO = transform;
		temp.effectTime = m_effectDuration;
		temp.effectStrength = m_effectStrength;

		return temp;
	}
}
