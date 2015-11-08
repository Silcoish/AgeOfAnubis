/* Copyright (c) Handsome Dragon Games
*  http://www.handsomedragongames.com
*  Script Created by:
*  Corey Underdown
*/
 
using UnityEngine;
 
public class FallingRock : Enemy
{
	Transform t;
	float startYPos;
	public float destroyDistance = 20;

	void Start()
	{
		t = transform;
		startYPos = t.position.y;
	}

	public override void EnemyBehaviour()
	{
		if(Mathf.Abs(t.transform.position.y - startYPos) >= destroyDistance)
		{
			Destroy(gameObject);
		}
	}

	void OnCollisionStay2D(Collision2D col)
	{
		if (col.gameObject.tag == "Player")
		{
			col.gameObject.GetComponent<Damageable>().OnTakeDamage(m_attack.GetDamage(gameObject.transform));
			Destroy(gameObject);
		}
	}
}