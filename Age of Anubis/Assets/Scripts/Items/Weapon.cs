using UnityEngine;
using System.Collections;

public enum WeaponSwing {LIGHT, MEDIUM, HEAVY}

public class Weapon : MonoBehaviour 
{
	public Attack m_attack;
	private PolygonCollider2D m_col;

	public WeaponSwing m_swingType;
	public float m_cooldown = 0.5f;
	public float m_colliderEnableDelay = 0.1f;
	public float m_colliderEnableTime = 0.3f;

	private float m_timer = 0;
	private bool m_isAttacking = false;


    void Awake()
    {
		m_col = gameObject.GetComponent<PolygonCollider2D>();
    }

	void Update()
	{
		if (m_isAttacking)
		{
			m_timer += Time.deltaTime;

			
			if (m_timer >= m_colliderEnableDelay)
			{
				m_col.enabled = true;
			}
			if (m_timer >= m_colliderEnableTime)
			{
				m_col.enabled = false;
			}
			if (m_timer >= m_cooldown)
			{
				m_isAttacking = false;
			}
		}

	}

    public void Attack(Animator anim)
    {
        if (!m_isAttacking)
		{
			anim.SetTrigger((int)m_swingType);
			m_isAttacking = true;
			m_timer = 0;
		} 
    }

	void OnTriggerEnter2D(Collider2D col)
	{
		Damageable damagable = col.GetComponent<Damageable>();

		if (damagable != null)
		{
			damagable.OnTakeDamage(m_attack.GetDamage(transform));
		}
	}

    
}
