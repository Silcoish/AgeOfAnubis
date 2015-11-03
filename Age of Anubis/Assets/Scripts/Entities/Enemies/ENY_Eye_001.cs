using UnityEngine;
using System.Collections;

public class ENY_Eye_001 : Enemy
{
    [Header("EyeStats")]
    public float m_moveSpeed = 2f;
    public float m_attackTimer = 5;
    private float m_curTimer = 0;
    public GameObject m_laserProj;
    public float m_shootDelay = 0.5F;
    public float m_recharge = 3;
    private float m_rechargeTimer = 0;
    private bool m_isFacingRight = true;
    private bool m_readyToShoot = false;
    private Transform m_player;
    private bool m_isTracking = false;
    public float m_trackingRange = 10;
    public float m_xDist = 2;

    public override void AwakeOverride()
    {
        base.AwakeOverride();

        m_player = GameObject.FindGameObjectWithTag("Player").transform;
        if (!m_player)
            Debug.Log("Eye - Failed to find GameObject with 'Player' tag");
    }

    public override void EnemyBehaviour()
    {
        Vector2 dir = m_player.position - transform.position;

        // Attacking
        if(m_readyToShoot && m_rechargeTimer >= m_recharge)
        {
            Debug.Log("Attacking");
            GameObject tempProj;
            tempProj = Instantiate(m_laserProj, transform.position, transform.rotation) as GameObject;
            tempProj.GetComponent<PROJ_Laser>().SetFacing(dir.x);
            m_readyToShoot = false;
            m_rechargeTimer = 0;
            Pause(m_shootDelay);
        }
        else if (Mathf.Abs(dir.x) < (m_xDist + 0.5) && Mathf.Abs(dir.y) < 1)
        {
            Debug.Log("Ready to Shoot");
            m_readyToShoot = true;
            Pause(m_shootDelay);
        }
        m_rechargeTimer += Time.deltaTime;
        //Debug.Log(m_rechargeTimer);
        Debug.Log(m_readyToShoot);
        Debug.Log(dir);

        // Movement
        m_isTracking = (Mathf.Abs(dir.x) < m_trackingRange && Mathf.Abs(dir.y) < m_trackingRange) ? true : false;
        if(m_isTracking)
        {
            if(Mathf.Abs(dir.x) < m_xDist)
            {
                dir.x = 0;
            }
            m_rb.velocity = dir.normalized * m_moveSpeed;
        }
        else
            m_rb.velocity = Vector2.zero;

        // Animation
        UpdateAnimationState(m_rb.velocity.x);
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
