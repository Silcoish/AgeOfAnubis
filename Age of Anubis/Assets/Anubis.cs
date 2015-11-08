/* Copyright (c) Dungeon Crawlers
*  Script Created by:
*  Corey Underdown
*/

//TODO behaviour
/*
 * Platforms raise on dash
 * Delay before Dashing
 * Bashing wall
 * Spawn Enemies and Projectiles at once
 * Do stage 2 and 3 behaviour picks
 * ANIMATIONS
 */
 
using UnityEngine;
using System.Collections.Generic;
 
public class Anubis : Enemy
{
	public enum State
	{
		IDLE,
		INTRO,
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
	List<State> prevStates = new List<State>();
	Side curSide;
	BattleStage curStage = BattleStage.FIRST;

	Transform t;
	Rigidbody2D rb;

	public BoxCollider2D collider;
	public BoxCollider2D frontCollider;

	public GameObject UIObject;
	public UnityEngine.UI.Image UIImage;

	private float m_hpPercentage = 100;

	[Header("Battle Stage Health Percentages")]
	float secondStateHPPercent = 60;
	float thirdStateHPPercent = 30;

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
	public int m_howManyShots = 5;
	int m_shots = 0;

	[Header("Dash State Variables")]
	public float dashSpeed;
	public float raycastLength = 1.0f;
	public float dashBufferTime = 5.0f;
	public float dashBufferCounter = 0.0f;
	float dash;
	public List<GameObject> dashPlatforms;

	[Header("Stuck State Variables")]
	public float m_stuckTime = 5f;
	float m_stuckTimeCounter = 0.0f;
	public float m_maxPercentageHPTakenStuck = 5f;
	bool setStartHealth;
	float stuckStartHPPercent;

	[Header("Crouch State Variables")]
	public float crouchSpeed = 10.0f;
	public float crouchDistanceDown = 10.0f;
	public float crouchOffset = 10.0f;
	public float crouchWaitTime = 2.0f;
	float crouchWaitCounter = 0.0f;
	bool isMovingDown = true;
	Vector2 crouchStartPos = Vector2.zero;

	[Header("Bashing State Variables")]
	public GameObject fallingRock;
	public float rockStartHeight = 10.5f;
	[Tooltip("x = left bound, y = right bounds")] public Vector2 rockBounds;
	public int spawnFrequency = 60;
	public Lava lava;

	bool isFacingRight = false;

	void Start()
	{
		t = transform;
		rb = GetComponent<Rigidbody2D>();
		spawnedEnemies = new List<GameObject>();
		if(UIObject != null)
		{
			UIObject.SetActive(true);
		}
	}

