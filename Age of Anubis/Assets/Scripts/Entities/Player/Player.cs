using UnityEngine;
using System.Collections;

public class Player : Damageable 
{

    public static Player Inst;
	[Header("PlayerStats")]
    public float m_moveSpeed;
    public float m_jumpHeight;
    public int m_jumpNumber;
	public int m_jumpMax;
	public float m_groundedCheckDistance;

	private CircleCollider2D m_colFeet;
	private BoxCollider2D m_colBody;
    private bool m_isJumping;
	public int m_jumpCounter;
    public float m_minJumpHeight = 1;
    private float m_jumpStart;
    private bool m_stopJump = false;
	private Vector2 m_velocity = Vector2.zero;

	public Animator m_anim;
    public Animator m_anim_arm;
    public Animator m_anim_legs;
    private bool isFacingRight = true;

	private Vector2 m_inputAxis = Vector2.zero;

    public GameObject m_playerHand;
	public GameObject m_currentWeapon;

    public bool m_isShopOpen = false;
    public float m_immunityTimer = 1;
    private float m_curImmTimer = 0;

    private bool m_isDying = false;
	private bool m_isDead = false;
    public float m_deathDelay = 5;
    private float m_deathDelayTimer = 0;

    public GameObject m_jumpCloud;

    //public float jumpMaxSpeed;
    //public float jumpMinSpeed;

	public override void AwakeOverride() 
    {
        Inst = this;
		m_colFeet = GetComponent<CircleCollider2D>();
		m_colBody = GetComponent<BoxCollider2D>();

		m_anim = GetComponent<Animator>();
        
        // Get the animator on the player arm
        Animator[] tempAnims = gameObject.GetComponentsInChildren<Animator>();
        foreach(var d in tempAnims)
        {
            if(d.name == "Player_Arm")
            {
                m_anim_arm = d;
            }
            else if(d.name == "Player_Legs")
            {
                m_anim_legs = d;
            }
        }

        // Get the players hand
        m_playerHand = m_anim_arm.transform.FindChild("Player_Hand").gameObject;
	}

    public override void StartOverride()
    {
        // Equip current weapon
        UpdateEquippedWeapon(PlayerInventory.Inst.m_currentWeapon);

		if (LastRunStats.inst != null)
		{
			LastRunStats.inst.startGold = PlayerInventory.Inst.m_currentGold;
		}
    }

	public override void OnTakeDamage(Damage dam)
	{
        if(m_curImmTimer < 0 && !m_isDying)
        {
            base.OnTakeDamage(dam);

            SetAnimTrigger("TakeDamage");
            
            m_curImmTimer = m_immunityTimer;

            UIManager.Inst.UpdateHealthBar(((float)m_hitPoints / (float)m_maxHitpoints));

			AudioManager.Inst.PlaySFX(AudioManager.Inst.a_player_takeDamage);
        }
	}

	public override void UpdateOverride()
    {
        if(m_isDying)
        {
            m_rb.velocity = new Vector2(0, 0);
            if(m_deathDelayTimer <= 0)
                DeathReset();
            else
                m_deathDelayTimer -= Time.deltaTime;
        }
        else
        {
            CheckIfGrounded();

            Check2WayPlatforms();

            PlayerInput();

            m_curImmTimer -= Time.deltaTime;

            // WARNING: Damage over time effects do not call OnTakeDamage() each tick, so health does not get updated.
            UIManager.Inst.UpdateHealthBar(((float)m_hitPoints / (float)m_maxHitpoints));
        }

        Damage d = new Damage();
        d.amount = 20;
        d.type = DamageType.NONE;

        if (Input.GetKeyDown(KeyCode.G))
            OnTakeDamage(d);

        if (Input.GetKeyDown(KeyCode.F))
            PlayerInventory.Inst.ChangeXP(20);

        if (Input.GetKeyDown(KeyCode.C))
            PlayerInventory.Inst.ChangeGold(20);
    }

    public override void OnDeath()
    {
        if(!m_isDying)
        {
            Debug.Log("OnDeath Called");

            SetAnimTrigger("Death");
            m_isDying = true;
            m_deathDelayTimer = m_deathDelay;

			if (LastRunStats.inst != null)
			{
				LastRunStats.inst.endGold = PlayerInventory.Inst.m_currentGold;
				LastRunStats.inst.died = true;
			}

			AudioManager.Inst.PlaySFX(AudioManager.Inst.a_player_death);
			RecoverHealth(0);
        } 
    }

    void DeathReset()
    {
		if(!m_isDead)
		{
			m_isDead = true;
			PlayerInventory.Inst.DeathReset();
			// Load shop level.
			LoadingManager.Inst.LoadLevel("ShopScene", false);
		}

    }

	public void LeaveDungeon()
	{
        LoadingManager.Inst.LoadLevel("ShopScene", false);
	}

