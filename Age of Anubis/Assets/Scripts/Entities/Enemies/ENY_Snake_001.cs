using UnityEngine;
using System.Collections;

public class ENY_Snake_001 : Enemy 
{
    [Header("SnakeStats")]
    public float m_moveSpeed = 2f;
    private float m_dir;
    public float m_ledgeCheckDist = 0.5F;

    public float m_attackTimer = 5;
    private float m_curTimer = 0;
    public GameObject m_venomProj;
    public Vector2 m_launchVec;
    private float m_launchX;
    public float m_launchForce;
    public float m_shootDelay = 0.5F;
    private bool m_isFacingRight = true;
    private bool m_readyToShoot = false;

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

        m_curTimer = m_attackTimer + (Random.value * 2); // Add between 0.1-2 seconds to make the shooting less uniform

        m_launchX = m_launchVec.x;
    }

    public override void EnemyBehaviour()
    {
        if (m_readyToShoot)
        {
            GameObject tempProj;
            tempProj = Instantiate(m_venomProj, transform.position, transform.rotation) as GameObject;
            Physics2D.IgnoreCollision(tempProj.GetComponent<Collider2D>(), GetComponent<Collider2D>());
            tempProj.GetComponent<Rigidbody2D>().AddForce(m_launchVec * m_launchForce, ForceMode2D.Impulse);
            m_curTimer = m_attackTimer;
            m_readyToShoot = false;
        }

        if(m_curTimer > 0)
        {
            m_curTimer -= Time.deltaTime;
        }
        else
        {
            m_readyToShoot = true;
            Pause(m_shootDelay);
        }

        // Check if near edge of platfrom or a wall and reverse direction
        if (m_dir > 0)
        {
            if (!CheckPlatformInRange(new Vector2(transform.position.x + m_ledgeCheckDist, transform.position.y), Vector2.down, 0.5F)
                || CheckPlatformInRange(transform.position, Vector2.right, m_ledgeCheckDist))
            {
                m_dir = -m_moveSpeed;
                m_launchVec = new Vector2(-m_launchX, m_launchVec.y);
            }
        }
        else
        {
            if (!CheckPlatformInRange(new Vector2(transform.position.x - m_ledgeCheckDist, transform.position.y), Vector2.down, 0.5F)
                || CheckPlatformInRange(transform.position, Vector2.left, m_ledgeCheckDist))
            {
                m_dir = m_moveSpeed;
                m_launchVec = new Vector2(m_launchX, m_launchVec.y);
            }
        }
        UpdateAnimationState(m_dir);

        m_rb.velocity = new Vector2(m_dir, m_rb.velocity.y);
    }

    // Flip the character object for left/right facing.
    void Flip()
    {
        m_isFacingRight = !m_isFacingRight;
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }

    // Update character animations based on Input.
    void UpdateAnimationState(float input)
    {
        if (input > 0 && !m_isFacingRight)
            Flip();
        else if (input < 0 && m_isFacingRight)
            Flip();
    }
}
