/* Copyright (c) Dungeon Crawlers
*  Script Created by:
*  Corey Underdown
*/

using UnityEngine;
 
public class FallingRock : Enemy
{
	Transform t;
	float startYPos;
	public float destroyDistance = 20;
	public Vector2 minMaxSize = new Vector2(0.5f, 1.5f);

	void Start()
	{
		t = transform;
		startYPos = t.position.y;
		float size = Random.Range(minMaxSize.x, minMaxSize.y);
		t.localScale = new Vector2(size, size);
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