using UnityEngine;
using System.Collections;

public class ENY_Eye_001 : Enemy
{
    [Header("EyeStats")]
    public float m_moveSpeed = 2f;

    public float m_trackingRange = 10;
    public float m_xDist = 2;
    public float m_chargeTimer = 0.5F;
    public float m_rechargeTimer = 3;
    private float m_curTimer = 0;
    private bool m_isTracking = false;
    public GameObject m_laserProj;
    
    private bool m_isFacingRight = true;
    private Transform m_player;

    private Animator m_anim;

    enum State { SEARCHING, CHARGING, ATTACKING, RECHARGING };
    private State m_state = State.SEARCHING;

    public override void AwakeOverride()
    {
        base.AwakeOverride();

        m_player = GameObject.FindGameObjectWithTag("Player").transform;
        if (!m_player)
            Debug.Log("Eye - Failed to find GameObject with 'Player' tag");

        m_anim = GetComponent<Animator>();
    }

    public override void EnemyBehaviour()
    {
        Vector2 dir = m_player.position - transform.position;
        m_rb.velocity = Vector2.zero;

        switch(m_state)
        {
            case State.SEARCHING:
                m_isTracking = (Mathf.Abs(dir.x) < m_trackingRange && Mathf.Abs(dir.y) < m_trackingRange) ? true : false;
                if (m_isTracking)
                {
                    if (Mathf.Abs(dir.x) < m_xDist)
                    {
                        dir.x = 0;
                    }
                    m_rb.velocity = dir.normalized * m_moveSpeed;
                }

                if (Mathf.Abs(dir.x) < (m_xDist + 0.5) && Mathf.Abs(dir.y) < 0.2)
                {
                    m_state = State.CHARGING;
                    m_anim.SetTrigger("Blink");
                    m_curTimer = m_chargeTimer;
                    //Debug.Log("Enter Charge State");
                }
                break;
            case State.CHARGING:
                if (m_curTimer <= 0)
                {
                    m_state = State.ATTACKING;
					AudioManager.Inst.PlaySFX(AudioManager.Inst.a_eny_eye_fire);
                    //Debug.Log("Enter Attack State");
                }
                break;
            case State.ATTACKING:
                GameObject tempProj;
                tempProj = Instantiate(m_laserProj, transform.position, transform.rotation) as GameObject;
                tempProj.GetComponent<PROJ_Laser>().SetFacing(dir.x);
                tempProj.transform.parent = transform;

                m_state = State.RECHARGING;
                m_curTimer = m_rechargeTimer;
                //Debug.Log("Enter Recharge State");
                break;
            case State.RECHARGING:
                if (m_curTimer <= 0)
                {
                    m_state = State.SEARCHING;
                    //Debug.Log("Enter Searching State");
                }
                break;
            default:
                Debug.Log("Eye enemy failed to select valid State");
                break;
        }
        m_curTimer -= Time.deltaTime;

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

	public void PlayBlinkSFX()
	{
		AudioManager.Inst.PlaySFX(AudioManager.Inst.a_eny_eye_blink);
	}
}
