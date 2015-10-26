/* Copyright (c) Handsome Dragon Games
*  http://www.handsomedragongames.com
*  Script Created by:
*  Corey Underdown
*/
 
using UnityEngine;
using System.Collections.Generic;
 
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
		CROUCH, //Used to go into the lava and appear elsewhere
		BASH //Anubis bashes the wall, rocks fall
	}

	enum Side
	{
		LEFT,
		RIGHT
	}

	enum BattleStage
	{
		FIRST,
		SECOND,
		THIRD
	}

	public State curState = State.IDLE;
	Side curSide;
	BattleStage curStage = BattleStage.FIRST;

	Transform t;
	Rigidbody2D rb;

	public BoxCollider2D collider;
	public BoxCollider2D frontCollider;

	[Header("Spawn Enemies Variables")]
	public GameObject spawnEnemy;
	public List<GameObject> spawnPoints;
	public float enemyWaitTime = 20.0f;
	public float enemyWaitCounter = 0.0f;
	List<GameObject> spawnedEnemies;
	int enemiesCount;
	bool hasSpawnedEnemies = false;

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

	bool isFacingRight = false;

	void Start()
	{
		t = transform;
		rb = GetComponent<Rigidbody2D>();
		spawnedEnemies = new List<GameObject>();
	}

	public override void EnemyBehaviour()
	{
		CheckSide();
		DebugKeyboardCommands();
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

	public override Damage UpdateDamage(Damage dam)
	{
		dam.knockback = 0;
		return dam;
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
		if (!hasSpawnedEnemies)
		{
			hasSpawnedEnemies = true;
			for (int i = 0; i < 3; i++)
			{
				spawnedEnemies.Add((GameObject)Instantiate(spawnEnemy, spawnPoints[Random.Range(0, spawnPoints.Count)].transform.position, Quaternion.identity));
				enemiesCount++;
			}
		}

		for (int i = 0; i < enemiesCount; i++)
		{
			if (spawnedEnemies[i].GetComponent<Enemy>().m_hitPoints <= 0)
			{
				enemiesCount--;
				spawnedEnemies.RemoveAt(i);
			}
		}

		enemyWaitCounter += Time.deltaTime;
		if(enemyWaitCounter >= enemyWaitTime || spawnedEnemies.Count == 0)
		{
			for (int i = 0; i < enemiesCount; i++)
			{
				spawnedEnemies[i].GetComponent<Enemy>().m_hitPoints = 0;
				Destroy(spawnedEnemies[i]);
			}
			spawnedEnemies.Clear();
			enemiesCount = 0;
			curState = State.DASH;
		}
	}

	void UpdateProjectileAndEnemies()
	{

	}

	void UpdateDash()
	{
		if (dash == 0)
			dash = SideFloat(dashSpeed);

		rb.velocity = new Vector2(dash, 0f);
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

	void DebugKeyboardCommands()
	{
		if (Input.GetKeyDown(KeyCode.Keypad1))
			curState = State.IDLE;
		if (Input.GetKeyDown(KeyCode.Keypad2))
			curState = State.PROJECTILE;
		if (Input.GetKeyDown(KeyCode.Keypad3))
			curState = State.ENEMIES;
		if (Input.GetKeyDown(KeyCode.Keypad4))
			curState = State.PROJECTILEANDENEMIES;
		if (Input.GetKeyDown(KeyCode.Keypad5))
			curState = State.DASH;
		if (Input.GetKeyDown(KeyCode.Keypad6))
			curState = State.STUCK;
		if (Input.GetKeyDown(KeyCode.Keypad7))
			curState = State.ESCAPE;
		if (Input.GetKeyDown(KeyCode.Keypad8))
			curState = State.CROUCH;
		if (Input.GetKeyDown(KeyCode.Keypad9))
			curState = State.BASH;
	}

	void Flip()
	{
		isFacingRight = !isFacingRight;
		Vector3 scale = transform.localScale;
		scale.x *= -1;
		transform.localScale = scale;
	}

	void OnCollisionEnter2D(Collision2D col)
	{
		if (col.gameObject.tag == "Player")
		{
			col.gameObject.GetComponent<Damageable>().OnTakeDamage(m_attack.GetDamage(gameObject.transform));
			collider.isTrigger = true;
			//frontCollider.isTrigger = true;
		}

		if(col.gameObject.tag == "Solid")
		{
			if(curState == State.DASH)
			{
				curState = State.PROJECTILE;
				dash = 0;
				Flip();
			}
		}
	}

	void OnTriggerExit2D(Collider2D col)
	{
		collider.isTrigger = false;
		//frontCollider.isTrigger = false;
	}

	void OnTriggerEnter2D(Collider2D col)
	{
		if (col.gameObject.tag == "Solid")
		{
			if (curState == State.DASH)
			{
				curState = State.PROJECTILE;
				dash = 0;
				Flip();
			}
		}
	}
}