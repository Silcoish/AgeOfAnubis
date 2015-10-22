/* Copyright (c) Handsome Dragon Games
*  http://www.handsomedragongames.com
*  Script Created by:
*  Corey Underdown
*/
 
using UnityEngine;
 
public class Eye : Enemy
{
	Animator anim;
	Transform t;
	public float closestDistance = 1.0f;
	public float shootRange = 1.0f;

	bool charging = false;
	public float chargeTimer = 2.0f;
	float chargeCounter = 0.0f;

	void Start()
	{
		anim = GetComponent<Animator>();
		t = transform;
	}

	public override void EnemyBehaviour()
	{
		if(!charging)
		{
			HorizontalMovement();
			VerticalMovement();
		}
		else
		{
			anim.SetBool("charging", true);
			Charging();
		}
	}

	void VerticalMovement()
	{
		float dis = Mathf.Abs(t.position.y - GameManager.inst.player.transform.position.y);

		if(dis < shootRange)
		{
			charging = true;
		}
	}

	void HorizontalMovement()
	{
		float dis = Mathf.Abs(t.position.x - GameManager.inst.player.transform.position.x);
		Vector2 moveVec = new Vector2(t.position.x, t.position.y);

		if (dis > closestDistance)
		{
			//move towards player	
			moveVec.x = Mathf.Lerp(t.position.x, GameManager.inst.player.transform.position.x, 0.05f);
		}
		else
		{
			//move away from player
			if (t.position.x > GameManager.inst.player.transform.position.x)
			{
				moveVec.x = Mathf.Lerp(t.position.x, GameManager.inst.player.transform.position.x + closestDistance, 0.02f);
			}
			else
			{
				moveVec.x = Mathf.Lerp(t.position.x, GameManager.inst.player.transform.position.x - closestDistance, 0.02f);
			}
		}

		transform.position = moveVec;
	}

	void Charging()
	{
		chargeCounter += Time.deltaTime;

		if(chargeCounter >= chargeTimer)
		{
			charging = false;
			anim.SetBool("charging", false);
			chargeCounter = 0.0f;
		}
	}

}