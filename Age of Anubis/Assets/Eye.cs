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
	public float closestDistance = 1;

	void Start()
	{
		anim = GetComponent<Animator>();
		t = transform;
	}

	public override void EnemyBehaviour()
	{
		float dis = Mathf.Abs(t.position.x - GameManager.inst.player.transform.position.x);
		Vector2 moveVec = new Vector2(t.position.x, t.position.y);

		if(dis > closestDistance)
		{
			//move towards player	
			moveVec.x = Mathf.Lerp(t.position.x, GameManager.inst.player.transform.position.x, 0.05f);
		}
		else
		{
			//move away from player
			if(t.position.x > GameManager.inst.player.transform.position.x)
			{
				moveVec.x = Mathf.Lerp(t.position.x, GameManager.inst.player.transform.position.x + closestDistance, 0.005f);
			}
			else
			{
				moveVec.x = Mathf.Lerp(t.position.x, GameManager.inst.player.transform.position.x - closestDistance, 0.02f);
			}
		}

		transform.position = moveVec;
	}

}