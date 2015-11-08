using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PhysicsEnemy : Enemy 
{
    public Transform m_topRight;
    public Transform m_topLeft;
    public Transform m_botRight;
    public Transform m_botLeft;
    protected Transform m_ledgeCheck;
    protected Transform m_wallCheck;

    public bool m_canClimb = false;
    protected bool m_isPathing = true;
    protected bool m_isVertical = false;
    protected Vector2 m_dir;
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

        GetLedgeTransform();
    }

    public override void OnTakeDamage(Damage dam)
    {
        base.OnTakeDamage(dam);

        EnablePathing(false);
    }

    protected bool CheckPosition(Transform pos)
    {
        return Physics2D.OverlapCircle(pos.position, m_checkRadius, m_platformMask);
    }

    protected void EnablePathing(bool state)
    {
        if (state)
        {
            m_isPathing = true;
            m_rb.gravityScale = 0;
            //Debug.Log("Climbing - true");
        }
        else
        {
            m_isPathing = false;
            m_rb.gravityScale = 1;
            //Debug.Log("Climbing - false");
        }
    }

    // Useful function for getting the opposite transforms
    // if you pass m_isVertical it will get the wall of your current ledge transform
    // if you pass the opposite of m_isVertical it will get the reverse direction of your current ledge transform
    protected Transform GetTransform(Transform ledge, bool isVertical)
    {
        if (ledge == m_topRight)
        {
            if (isVertical)
                return m_topLeft;
            else
                return m_botRight;
        }
        else if (ledge == m_topLeft)
        {
            if (isVertical)
                return m_topRight;
            else
                return m_botLeft;
        }
        else if (ledge == m_botRight)
        {
            if (isVertical)
                return m_botLeft;
            else
                return m_topRight;
        }
        else
        {
            if (isVertical)
                return m_botRight;
            else
                return m_topLeft;
        }
    }

    protected void GetLedgeTransform()
    {
        // There should always be 2 colliders active when become active (ie the platform we are standing on)
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

        if (m_canClimb && ledges.Count >= 2)
        {
            if (Random.value > 0.5)
                m_ledgeCheck = ledges[0];
            else
                m_ledgeCheck = ledges[1];

            if (ledges[0].position.x == ledges[1].position.x)
                m_isVertical = true;
            else
                m_isVertical = false;

            m_wallCheck = GetTransform(m_ledgeCheck, m_isVertical);
            EnablePathing(true);
            //Debug.Log("Ledges check: " + ledges[0] + ledges[1]);
            //Debug.Log("Ledge: " + m_ledgeCheck + ", Wall: " + m_wallCheck + ", Vertical: " + m_isVertical);
        }
        else if(!m_canClimb)
        {
            if (Random.value > 0.5F)
                m_ledgeCheck = m_botRight;
            else
                m_ledgeCheck = m_botLeft;

            m_wallCheck = GetTransform(m_ledgeCheck, m_isVertical);
            EnablePathing(true);
            //Debug.Log("Ledges check: " + ledges[0] + ledges[1]);
            //Debug.Log("Ledge: " + m_ledgeCheck + ", Wall: " + m_wallCheck + ", Vertical: " + m_isVertical);
        }
        else
        {
            // No ledges were in range, we should fall
            EnablePathing(false);
        }
    }
}
