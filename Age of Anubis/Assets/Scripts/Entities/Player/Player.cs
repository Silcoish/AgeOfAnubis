﻿using UnityEngine;
using System.Collections;

public class Player : Damageable 
{
	[Header("PlayerStats")]
    public float m_moveSpeed;
    public float m_jumpHeight;
    public int m_jumpNumber;
	public int m_jumpMax;
	public float m_groundedCheckDistance;

	private CircleCollider2D m_colFeet;
	private BoxCollider2D m_colBody;
    private bool m_isJumping;
	public int m_jumpCounter;
	private Vector2 m_velocity = Vector2.zero;

	public override void AwakeOverride() 
    {
		m_colFeet = GetComponent<CircleCollider2D>();
		m_colBody = GetComponent<BoxCollider2D>();
	}

	public override void UpdateOverride()
    {
		CheckIfGrounded();

		Check2WayPlatforms();

		PlayerInput();	
    }

	///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	
	void PlayerInput()
	{
		//Set velocity to current to maintain any current velocity if not effected by the player.
		m_velocity = m_rb.velocity;


		float move = Input.GetAxisRaw("Horizontal");
		m_velocity.x = move * m_moveSpeed * m_globalMoveSpeed;


		if (Input.GetButtonDown("Jump"))
		{
			Jump();
		}


		if (Input.GetButtonUp("Jump"))
		{
			JumpStop();
		}

		m_rb.velocity = m_velocity;
	}

	void Jump()
	{
		if (m_jumpCounter > 0)
		{
			print("Conter > 0");
			m_velocity.y = m_jumpHeight * m_globalMoveSpeed;
			m_jumpCounter--;
		}
	}

	void JumpStop()
	{
		if (m_velocity.y > 0)
		{
			m_velocity.y = 0;
		}

	}

	void CheckIfGrounded()
	{
		if (m_rb.velocity.y <= 0)
		{
			RaycastHit2D[] allHits = Physics2D.CircleCastAll((Vector2)(transform.position) + (Vector2)m_colFeet.offset, m_colFeet.radius, Vector2.down, m_groundedCheckDistance);

			foreach (var hit in allHits)
			{
				if (hit.collider.CompareTag("NotSolid") || hit.collider.CompareTag("Solid") || hit.collider.CompareTag("Wall"))
				{
					//Check if Player is above obect
					if (m_colFeet.bounds.min.y >= hit.collider.bounds.max.y)
					{
						m_jumpCounter = m_jumpMax;
					}
				}
			}
		}
	}

	void Check2WayPlatforms()
	{
		Vector2 offsetDistance = new Vector2(2, 2);

		Collider2D[] allCols = Physics2D.OverlapAreaAll((Vector2)m_colFeet.bounds.min - offsetDistance, (Vector2)m_colFeet.bounds.max + offsetDistance);

		foreach (var col in allCols)
		{
			if (col.CompareTag("NotSolid"))
			{
				if (m_colFeet.bounds.min.y < col.bounds.max.y)
				{
					Physics2D.IgnoreCollision(m_colFeet, col, true);
					Physics2D.IgnoreCollision(m_colBody, col, true);
				}
				else
				{
					Physics2D.IgnoreCollision(m_colFeet, col, false);
					Physics2D.IgnoreCollision(m_colBody, col, false);
				}

			}
		}
	}
}
