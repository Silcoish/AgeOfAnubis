using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ENY_Spider_001 : PhysicsEnemy 
{
    [Header("SpiderStats")]
    public float m_moveSpeed = 2f;

    public override void EnemyBehaviour()
    {
        if(!m_isPathing)
        {
            // Check if we have hit the ground
            if(m_rb.velocity.y == 0)
            {
                // Reenable climbing once we stop falling
                EnablePathing(true);
                GetLedgeTransform();
            }
        }
        else
        {
            // Check if we hit a wall
            if(CheckPosition(m_wallCheck))
            {
                m_isVertical = !m_isVertical;
                m_ledgeCheck = m_wallCheck;
                m_wallCheck = GetTransform(m_ledgeCheck, m_isVertical);
            }
            // Check if we hit the end of a platform
            else if(!CheckPosition(m_ledgeCheck))
            {
                // TODO: Spider will wrap around ledges when they run off the end

                // Reverse Direction when we hit the end of our ledge
                m_ledgeCheck = GetTransform(m_ledgeCheck, !m_isVertical);
                m_wallCheck = GetTransform(m_ledgeCheck, m_isVertical);
            }
            // Move forward
            else
            {
                if(m_isVertical)
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