    public override void RecoverHealth(int amount)
    {
        base.RecoverHealth(amount);

        UIManager.Inst.UpdateHealthBar(((float)m_hitPoints / (float)m_maxHitpoints));
    }

	///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	
	void PlayerInput()
	{
		//Set velocity to current to maintain any current velocity if not effected by the player.
		m_velocity = m_rb.velocity;

        if(!m_isShopOpen)
        {
            m_inputAxis.x = Input.GetAxisRaw("Horizontal");
            m_inputAxis.y = Input.GetAxisRaw("Vertical");
        }
        else
        {
            m_inputAxis.x = 0;
            m_inputAxis.y = 0;
        }

		m_velocity.x = m_inputAxis.x * m_moveSpeed * m_globalMoveSpeed;

        // Update player animation state
		UpdateAnimationState(m_inputAxis.x);
		SetAnimSpeed(m_inputAxis.x);
        SetAnimVSpeed(m_velocity.y);
        //if (m_velocity.y > jumpMaxSpeed)
        //    jumpMaxSpeed = m_velocity.y;
        //if (m_velocity.y < jumpMinSpeed)
        //    jumpMinSpeed = m_velocity.y;


		if (Input.GetKeyDown(KeyCode.K))
		{
			RecoverHealth(-1000000);
		}

		if (Input.GetKeyDown(KeyCode.L))
		{
			PlayerInventory.Inst.m_currentWeapon = WeaponManager.inst.GenerateWeapon(5);
			UpdateEquippedWeapon(PlayerInventory.Inst.m_currentWeapon);
			m_hitPoints = 250;
			RecoverHealth(10000);
		}

        if(Input.GetKeyDown(KeyCode.J))
        {
            PlayerInventory.Inst.ChangeGold(1000000);
        }

        if(!m_isShopOpen)
        {
            if (Input.GetButtonDown("Jump"))
            {
                //Debug.Log("Jump key down");
				
                Jump();
            }

            if (Input.GetButtonUp("Jump"))
            {
                //Debug.Log("Jump key up");
                m_stopJump = true;
            }

            if (m_stopJump && transform.position.y > m_jumpStart + m_minJumpHeight)
            {
                //Debug.Log("jumpstop called");
                JumpStop();
                m_stopJump = false;
            }

            if (Input.GetButtonDown("Fire1"))
            {
                if (m_currentWeapon != null)
                {
                    m_currentWeapon.GetComponent<Weapon>().Attack(m_anim);
                    m_currentWeapon.GetComponent<Weapon>().Attack(m_anim_arm);
                    m_currentWeapon.GetComponent<Weapon>().Attack(m_anim_legs);
                }
            }
        }
		
		if (Input.GetButtonDown("Fire2"))
		{
            if(!isInAttackAnimation())
                UpdateEquippedWeapon(PlayerInventory.Inst.SwitchWeapon());
		}

		if (m_inputAxis.y < -0.7 && Mathf.Abs(m_inputAxis.y) > Mathf.Abs(m_inputAxis.x))
		{
			RaycastHit2D[] hit = Physics2D.CircleCastAll(transform.position, m_colFeet.radius, Vector2.down, 2);

			foreach (var h in hit)
			{
				if (h.collider.gameObject.tag == "NotSolid")
				{
					Physics2D.IgnoreCollision(h.collider, m_colBody, true);
					Physics2D.IgnoreCollision(h.collider, m_colFeet, true);
				}
			}
		}

		m_rb.velocity = m_velocity;
	}

	void Jump()
	{
		if (m_jumpCounter > 0)
		{
            if(!m_anim.GetBool("Grounded"))
            {
                m_velocity.y = (m_jumpHeight * 0.8F) * m_globalMoveSpeed;
                m_stopJump = false;

                if (m_jumpCloud != null)
                {
                    Instantiate(m_jumpCloud, new Vector3(transform.position.x, transform.position.y - 0.8F, 0), transform.rotation);
                }
            }
            else
            {
                m_velocity.y = m_jumpHeight * m_globalMoveSpeed;
                m_jumpStart = transform.position.y;
            }
			m_jumpCounter--;

            SetAnimGrounded(false); // Tell the animator we are airborne

			if (Random.Range(0, 2) == 0)
				AudioManager.Inst.PlaySFX(AudioManager.Inst.a_player_jump1);
			else
				AudioManager.Inst.PlaySFX(AudioManager.Inst.a_player_jump2);

		}
	}

	void JumpStop()
	{
		if (m_velocity.y > 0)
		{
			m_velocity.y = 0;
		}

	}

