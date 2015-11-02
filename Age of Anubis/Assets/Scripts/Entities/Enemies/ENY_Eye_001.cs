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
    private bool m_isFacingRight = true;
    private bool m_readyToShoot = false;

    public override void AwakeOverride()
    {
        base.AwakeOverride();
    }

    public override void EnemyBehaviour()
    {
        
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
