using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ENY_Spider_001 : Enemy 
{
    [Header("SpiderStats")]
    public float m_moveSpeed = 2f;

    public Transform m_topRight;
    public Transform m_topLeft;
    public Transform m_botRight;
    public Transform m_botLeft;
    private Transform m_ledgeCheck;
    private Transform m_wallCheck;

    private bool m_isClimbing = true;
    private bool m_isVertical = true;
    private Vector2 m_dir;
    public float m_checkOffset = 0.1F;
    public float m_checkRadius = 0.1F;
    public LayerMask m_platformMask;


    public override void AwakeOverride()
    {
        base.AwakeOverride();

        // Set up check transforms
        float offset = GetComponent<CircleCollider2D>().radius + m_checkOffset;
        m_topRight.localPosition = new Vector2(offset, offset);
        m_topLeft.localPosition = new Vector2(-offset, offset);
        m_botRight.localPosition = new Vector2(offset, -offset);
        m_botLeft.localPosition = new Vector2(-offset, -offset);

        // There should always be 2 colliders active when we awake (ie the platform we are standing on)
        // Pick one of those 2 as the ledge check, which also determines our initial direction and wall check.
        List<Transform> ledges = new List<Transform>();

        if (CheckPosition(m_topRight))
            ledges.Add(m_topRight);
        if (CheckPosition(m_topLeft))
            ledges.Add(m_topLeft);
        if (CheckPosition(m_botRight))
            ledges.Add(m_botRight);
        if (CheckPosition(m_botLeft))
            ledges.Add(m_botLeft);

        if(ledges.Count >= 2)
        {
            if (Random.value > 0.5)
                m_ledgeCheck = ledges[0];
            else
                m_ledgeCheck = ledges[1];

            if (ledges[0].position.x == ledges[1].position.x)
                m_isVertical = true;
            else
                m_isVertical = false;

            m_wallCheck = GetWallTransform(m_ledgeCheck);
            EnableClimbing(true);
        }
        else
        {
            // No ledges were in range, we should fall
            EnableClimbing(false);
        }
    }

    public override void OnTakeDamage(Damage dam)
    {
        base.OnTakeDamage(dam);

        EnableClimbing(false);
    }

    public override void EnemyBehaviour()
    {
        if(!m_isClimbing)
        {
            // Check if we have hit the ground
            if(m_rb.velocity.y == 0)
            {
                // Reenable climbing once we stop falling
                EnableClimbing(true);
            }
        }
        else
        {
            // 
        }
    }

    bool CheckPosition(Transform pos)
    {
        return Physics2D.OverlapCircle(pos.position, m_checkRadius, m_platformMask);
    }

    void EnableClimbing(bool state)
    {
        if(state)
        {
            m_isClimbing = true;
            m_rb.gravityScale = 0;
            Debug.Log("Climbing - true");
        }
        else
        {
            m_isClimbing = false;
            m_rb.gravityScale = 1;
            Debug.Log("Climbing - false");
        }
    }

    Transform GetWallTransform(Transform ledge)
    {
        if(ledge == m_topRight)
        {
            if (m_isVertical)
                return m_topLeft;
            else
                return m_botRight;
        }
        else if(ledge == m_topLeft)
        {
            if (m_isVertical)
                return m_topRight;
            else
                return m_botLeft;
        }
        else if(ledge == m_botRight)
        {
            if (m_isVertical)
                return m_botLeft;
            else
                return m_topRight;
        }
        else
        {
            if (m_isVertical)
                return m_botRight;
            else
                return m_topLeft;
        }
    }
}
