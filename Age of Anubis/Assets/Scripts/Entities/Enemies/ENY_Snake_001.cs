using UnityEngine;
using System.Collections;

public class ENY_Snake_001 : PhysicsEnemy
{
    [Header("SnakeStats")]
    public float m_moveSpeed = 2f;

    public float m_attackTimer = 5;
    public float m_chargeTimer = 0.5F;
    public float m_rechargeTimer = 0.5F;
    private float m_curTimer = 0;

    public GameObject m_venomProj;
    public Vector2 m_launchVec;
    private float m_launchX;
    public float m_launchForce;

    enum State { SEARCHING, CHARGING, ATTACKING, RECHARGING };
    private State m_state = State.SEARCHING;

    public override void AwakeOverride()
    {
        base.AwakeOverride();

        m_curTimer = m_attackTimer + (Random.value * 2); // Add between 0.1-2 seconds to make the shooting less uniform

        m_launchX = m_launchVec.x;
    }

    public override void EnemyBehaviour()
    {
        if(m_isPathing)
            m_rb.velocity = Vector2.zero;

        switch (m_state)
        {
            case State.SEARCHING:
                if(!m_isPathing)
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

                        if (!CheckPosition(m_ledgeCheck))
                        {
                            EnablePathing(false);
                        }
                    }
                    // Check if we hit the end of a platform
                    else if (!CheckPosition(m_ledgeCheck))
                    {
                        //Debug.Log("Hit Ledge");
                        // Reverse Direction when we hit the end of our ledge
                        m_ledgeCheck = GetTransform(m_ledgeCheck, !m_isVertical);
                        m_wallCheck = GetTransform(m_ledgeCheck, m_isVertical);

                        // If our new ledge choice is also over a ledge then enable gravity
                        if (!CheckPosition(m_ledgeCheck))
                        {
                            EnablePathing(false);
                        }
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
                
                if(m_curTimer <= 0)
                {
                    m_state = State.CHARGING;
                    m_curTimer = m_chargeTimer;
                    Debug.Log("Enter Charge State");
                }
                break;
            case State.CHARGING:
                if (m_curTimer <= 0)
                {
                    m_state = State.ATTACKING;
                    Debug.Log("Enter Attack State");
                }
                break;
            case State.ATTACKING:
                GameObject tempProj;
                tempProj = Instantiate(m_venomProj, transform.position, transform.rotation) as GameObject;
                Physics2D.IgnoreCollision(tempProj.GetComponent<Collider2D>(), GetComponent<Collider2D>());
                tempProj.GetComponent<Rigidbody2D>().AddForce(m_launchVec * m_launchForce, ForceMode2D.Impulse);

                m_state = State.RECHARGING;
                m_curTimer = m_rechargeTimer;
                Debug.Log("Enter Recharge State");
                break;
            case State.RECHARGING:
                if (m_curTimer <= 0)
                {
                    m_state = State.SEARCHING;
                    m_curTimer = m_attackTimer;
                    Debug.Log("Enter Searching State");
                }
                break;
            default:
                Debug.Log("Snake enemy failed to select valid State");
                break;
        }
        m_curTimer -= Time.deltaTime;

        // Animation
        //UpdateAnimationState(m_rb.velocity.x);
    }
}
