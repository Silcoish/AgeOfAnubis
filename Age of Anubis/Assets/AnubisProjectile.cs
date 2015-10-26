/* Copyright (c) Handsome Dragon Games
*  http://www.handsomedragongames.com
*  Script Created by:
*  Corey Underdown
*/
 
using UnityEngine;
 
public class AnubisProjectile : Enemy
{
	float vMoveSpeed;
	float hMoveSpeed;
	float yDest = 0;
	public float lifeTime = 10.0f;
	float lifeTimeCounter = 0.0f;
	public CircleCollider2D collider;

	public override void EnemyBehaviour()
	{
		lifeTimeCounter += Time.deltaTime;
		transform.position = new Vector2(transform.position.x + hMoveSpeed * Time.deltaTime, Mathf.Lerp(transform.position.y, yDest, vMoveSpeed));

		if(lifeTimeCounter >= lifeTime)
		{
			Destroy(gameObject);
		}
	}

	//Parameters: Horizontal Speed, Vertical Speed, Y Destination
	public void Setup(float hSpeed, float vSpeed, float y)
	{
		hMoveSpeed = hSpeed;
		vMoveSpeed = vSpeed;
		yDest = transform.position.y + y;
	}

	void OnCollisionEnter2D(Collision2D col)
	{
		if (col.gameObject.tag == "Player")
		{
			col.gameObject.GetComponent<Damageable>().OnTakeDamage(m_attack.GetDamage(gameObject.transform));
			collider.isTrigger = true;
		}
	}

	void OnTriggerExit2D(Collider2D col)
	{
		collider.isTrigger = false;
	}
}