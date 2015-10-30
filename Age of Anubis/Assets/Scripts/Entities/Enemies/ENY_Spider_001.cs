using UnityEngine;
using System.Collections;

public enum Facing { UP, DOWN, LEFT, RIGHT}
public enum CollisionState { NONE, LEDGE, WALL}

public class ENY_Spider_001 : Enemy 
{
    [Header("SpiderStats")]
    public float m_moveSpeed = 2f;

    public float m_gravityScale;
    public float m_ledgeCheckDist;

    public Facing m_facing = Facing.RIGHT;

    public Vector2 m_forward;
    public Vector2 m_below;
    public bool m_isInverted = false; // true if on ceiling/right side of wall
    public CollisionState m_colState = CollisionState.NONE;

    public override void AwakeOverride()
    {
        base.AwakeOverride();

        if (Random.value >= 0.5F)
        {
            m_facing = Facing.RIGHT;
        }
        else
        {
            m_facing = Facing.LEFT;
        }
        m_isInverted = false;
        ChangeDirectionVectors();

        m_ledgeCheckDist = GetComponent<CircleCollider2D>().radius + 0.1F;
        m_gravityScale = Physics2D.gravity.y;

    }

    public override void EnemyBehaviour()
    {
        if(CheckPlatformInRange(transform.position, m_forward, m_ledgeCheckDist))
        {
            // Wall in front
            Debug.Log("Wall in front");
            m_colState = CollisionState.WALL;
        }
        else if(CheckPlatformInRange(transform.position, m_below, m_ledgeCheckDist))
        {
            // Moving off of ledge
            Debug.Log("Ledge reached");
            m_colState = CollisionState.LEDGE;
        }
        else
        {
            m_colState = CollisionState.NONE;
        }

        // HERE WE GO BOYS!
        switch(m_colState)
        {
            case CollisionState.WALL:
                {
                    switch(m_facing)
                    {
                        case Facing.UP:
                            {
                                if(m_isInverted)
                                {
                                    m_facing = Facing.RIGHT;
                                    m_isInverted = true;
                                }
                                else
                                {
                                    m_facing = Facing.LEFT;
                                    m_isInverted = true;
                                }
                            }
                            break;
                        case Facing.DOWN:
                            {
                                if (m_isInverted)
                                {
                                    m_facing = Facing.LEFT;
                                    m_isInverted = false;
                                }
                                else
                                {
                                    m_facing = Facing.RIGHT;
                                    m_isInverted = false;
                                }
                            }
                            break;
                        case Facing.LEFT:
                            {
                                if (m_isInverted)
                                {
                                    m_facing = Facing.DOWN;
                                    m_isInverted = true;
                                }
                                else
                                {
                                    m_facing = Facing.UP;
                                    m_isInverted = true;
                                }
                            }
                            break;
                        case Facing.RIGHT:
                            {
                                if (m_isInverted)
                                {
                                    m_facing = Facing.DOWN;
                                    m_isInverted = false;
                                }
                                else
                                {
                                    m_facing = Facing.UP;
                                    m_isInverted = false;
                                }
                            }
                            break;
                    }
                }
                ChangeDirectionVectors();
                break;
            case CollisionState.LEDGE:
                {
                    switch (m_facing)
                    {
                        case Facing.UP:
                            {
                                if (m_isInverted)
                                {
                                    m_facing = Facing.LEFT;
                                    m_isInverted = false;
                                }
                                else
                                {
                                    m_facing = Facing.RIGHT;
                                    m_isInverted = false;
                                }
                            }
                            break;
                        case Facing.DOWN:
                            {
                                if (m_isInverted)
                                {
                                    m_facing = Facing.LEFT;
                                    m_isInverted = true;
                                }
                                else
                                {
                                    m_facing = Facing.RIGHT;
                                    m_isInverted = true;
                                }
                            }
                            break;
                        case Facing.LEFT:
                            {
                                if (m_isInverted)
                                {
                                    m_facing = Facing.UP;
                                    m_isInverted = false;
                                }
                                else
                                {
                                    m_facing = Facing.DOWN;
                                    m_isInverted = false;
                                }
                            }
                            break;
                        case Facing.RIGHT:
                            {
                                if (m_isInverted)
                                {
                                    m_facing = Facing.UP;
                                    m_isInverted = true;
                                }
                                else
                                {
                                    m_facing = Facing.DOWN;
                                    m_isInverted = true;
                                }
                            }
                            break;
                    }
                }
                ChangeDirectionVectors();
                break;
            default:
                // Haven't reached wall or ledge, keep going.
                break;
        }

        m_rb.velocity = (m_forward * m_moveSpeed) + (m_below * Mathf.Abs(m_gravityScale));

        Debug.DrawRay(transform.position, m_forward, Color.yellow);
        Debug.DrawRay(transform.position, m_below, Color.red);
    }

    void ChangeDirectionVectors()
    {
        switch(m_facing)
        {
            case Facing.UP:
                m_forward = new Vector2(0, 1);
                m_below = m_isInverted ? new Vector2(-1, 0) : new Vector2(1, 0);
                break;
            case Facing.DOWN:
                m_forward = new Vector2(0, -1);
                m_below = m_isInverted ? new Vector2(-1, 0) : new Vector2(1, 0);
                break;
            case Facing.LEFT:
                m_forward = new Vector2(-1, 0);
                m_below = m_isInverted ? new Vector2(0, 1) : new Vector2(0, -1);
                break;
            case Facing.RIGHT:
                m_forward = new Vector2(1, 0);
                m_below = m_isInverted ? new Vector2(0, 1) : new Vector2(0, -1);
                break;
        }
    }
}
