using UnityEngine;
using System.Collections;

public class ENY_Scarab_001 : Enemy 
{
	[Header("ScarabStats")]
	public float m_moveSpeed = 10f;
	public float m_fakeGravity = 10f;

	private float m_localGravityScale;

	public override void AwakeOverride()
	{
		base.AwakeOverride();

		m_localGravityScale = m_rb.gravityScale;
	}


	public override void EnemyBehaviour()
	{

		/*
		CheckReturn checkreturn = CheckEnemyLocation();

		if (checkreturn.colBelow != null)
		{
			m_rb.gravityScale = 0;

			m_rb.velocity = transform.right * m_moveSpeed;

			m_rb.AddRelativeForce(Vector3.down * m_fakeGravity);
		}
		else
		{
			m_rb.gravityScale = m_localGravityScale;
		}

		if (checkreturn.type == CheckReturnEnum.ReachedWall || checkreturn.type == CheckReturnEnum.ReachedEdge)
		{
			transform.rotation = transform.rotation * Quaternion.FromToRotation(Vector2.left, Vector2.right);
		}*/


	}



}
