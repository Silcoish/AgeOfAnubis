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
    protected bool m_isDead = false;
    protected bool m_isDeathAnimation = false;
    private float m_deathTimer;
    private GameObject tempProj;

    private bool m_isFacingRight = true;
    private Transform m_player;

    private Animator m_anim;

    enum State { IDLE, SEARCHING, CHARGING, ATTACKING, RECHARGING, FALLING, DEATH };
    private State m_state = State.IDLE;

    public override void AwakeOverride()
    {
        base.AwakeOverride();

        m_player = GameObject.FindGameObjectWithTag("Player").transform;
        if (!m_player)
            Debug.Log("Eye - Failed to find GameObject with 'Player' tag");

        m_anim = GetComponent<Animator>();
    }

    public override void OnDeath()
    {
        if(!m_isDead)
        {
            //Disable colliders and activate timer so death animation can play before turning off
            m_rb.gravityScale = 1;
            gameObject.layer = LayerMask.NameToLayer("NoCollide");
            m_col.isTrigger = false;
            m_isDead = true;
            m_state = State.FALLING;

            if (m_deathParticle)
                Instantiate(m_deathParticle, transform.position, transform.rotation);

            m_rb.velocity = new Vector2(0, -0.1F);

            if (LastRunStats.inst != null)
            {
                LastRunStats.inst.enemiesKilled++;
            }
        }
    }

    protected void AfterDeath()
    {
        if (m_room)
            m_room.EnemyDied(this);
        bool spawnedPotion = false;
        if (GameManager.inst.healthPotionPrefab)
        {
            if (GameManager.inst.CheckForHPDrop())
            {
                Instantiate(GameManager.inst.healthPotionPrefab, transform.position, transform.rotation);
                spawnedPotion = true;
            }
        }
        if (GameManager.inst.coinPrefab && !spawnedPotion)
            Instantiate(GameManager.inst.coinPrefab, transform.position, transform.rotation);
        PlayerInventory.Inst.ChangeXP(m_XP);

        gameObject.SetActive(false);
    }

    public override void EnemyBehaviour()
    {
        m_anim.SetInteger("State", (int)m_state);

        Vector2 dir = m_player.position - transform.position;

        switch (m_state)
        {
            case State.IDLE:
                m_rb.velocity = Vector2.zero;
                m_isTracking = (Mathf.Abs(dir.x) < m_trackingRange && Mathf.Abs(dir.y) < m_trackingRange) ? true : false;
                if (m_isTracking)
                {
                    m_state = State.SEARCHING;
                }
                break;
            case State.SEARCHING:
                m_rb.velocity = Vector2.zero;
                m_isTracking = (Mathf.Abs(dir.x) < m_trackingRange && Mathf.Abs(dir.y) < m_trackingRange) ? true : false;
                if (m_isTracking)
                {
                    if (Mathf.Abs(dir.x) < m_xDist)
                    {
                        dir.x = 0;
                    }
                    m_rb.velocity = dir.normalized * m_moveSpeed;
                }
                else
                {
                    m_state = State.IDLE;
                }

                if (Mathf.Abs(dir.x) < (m_xDist + 0.5) && Mathf.Abs(dir.y) < 0.2)
                {
                    m_state = State.CHARGING;
                    m_curTimer = m_chargeTimer;
                    //Debug.Log("Enter Charge State");
                }
                break;
            case State.CHARGING:
                m_rb.velocity = Vector2.zero;
                if (m_curTimer <= 0)
                {
                    m_state = State.ATTACKING;
                    if (Vector2.Distance(GameManager.inst.player.transform.position, transform.position) <= 10.0f)
                        AudioManager.Inst.PlaySFX(AudioManager.Inst.a_eny_eye_fire);
                    //Debug.Log("Enter Attack State");
                    m_curTimer = 1;
                    tempProj = Instantiate(m_laserProj, new Vector3(transform.position.x, transform.position.y + 0.1F, 0), transform.rotation) as GameObject;
                    tempProj.transform.parent = transform;
                }
                break;
            case State.ATTACKING:
                m_rb.velocity = Vector2.zero;
                if (m_curTimer <= 0)
                {
                    m_state = State.RECHARGING;
                    m_curTimer = m_rechargeTimer;
                }
                break;
            case State.RECHARGING:
                m_rb.velocity = Vector2.zero;
                if (m_curTimer <= 0)
                {
                    m_state = State.SEARCHING;
                    //Debug.Log("Enter Searching State");
                }
                break;
            case State.FALLING:
                if (m_rb.velocity.y >= 0)
                {
                    m_curTimer = 0.5F;
                    m_state = State.DEATH;
                }
                break;
            case State.DEATH:
                m_rb.velocity = Vector2.zero;
                if (m_curTimer <= 0)
                {
                    AfterDeath();
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
		
	}
}
