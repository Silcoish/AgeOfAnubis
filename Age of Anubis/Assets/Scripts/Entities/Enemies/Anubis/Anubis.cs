/* Copyright (c) Dungeon Crawlers
*  Script Created by:
*  Corey Underdown
*/

//TODO behaviour
/*
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

	private Animator anim;

	public GameObject UIObject;
	public UnityEngine.UI.Image UIImage;

	private float m_hpPercentage = 100;

	#region Battle State Variables
	[Header("Battle Stage Health Percentages")]
	float secondStateHPPercent = 60;
	float thirdStateHPPercent = 30;

	[Header("Intro State Variables")]
	public float introTime = 2.0f;
	float introTimeCounter = 0.0f;
	public float introMusicFadeTime = 3.0f;
	bool introAudioPlayed = false;

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
	bool dashPlayedAudio = false;
	public GameObject leftPos;
	public GameObject rightPos;
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
	public float bashTime = 8f;
	float bashCounter = 0.0f;
	#endregion

	bool isFacingRight = false;

	void Start()
	{
		t = transform;
		anim = GetComponent<Animator>();
		rb = GetComponent<Rigidbody2D>();
		spawnedEnemies = new List<GameObject>();

		if (GameManager.inst.bossHPObject != null)
		{
			UIObject = GameManager.inst.bossHPObject;
			UIObject.SetActive(true);
			UIImage = UIObject.transform.GetChild(0).GetComponent<UnityEngine.UI.Image>();
		}
		else
		{
			print("Cannot find boss hp");
		}
	}

	public override void EnemyBehaviour()
	{
		CalculateHPPercentage();
		CheckSide();
		CheckForOutOfBounds();
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
		AudioManager.Inst.PlaySFX(AudioManager.Inst.a_anubis_takeDamage);
		dam.knockback = 0;
		if(m_hpPercentage <= thirdStateHPPercent)
		{
			curState = State.IDLE;
		}
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
			AudioManager.Inst.FadeMusic(AudioManager.Inst.s_rumble);
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
		if(!introAudioPlayed)
		{
			introAudioPlayed = true;
			AudioManager.Inst.PlaySFX(AudioManager.Inst.a_anubis_intro);
		}

		introTimeCounter += Time.deltaTime;
		if(introTimeCounter >= introTime)
		{
			AudioManager.Inst.FadeMusic(AudioManager.Inst.s_boss, introMusicFadeTime);
			FinishedState();
		}
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
			AudioManager.Inst.PlaySFX(AudioManager.Inst.a_anubis_fireball);
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
			AudioManager.Inst.PlaySFX(AudioManager.Inst.a_anubis_dashCharge);
		}

		dashBufferCounter += Time.deltaTime;

		if(dashBufferCounter >= dashBufferTime)
		{
			if(!dashPlayedAudio)
			{
				AudioManager.Inst.PlaySFX(AudioManager.Inst.a_anubis_dash);
				anim.SetTrigger("dash");
				dashPlayedAudio = true;
			}
			rb.velocity = new Vector2(dash, 0f);

			print("Dash : " + dash);
			if(dash == -1)
			{
				if (t.position.x >= rightPos.transform.position.x)
				{
					t.position = new Vector2(rightPos.transform.position.x, t.position.y);
					FinishedState();
				}
			}
			else
			{
				if (t.position.x <= leftPos.transform.position.x)
				{
					t.position = new Vector2(leftPos.transform.position.x, t.position.y);
					FinishedState();
				}
			}
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
		FallingRocks();
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
		FallingRocks();
		//do animation here
		bashCounter += Time.deltaTime;
		if(bashCounter >= bashTime)
		{
			bashCounter = 0;
			FinishedState();
		}
	}

	void FallingRocks()
	{
		if (Random.Range(0, spawnFrequency) == 0)
		{
			Instantiate(fallingRock, new Vector2(m_room.transform.position.x + Random.Range(rockBounds.x, rockBounds.y), m_room.transform.position.y + rockStartHeight), Quaternion.Euler(new Vector3(0, 0, Random.Range(0, 360))));
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
		anim.SetTrigger("idle");
	}

	void ChooseNextState1()
	{
		if (prevStates[prevStates.Count - 1] == State.DASH)
		{
			curState = State.STUCK;
			anim.SetTrigger("stuck");
			for(int i = 0; i < dashPlatforms.Count; i++)
			{
				dashPlatforms[i].GetComponent<BoxCollider2D>().isTrigger = true;
			}
			dash = 0;
			dashBufferCounter = 0;
			dashPlayedAudio = false;
		}

		if (prevStates[prevStates.Count - 1] == State.STUCK || prevStates[prevStates.Count - 1] == State.INTRO)
		{
			if (prevStates[prevStates.Count - 1] == State.STUCK)
				Flip();
			if (Random.Range(0, 2) == 0)
			{
				curState = State.PROJECTILE;
				anim.SetTrigger("idle");
			}
			else
			{
				curState = State.ENEMIES;
				anim.SetTrigger("idle");
			}
		}

		if (prevStates[prevStates.Count - 1] == State.ENEMIES)
		{
			if (Random.Range(0, 3) == 0)
			{
				curState = State.PROJECTILE;
				anim.SetTrigger("idle");
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
				anim.SetTrigger("idle");
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
		lava.gameObject.SetActive(true);
		lava.isActive = true;

		for(int i = 0; i < dashPlatforms.Count; i++)
		{
			dashPlatforms[i].GetComponent<BoxCollider2D>().isTrigger = false;
		}

		if (prevStates[prevStates.Count - 1] == State.BASH)
		{
			curState = State.CROUCH;
			anim.SetTrigger("crouch");
		}
		else
		{
			curState = State.BASH;
			anim.SetTrigger("bash");
		}

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

	void CheckForOutOfBounds()
	{
		if(t.position.x <= leftPos.transform.position.x)
		{
			t.position = new Vector2(leftPos.transform.position.x, t.position.y);
		}
		else if(t.position.x >= rightPos.transform.position.x)
		{
			t.position = new Vector2(rightPos.transform.position.x, t.position.y);
		}
	}

	void CalculateHPPercentage()
	{
		m_hpPercentage = ((float)m_hitPoints / (float)m_maxHitpoints) * 100;

		if(UIImage != null)
		{
			UIImage.fillAmount = m_hpPercentage / 100;
		}
	}

	public override void OnDeath()
	{
		LoadingManager.Inst.LoadLevel("Credits", false);
		Destroy(GameManager.inst.player);
		Destroy(gameObject);
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
			//if(curState == State.DASH)
			//{
			//	FinishedState();
			//}/
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
			//if (curState == State.DASH)
			//{
			//	FinishedState();
			//}
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