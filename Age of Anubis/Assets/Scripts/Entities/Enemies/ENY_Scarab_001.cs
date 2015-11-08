using UnityEngine;
using System.Collections;

public class ENY_Scarab_001 : PhysicsEnemy 
{
	[Header("ScarabStats")]
	public float m_moveSpeed = 10f;

	public override void EnemyBehaviour()
	{
        if (m_isPathing)
            m_rb.velocity = Vector2.zero;

        if (!m_isPathing)
        {
            // Check if we have hit the ground
            if (m_rb.velocity.y == 0)
            {
                // Reenable climbing once we stop falling
                EnablePathing(true);
                GetLedgeTransform();
            }
        }
        else
        {
            // Check if we hit a wall
            if (CheckPosition(m_wallCheck))
            {
                //Debug.Log("Hit Wall");
                m_ledgeCheck = GetTransform(m_ledgeCheck, !m_isVertical);
                m_wallCheck = GetTransform(m_ledgeCheck, m_isVertical);
            }
            // Check if we hit the end of a platform
            else if (!CheckPosition(m_ledgeCheck))
            {
                //Debug.Log("Hit Ledge");
                // Reverse Direction when we hit the end of our ledge
                m_ledgeCheck = GetTransform(m_ledgeCheck, !m_isVertical);
                m_wallCheck = GetTransform(m_ledgeCheck, m_isVertical);
            }
            // Move forward
            else
            {
                //Debug.Log("Moving");
                if (m_isVertical)
                {
                    m_dir = new Vector2(0, m_ledgeCheck.localPosition.y);
                }
                else
                {
                    m_dir = new Vector2(m_ledgeCheck.localPosition.x, 0);
                }
                m_dir.Normalize();

                m_rb.velocity = m_dir * m_moveSpeed;
            }
        }
	}
}