	void CheckIfGrounded()
	{
		if (m_rb.velocity.y <= 0)
		{
			RaycastHit2D[] allHits = Physics2D.CircleCastAll((Vector2)(transform.position) + (Vector2)m_colFeet.offset, m_colFeet.radius, Vector2.down, m_groundedCheckDistance);

			foreach (var hit in allHits)
			{
				if (hit.collider.CompareTag("NotSolid") || hit.collider.CompareTag("Solid") || hit.collider.CompareTag("Wall"))
				{
					//Check if Player is above object
					if (m_colFeet.bounds.min.y >= hit.collider.bounds.max.y)
					{
						m_jumpCounter = m_jumpMax;
                        SetAnimGrounded(true); // Tell the animator we have landed
                        m_stopJump = false;

						//AudioManager.Inst.PlaySFX(AudioManager.Inst.a_player_land);
					}
				}
                else
                {
                    SetAnimGrounded(false); // Either we are still airborne from jumping, or are falling off a platform
                }
			}
		}
	}

	void Check2WayPlatforms()
	{
		Vector2 offsetDistance = new Vector2(2, 2F);

		Collider2D[] allCols = Physics2D.OverlapAreaAll((Vector2)m_colFeet.bounds.min - offsetDistance, (Vector2)m_colFeet.bounds.max + offsetDistance);

		foreach (var col in allCols)
		{
			if (col.CompareTag("NotSolid"))
			{
				if (m_colFeet.bounds.min.y < col.bounds.max.y - 0.05F)
				{
					Physics2D.IgnoreCollision(m_colFeet, col, true);
					Physics2D.IgnoreCollision(m_colBody, col, true);
				}
				else
				{
					Physics2D.IgnoreCollision(m_colFeet, col, false);
					Physics2D.IgnoreCollision(m_colBody, col, false);
				}

			}
		}
	}

    // Updates the weapon object equipped to the player.
    public void UpdateEquippedWeapon(GameObject newWep)
    {
        if (m_currentWeapon)
        {
            Destroy(m_currentWeapon);
        }

        m_currentWeapon = Instantiate(newWep, m_playerHand.transform.position, m_playerHand.transform.rotation) as GameObject;
        m_currentWeapon.transform.parent = m_playerHand.transform;
        m_currentWeapon.transform.localScale = new Vector3(1, 1, 1);
    }

    // Flip the character object for left/right facing.
    void Flip()
    {
        isFacingRight = !isFacingRight;
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }

    // Update player animations based on Input.
    void UpdateAnimationState(float input)
    {
        if(!isAttacking())
        {
            if (input > 0 && !isFacingRight)
                Flip();
            else if (input < 0 && isFacingRight)
                Flip();
        }
    }
    
    // Set the Speed value of the animators
    void SetAnimSpeed(float speed)
    {
        m_anim.SetFloat("Speed", Mathf.Abs(speed));
        m_anim_arm.SetFloat("Speed", Mathf.Abs(speed));
        m_anim_legs.SetFloat("Speed", Mathf.Abs(speed));
    }

    // Set the Grounded value of the animators
    void SetAnimGrounded(bool isGrounded)
    {
        m_anim.SetBool("Grounded", isGrounded);
        m_anim_arm.SetBool("Grounded", isGrounded);
        m_anim_legs.SetBool("Grounded", isGrounded);
    }

    // Set the vertical speed value of the animators
    void SetAnimVSpeed(float vSpeed)
    {
        m_anim.SetFloat("VSpeed", vSpeed);
        m_anim_arm.SetFloat("VSpeed", vSpeed);
        m_anim_legs.SetFloat("VSpeed", vSpeed);
    }

    // Set off an animation trigger with name triggerName
    void SetAnimTrigger(string triggerName)
    {
        m_anim.SetTrigger(triggerName);
        m_anim_arm.SetTrigger(triggerName);
        m_anim_legs.SetTrigger(triggerName);
    }

    public bool isAttacking()
    {
        if (m_anim_arm.GetCurrentAnimatorStateInfo(2).IsName("Attack(Light)2")
            || m_anim_arm.GetCurrentAnimatorStateInfo(2).IsName("Attack(Medium)2")
            || m_anim_arm.GetCurrentAnimatorStateInfo(2).IsName("Attack(Heavy)2"))
            return true;
        else
            return false;
    }

    public bool isInAttackAnimation()
    {
        if (m_anim_arm.GetCurrentAnimatorStateInfo(2).IsName("Attack(Light)1")
            || m_anim_arm.GetCurrentAnimatorStateInfo(2).IsName("Attack(Light)2")
            || m_anim_arm.GetCurrentAnimatorStateInfo(2).IsName("Attack(Medium)1")
            || m_anim_arm.GetCurrentAnimatorStateInfo(2).IsName("Attack(Medium)2")
            || m_anim_arm.GetCurrentAnimatorStateInfo(2).IsName("Attack(Medium)3")
            || m_anim_arm.GetCurrentAnimatorStateInfo(2).IsName("Attack(Heavy)1")
            || m_anim_arm.GetCurrentAnimatorStateInfo(2).IsName("Attack(Heavy)2")
            || m_anim_arm.GetCurrentAnimatorStateInfo(2).IsName("Attack(Heavy)3"))
            return true;
        else
            return false;
    }
}