	public override void EnemyBehaviour()
	{
		CalculateHPPercentage();
		CheckSide();
		switch (curState)
		{
			case State.IDLE:
				UpdateIdle();
				break;
			case State.INTRO:
				UpdateIntro();
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
			case State.BASH:
				UpdateBash();
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
		if (m_hpPercentage <= secondStateHPPercent && m_hpPercentage > thirdStateHPPercent)
		{
			curStage = BattleStage.SECOND;
		}
		else if(m_hpPercentage <= thirdStateHPPercent)
		{
			curStage = BattleStage.THIRD;
		}

		if(prevStates.Count >= 1)
		{
			if(curStage == BattleStage.FIRST)
			{
				ChooseNextState1();
			}
			else if (curStage == BattleStage.SECOND)
			{
				ChooseNextState2();
			}
			else
			{
				ChooseNextState3();
			}
		}
		else
		{
			curState = State.INTRO;
		}
	}

	void UpdateIntro()
	{
		FinishedState();
	}

	void UpdateProjectile()
	{
		m_projectileCounter += Time.deltaTime;
		if (m_projectileCounter > m_projectileCooldown)
		{
			m_projectileCounter = 0.0f;
			GameObject tempProjectile = (GameObject)Instantiate(m_projectile, transform.position, Quaternion.identity);
			tempProjectile.GetComponent<AnubisProjectile>().Setup(SideFloat(m_projectileHorizSpeed), m_projectileVertSpeed, ChooseLayer());
			m_shots++;

		}

		if (m_shots >= m_howManyShots)
		{
			FinishedState();
			m_shots = 0;
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
			FinishedState();
		}
	}

	void UpdateProjectileAndEnemies()
	{
		UpdateProjectile();
		UpdateEnemies();
	}

	void UpdateDash()
	{
		if (dash == 0)
		{
			dash = SideFloat(dashSpeed);
			for(int i = 0; i < dashPlatforms.Count; i++)
			{
				dashPlatforms[i].GetComponent<BoxCollider2D>().isTrigger = false;
			}
		}

		dashBufferCounter += Time.deltaTime;

		if(dashBufferCounter >= dashBufferTime)
		{
			rb.velocity = new Vector2(dash, 0f);
		}

	}

	void UpdateStuck()
	{
		CalculateHPPercentage();
		if(!setStartHealth)
		{
			stuckStartHPPercent = m_hpPercentage;
			setStartHealth = true;
		}

		if(stuckStartHPPercent - m_hpPercentage >= m_maxPercentageHPTakenStuck)
		{
			FinishedState();
			return;
		}

		m_stuckTimeCounter += Time.deltaTime;

		if(m_stuckTimeCounter > m_stuckTime)
		{
			setStartHealth = false;
			m_stuckTimeCounter = 0;
			FinishedState();
		}
	}

	void UpdateEscape()
	{

	}

	void UpdateCrouch()
	{
		if(crouchStartPos == Vector2.zero)
		{
			crouchStartPos = (Vector2)t.position;
		}

		if (isMovingDown)
		{
			t.position = new Vector2(t.position.x, t.position.y - crouchSpeed * Time.deltaTime);
			if(Mathf.Abs(crouchStartPos.y - t.position.y) >= crouchDistanceDown )
			{
				isMovingDown = false;
				if(curSide == Side.LEFT)
				{
					t.position = new Vector2(t.position.x + crouchOffset, t.position.y);
				}
				else
				{
					t.position = new Vector2(t.position.x - crouchOffset, t.position.y);
				}
				Flip();
			}
		}
		else
		{
			crouchWaitCounter += Time.deltaTime;
			if(crouchWaitCounter >= crouchWaitTime)
			{
				t.position = new Vector2(t.position.x, t.position.y + crouchSpeed * Time.deltaTime);

				if(t.position.y >= crouchStartPos.y)
				{
					t.position = new Vector2(t.position.x, crouchStartPos.y);
					isMovingDown = true;
					crouchWaitCounter = 0.0f;
					crouchStartPos = Vector2.zero;
					FinishedState();
				}
			}
		}
	}

	void UpdateBash()
	{
		if(Random.Range(0, spawnFrequency) == 0)
		{
			Instantiate(fallingRock, new Vector2(Random.Range(rockBounds.x, rockBounds.y), rockStartHeight), Quaternion.Euler(new Vector3(0, 0, Random.Range(0, 360))));
		}
	}

	void FinishedState()
	{
		if(prevStates.Count >= 3)
		{
			prevStates.RemoveAt(0);
		}
		prevStates.Add(curState);

		curState = State.IDLE;
	}

	void ChooseNextState1()
	{
		if (prevStates[prevStates.Count - 1] == State.DASH)
		{
			curState = State.STUCK;
			for(int i = 0; i < dashPlatforms.Count; i++)
			{
				dashPlatforms[i].GetComponent<BoxCollider2D>().isTrigger = true;
			}
			dash = 0;
			dashBufferCounter = 0;
		}

		if (prevStates[prevStates.Count - 1] == State.STUCK || prevStates[prevStates.Count - 1] == State.INTRO)
		{
			if (prevStates[prevStates.Count - 1] == State.STUCK)
				Flip();
			if (Random.Range(0, 2) == 0)
			{
				curState = State.PROJECTILE;
			}
			else
			{
				curState = State.ENEMIES;
			}
		}

		if (prevStates[prevStates.Count - 1] == State.ENEMIES)
		{
			if (Random.Range(0, 3) == 0)
			{
				curState = State.PROJECTILE;
			}
			else
			{
				curState = State.DASH;
			}
		}

		if (prevStates[prevStates.Count - 1] == State.PROJECTILE)
		{
			if (Random.Range(0, 3) == 0)
			{
				curState = State.ENEMIES;
			}
			else
			{
				curState = State.DASH;
			}
		}
	}

	void ChooseNextState2()
	{
		ChooseNextState1();
	}

	void ChooseNextState3()
	{
		curState = State.BASH;
		for(int i = 0; i < dashPlatforms.Count; i++)
		{
			dashPlatforms[i].GetComponent<BoxCollider2D>().isTrigger = false;
		}
		lava.gameObject.SetActive(true);
		lava.isActive = true;
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

	void Flip()
	{
		isFacingRight = !isFacingRight;
		Vector3 scale = transform.localScale;
		scale.x *= -1;
		transform.localScale = scale;
	}

	void CalculateHPPercentage()
	{
		m_hpPercentage = ((float)m_hitPoints / (float)m_maxHitpoints) * 100;

		if(UIImage != null)
		{
			UIImage.fillAmount = m_hpPercentage / 100;
		}
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
				FinishedState();
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
				FinishedState();
			}
		}
	}

	void OnDestroy()
	{
		if(UIObject != null)
		{
			UIObject.SetActive(false);
		}
	}
}