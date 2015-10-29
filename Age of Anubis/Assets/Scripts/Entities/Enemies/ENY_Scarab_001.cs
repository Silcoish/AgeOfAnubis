using UnityEngine;
using System.Collections;

public class ENY_Scarab_001 : Enemy 
{
	[Header("ScarabStats")]
	public float m_moveSpeed = 10f;
	public float m_fakeGravity = 10f;

	private float m_localGravityScale;
    private float m_dir;
    public float m_ledgeCheckDist = 1;

	public override void AwakeOverride()
	{
		base.AwakeOverride();

        if (Random.value >= 0.5F)
        {
            m_dir = m_moveSpeed;
        }
        else
        {
            m_dir = -m_moveSpeed;
        }

		//m_localGravityScale = m_rb.gravityScale;
	}

	public override void EnemyBehaviour()
	{
        // Check if near edge of platfrom and reverse direction
        if(m_dir > 0)
        {
            if(!CheckFloor(transform.position.x + m_ledgeCheckDist, 1))
            {
                m_dir = -m_moveSpeed;
            }
        }
        else
        {
            if (!CheckFloor(transform.position.x - m_ledgeCheckDist, 1))
            {
                m_dir = m_moveSpeed;
            }
        }

        m_rb.velocity = new Vector2(m_dir, m_rb.velocity.y);



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
