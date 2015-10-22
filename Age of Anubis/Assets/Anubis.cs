/* Copyright (c) Handsome Dragon Games
*  http://www.handsomedragongames.com
*  Script Created by:
*  Corey Underdown
*/
 
using UnityEngine;
 
public class Anubis : Enemy
{
	public enum State
	{
		IDLE,
		PROJECTILE,
		ENEMIES,
		PROJECTILEANDENEMIES,
		DASH,
		STUCK,
		ESCAPE, //Breaking out of the wall after being stuck
		CROUCH //Used to go into the lava and appear elsewhere
	}

	enum Side
	{
		LEFT,
		RIGHT
	}

	public State curState = State.IDLE;
	Side curSide;

	Transform t;

	[Header("Projectile State Variables")]
	public GameObject m_projectile;
	public float m_projectileHorizSpeed;
	public float m_projectileVertSpeed;
	public float m_projectileTopRow;
	public float m_projectileMiddleRow;
	public float m_projectileBottomRow;
	public float m_projectileCooldown;
	float m_projectileCounter;

	[Header("Dash State Variables")]
	public float dashSpeed;
	public float raycastLength = 1.0f;
	float dash;

	void Start()
	{
		t = transform;
	}

	public override void EnemyBehaviour()
	{
		CheckSide();
		switch (curState)
		{
			case State.IDLE:
				UpdateIdle();
				break;
			case State.PROJECTILE:
				UpdateProjectile();
				break;
			case State.ENEMIES:
				UpdateEnemies();
				break;
			case State.PROJECTILEANDENEMIES:
				UpdateProjectileAndEnemies();
				break;
			case State.DASH:
				UpdateDash();
				break;
			case State.STUCK:
				UpdateStuck();
				break;
			case State.ESCAPE:
				UpdateEscape();
				break;
			case State.CROUCH:
				UpdateCrouch();
				break;
			default:
				Debug.LogError("Invalid State for Anubis");
				break;
		}
	}

	#region StateUpdates
	void UpdateIdle()
	{

	}

	void UpdateProjectile()
	{
		m_projectileCounter += Time.deltaTime;
		if (m_projectileCounter > m_projectileCooldown)
		{
			m_projectileCounter = 0.0f;
			GameObject tempProjectile = (GameObject)Instantiate(m_projectile, transform.position, Quaternion.identity);
			tempProjectile.GetComponent<AnubisProjectile>().Setup(SideFloat(m_projectileHorizSpeed), m_projectileVertSpeed, ChooseLayer());
		}
	}

	void UpdateEnemies()
	{

	}

	void UpdateProjectileAndEnemies()
	{

	}

	void UpdateDash()
	{
		if (dash == 0)
			dash = SideFloat(dashSpeed);
		t.position = new Vector2(t.position.x + dash * Time.deltaTime, t.position.y);

		RaycastHit2D hit = Physics2D.Raycast((Vector2)transform.position + new Vector2(-SideFloat(1f), 0) * 1.2f, new Vector2(-SideFloat(1f), 0f), raycastLength);
		Debug.DrawRay((Vector2)transform.position + new Vector2(-SideFloat(1f), 0) * 1.2f, Vector2.left, Color.green);
		if(hit.collider != null)
		{
			if(hit.collider.tag == "Solid")
			{
				curState = State.PROJECTILE;
				dash = 0;
			}
		}
	}

	void UpdateStuck()
	{

	}

	void UpdateEscape()
	{

	}

	void UpdateCrouch()
	{

	}
	#endregion

	void CheckSide()
	{
		if (t.position.x < m_room.gameObject.transform.position.x)
		{
			curSide = Side.LEFT;
		}
		else
		{
			curSide = Side.RIGHT;
		}
	}

	float ChooseLayer()
	{
		int r = Random.Range(0, 3);
		if (r == 0)
			return m_projectileTopRow;
		if (r == 1)
			return m_projectileMiddleRow;

		return m_projectileBottomRow;
	}

	float SideFloat(float f)
	{
		if (curSide == Side.LEFT)
			return f;
		else
			return -f;
	}
}