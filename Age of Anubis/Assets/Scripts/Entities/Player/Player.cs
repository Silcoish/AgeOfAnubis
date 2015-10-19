using UnityEngine;
using System.Collections;

public class Player : Damageable 
{
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
	private Vector2 m_velocity = Vector2.zero;

	public Animator m_anim;
    public Animator m_anim_arm;
    private bool isFacingRight = true;

	private Vector2 m_inputAxis = Vector2.zero;

	public Weapon m_currentWeapon;

	public override void AwakeOverride() 
    {
		m_colFeet = GetComponent<CircleCollider2D>();
		m_colBody = GetComponent<BoxCollider2D>();

		m_anim = GetComponent<Animator>();
        
        Animator[] tempAnims = gameObject.GetComponentsInChildren<Animator>();
        foreach(var d in tempAnims)
        {
            if(d.name == "Player_Arm")
            {
                m_anim_arm = d;
            }
        }
	}

	public override void UpdateOverride()
    {
		CheckIfGrounded();

		Check2WayPlatforms();

		PlayerInput();	
		
    }

	///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	
	void PlayerInput()
	{
		//Set velocity to current to maintain any current velocity if not effected by the player.
		m_velocity = m_rb.velocity;

		m_inputAxis.x = Input.GetAxisRaw("Horizontal");
		m_inputAxis.y = Input.GetAxisRaw("Vertical");

		m_velocity.x = m_inputAxis.x * m_moveSpeed * m_globalMoveSpeed;

        // Update player animation state
		UpdateAnimationState(m_inputAxis.x);
		SetAnimSpeed(m_inputAxis.x);
        SetAnimVSpeed(m_velocity.y);

		if (Input.GetButtonDown("Jump"))
		{
			Jump();
		}


		if (Input.GetButtonUp("Jump"))
		{
			JumpStop();
		}

		if (Input.GetButtonDown("Fire1"))
		{
			if (m_currentWeapon != null)
				m_currentWeapon.Attack(m_anim);
		}
		if (Input.GetButtonDown("Fire2"))
		{
			m_currentWeapon = PlayerInventory.Inst.SwitchWeapon();
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
			print("Conter > 0");
			m_velocity.y = m_jumpHeight * m_globalMoveSpeed;
			m_jumpCounter--;

            SetAnimGrounded(false); // Tell the animator we are airborne
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
					//Check if Player is above obect
					if (m_colFeet.bounds.min.y >= hit.collider.bounds.max.y)
					{
						m_jumpCounter = m_jumpMax;
                        SetAnimGrounded(true); // Tell the animator we have landed
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
		Vector2 offsetDistance = new Vector2(2, 2);

		Collider2D[] allCols = Physics2D.OverlapAreaAll((Vector2)m_colFeet.bounds.min - offsetDistance, (Vector2)m_colFeet.bounds.max + offsetDistance);

		foreach (var col in allCols)
		{
			if (col.CompareTag("NotSolid"))
			{
				if (m_colFeet.bounds.min.y < col.bounds.max.y)
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
        if (input > 0 && !isFacingRight)
            Flip();
        else if (input < 0 && isFacingRight)
            Flip();
    }
    
    // Set the Speed value of the animators
    void SetAnimSpeed(float speed)
    {
        m_anim.SetFloat("Speed", Mathf.Abs(speed));
        m_anim_arm.SetFloat("Speed", Mathf.Abs(speed));
    }

    // Set the Grounded value of the animators
    void SetAnimGrounded(bool isGrounded)
    {
        m_anim.SetBool("Grounded", isGrounded);
        m_anim_arm.SetBool("Grounded", isGrounded);
    }

    void SetAnimVSpeed(float vSpeed)
    {
        m_anim.SetFloat("VSpeed", Mathf.Abs(vSpeed));
        m_anim_arm.SetFloat("VSpeed", Mathf.Abs(vSpeed));
    }
}
