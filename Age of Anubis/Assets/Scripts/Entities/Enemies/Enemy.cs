using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Enemy : Damageable
{
	[Header("EnemyStats")]
	public Attack m_attack;


	// Update is called once per frame
	public override void UpdateOverride()
	{
		EnemyBehaviour();
	}

	public virtual void EnemyBehaviour()
	{

	}

	public override void OnDeath()
	{
		gameObject.SetActive(false);
	}


	void OnCollisionEnter2D(Collision2D col)
	{
		if (col.gameObject.tag == "Player")
		{
			col.gameObject.GetComponent<Damageable>().OnTakeDamage(m_attack.GetDamage(gameObject.transform));
		}
	}


}
