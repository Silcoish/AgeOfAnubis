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

    private CircleCollider2D m_physCol;

    public GameObject m_spriteObject;
    public Animator m_anim;
    protected bool m_isDead = false;
    public bool m_spriteFaceRight = true;

    public override void AwakeOverride()
    {
        base.AwakeOverride();

        // Reset rotation NOTE: Enemies inheriting from PhysicsEnemy will NOT work once rotated
        transform.rotation = Quaternion.Euler(Vector3.zero);

        Animator[] tempAnims = gameObject.GetComponentsInChildren<Animator>();
        if(tempAnims.Length > 0)
        {
            m_anim = tempAnims[0];
            m_spriteObject = m_anim.transform.parent.gameObject;
        }
        else
            Debug.Log("PhysicsEnemy - " + gameObject.name + " failed to find Animator on child object.");

        CircleCollider2D[] tempCols = gameObject.GetComponentsInChildren<CircleCollider2D>();
        if(tempCols.Length > 0)
        {
            foreach(var c in tempCols)
            {
                if(c.name == "PhysicsCollider")
                {
                    m_physCol = c;
                }
            }
        }
        else
            Debug.Log("PhysicsEnemy - " + gameObject.name + " failed to find CircleCollider2D on child object.");

        // Set up check transforms
        Vector2 pos = m_physCol.transform.localPosition;
        float offset = m_physCol.radius + m_checkOffset;
        //Debug.Log(gameObject.name + " pos = " + pos + " offset = " + offset);
        m_topRight.localPosition = new Vector2(pos.x + offset, pos.y + offset);
        m_topLeft.localPosition = new Vector2(pos.x + -offset, pos.y + offset);
        m_botRight.localPosition = new Vector2(pos.x + offset, pos.y + -offset);
        m_botLeft.localPosition = new Vector2(pos.x + -offset, pos.y + -offset);

        GetLedgeTransform();
    }

    public override void OnTakeDamage(Damage dam)
    {
        base.OnTakeDamage(dam);

        m_anim.SetTrigger("TakeDamage");
        EnablePathing(false);
    }

    protected bool CheckPosition(Transform pos)
    {
        return Physics2D.OverlapCircle(pos.position, m_checkRadius, m_platformMask);
    }

    public override void OnDeath()
    {
        if (m_room)
            m_room.EnemyDied(this);
        if (m_deathParticle)
            Instantiate(m_deathParticle, transform.position, transform.rotation);
        if (GameManager.inst.coinPrefab)
            Instantiate(GameManager.inst.coinPrefab, transform.position, transform.rotation);
        if (GameManager.inst.healthPotionPrefab)
        {
            if (Random.value <= GameManager.inst.hpDropChance)
                Instantiate(GameManager.inst.healthPotionPrefab, transform.position, transform.rotation);
        }
        PlayerInventory.Inst.ChangeXP(m_XP);

        //Disable colliders and activate timer so death animation can play before turning off
        m_anim.SetTrigger("Death");
        m_col.enabled = false;
        m_isDead = true;
    }

    protected void AfterDeath()
    {
        gameObject.SetActive(false);
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
        else if (!m_canClimb && ledges.Count >= 2)
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

    // Fixes the facing in sprite manipulation if the sprite sheet faces the wrong direction
    bool FacingRight(bool isFacingRight)
    {
        if (isFacingRight && m_spriteFaceRight || !isFacingRight && !m_spriteFaceRight)
            return true;
        else
            return false;
    }

    // Flip the character object for left/right facing.
    void SetSpriteTransform(Quaternion direction, bool isFacingRight)
    {
        m_spriteObject.transform.rotation = direction;

        if(FacingRight(isFacingRight))
        {
            Vector3 scale = m_spriteObject.transform.localScale;
            scale.x = 1;
            m_spriteObject.transform.localScale = scale;
        }
        else
        {
            Vector3 scale = m_spriteObject.transform.localScale;
            scale.x = -1;
            m_spriteObject.transform.localScale = scale;
        }
    }

    // Update player animations based on Input.
    protected void UpdateAnimationState()
    {
        Quaternion up = Quaternion.Euler(0, 0, 180);
        Quaternion down = Quaternion.Euler(0, 0, 0);
        Quaternion right = Quaternion.Euler(0, 0, 90);
        Quaternion left = Quaternion.Euler(0, 0, -90);

        if (m_ledgeCheck == m_topRight)
        {
            if (m_isVertical)
                SetSpriteTransform(right, true);
            else
                SetSpriteTransform(up, false);
        }
        else if (m_ledgeCheck == m_topLeft)
        {
            if (m_isVertical)
                SetSpriteTransform(left, false);
            else
                SetSpriteTransform(up, true);
        }
        else if (m_ledgeCheck == m_botRight)
        {
            if (m_isVertical)
                SetSpriteTransform(right, false);
            else
                SetSpriteTransform(down, true);
        }
        else
        {
            if (m_isVertical)
                SetSpriteTransform(left, true);
            else
                SetSpriteTransform(down, false);
        }
    }
}
