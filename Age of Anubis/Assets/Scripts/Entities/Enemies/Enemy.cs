using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Enemy : Damageable
{
	[Header("EnemyStats")]
	public Attack m_attack;
	private BoxCollider2D m_colBox;
	private CircleCollider2D m_colCircle;
    public GameObject m_deathParticle;

	private BoxCollider2D m_forwardCol;
	private BoxCollider2D m_belowCol;
	private BoxCollider2D m_cornerCol;

	protected RoomObject m_room;

	protected enum CheckReturnEnum { None, Grounded, ReachedEdge, ReachedWall };

	protected struct CheckReturn
	{
		public CheckReturnEnum type;
		public Collider2D colFront;
		public Collider2D colBelow;
	};


	public override void AwakeOverride()
	{
		m_colBox = GetComponent<BoxCollider2D>();
		m_colCircle = GetComponent<CircleCollider2D>();

		//CreateDirectionColliders();
	}

	// Update is called once per frame
	public override void UpdateOverride()
	{
		EnemyBehaviour();
	}

	public virtual void EnemyBehaviour()
	{
		
		if (CheckEnemyLocation().type == CheckReturnEnum.None)
		{
			//print("Airborne");
		}
		else if (CheckEnemyLocation().type == CheckReturnEnum.ReachedEdge)
		{
			//print("ReachedEdge");
		}
		else if (CheckEnemyLocation().type == CheckReturnEnum.ReachedWall)
		{
			//print("ReachedWall");
		}
		
	}


	public override void OnDeath()
	{
		if(m_room)
            m_room.EnemyDied(this);
        if(m_deathParticle)
            Instantiate(m_deathParticle, transform.position, transform.rotation);
        if(GameManager.inst.coinPrefab)
            Instantiate(GameManager.inst.coinPrefab, transform.position, transform.rotation);
		gameObject.SetActive(false);
	}


	void OnCollisionEnter2D(Collision2D col)
	{
		if (col.gameObject.tag == "Player")
		{
			col.gameObject.GetComponent<Damageable>().OnTakeDamage(m_attack.GetDamage(gameObject.transform));
		}
	}

	void CreateDirectionColliders()
	{
		float offset = 0.05f;
		float depth = 0.4f;

		m_forwardCol = gameObject.AddComponent<BoxCollider2D>();
		m_belowCol = gameObject.AddComponent<BoxCollider2D>();
		m_cornerCol = gameObject.AddComponent<BoxCollider2D>();

		m_forwardCol.isTrigger = true;
		m_belowCol.isTrigger = true;
		m_cornerCol.isTrigger = true;

		if (m_colBox != null)
		{
			//print("Created From Box");
			m_forwardCol.size = new Vector2(offset * 2, m_colBox.size.y - offset);
			m_belowCol.size = new Vector2(m_colBox.size.x - offset, depth);
			m_cornerCol.size = new Vector2(depth,depth);

			m_forwardCol.offset = new Vector2(m_colBox.offset.x + (m_colBox.size.x / 2) + (offset), m_colBox.offset.y);
			m_belowCol.offset = new Vector2(m_colBox.offset.x, m_colBox.offset.y - (m_colBox.size.y / 2) - offset - (depth / 2));
			m_cornerCol.offset = new Vector2(m_colBox.offset.x + (m_colBox.size.x / 2) + offset + (depth / 2), m_colBox.offset.y - (m_colBox.size.y / 2) - offset - (depth / 2));

		}
		else if (m_colCircle != null)
		{
			//print("Created From Circle");
			m_forwardCol.size = new Vector2(depth, m_colCircle.radius * 2 - offset);
			m_belowCol.size = new Vector2(m_colCircle.radius * 2 - offset, depth);
			m_cornerCol.size = new Vector2(depth, depth);

			m_forwardCol.offset = new Vector2(m_colCircle.offset.x + m_colCircle.radius + offset + (depth / 2), m_colCircle.offset.y);
			m_belowCol.offset = new Vector2(m_colCircle.offset.x, m_colCircle.offset.y - m_colCircle.radius - offset - (depth / 2));
			m_cornerCol.offset = new Vector2(m_colCircle.offset.x + m_colCircle.radius + offset + (depth / 2), m_colCircle.offset.y - m_colCircle.radius - offset - (depth / 2));
		}
		else
		{
			Debug.LogError("Broken no collider on Enemy: BOX or CIRCLE only");
		}

	}

	protected CheckReturn CheckEnemyLocation()
	{
		CheckReturn tempReturn = new CheckReturn();
		tempReturn.colBelow = null;
		tempReturn.colFront = null;
		tempReturn.type = CheckReturnEnum.None;

		Collider2D[] allColls;

		//CheckBelow
		
		allColls = Physics2D.OverlapAreaAll(m_belowCol.bounds.min, m_belowCol.bounds.max);
		

		foreach (var col in allColls)
		{
			if (col.gameObject.CompareTag("NotSolid") || col.gameObject.CompareTag("Solid") || col.gameObject.CompareTag("Wall"))
			{
				tempReturn.colBelow = col;
				tempReturn.type = CheckReturnEnum.Grounded;
				break;
			}
		}

		if (tempReturn.colBelow != null)
		{
			//Check Front
			allColls = Physics2D.OverlapAreaAll(m_forwardCol.bounds.min, m_forwardCol.bounds.max);

			foreach (var col in allColls)
			{
				if (col.gameObject.CompareTag("NotSolid") || col.gameObject.CompareTag("Solid") || col.gameObject.CompareTag("Wall"))
				{
					tempReturn.colFront = col;
					tempReturn.type = CheckReturnEnum.ReachedWall;
					break;
				}
			}

			if (tempReturn.colFront == null)
			{
				//Check Corner
				allColls = Physics2D.OverlapAreaAll(m_cornerCol.bounds.min, m_cornerCol.bounds.max);

				bool colWithWall = false;

				foreach (var col in allColls)
				{
					if (col.gameObject.CompareTag("NotSolid") || col.gameObject.CompareTag("Solid") || col.gameObject.CompareTag("Wall"))
					{
						colWithWall = true;
					}
				}

				if (!colWithWall)
				{
					tempReturn.type = CheckReturnEnum.ReachedEdge;
				}

			}
			
		}

		return tempReturn;
	}

	public void SetRoom(RoomObject r)
	{
		m_room = r;
	}

}
