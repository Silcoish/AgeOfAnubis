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

	private bool m_isAttacking = false;

    void Awake()
    {
		m_col = gameObject.GetComponent<PolygonCollider2D>();
        m_col.enabled = false;
    }

	void Update()
	{
        if(transform.parent.parent.GetComponent<Animator>().GetCurrentAnimatorStateInfo(1).IsName("Attack(Light)")
            || transform.parent.parent.GetComponent<Animator>().GetCurrentAnimatorStateInfo(1).IsName("Attack(Medium)")
            || transform.parent.parent.GetComponent<Animator>().GetCurrentAnimatorStateInfo(1).IsName("Attack(Heavy)"))
        {
            m_isAttacking = true;
            m_col.enabled = true;
        }
        else
        {
            m_isAttacking = false;
            m_col.enabled = false;
        }
	}

    public void Attack(Animator anim)
    {
        if (!m_isAttacking)
		{
			anim.SetInteger("AttackType", (int)m_swingType);
            anim.SetTrigger("Attack");
		} 
    }

	void OnTriggerEnter2D(Collider2D col)
	{
		Damageable damagable = col.GetComponent<Damageable>();

		if (damagable != null)
		{
			damagable.OnTakeDamage(m_attack.GetDamage(transform));


			switch (m_swingType)
			{
				case WeaponSwing.LIGHT:
					print("Light");
					AudioManager.Inst.PlaySFX(AudioManager.Inst.a_cut);
					break;
				case WeaponSwing.MEDIUM:
					print("Medium");
					AudioManager.Inst.PlaySFX(AudioManager.Inst.a_giveDamage);
					break;
				case WeaponSwing.HEAVY:
					print("Heavy");
					AudioManager.Inst.PlaySFX(AudioManager.Inst.a_thump);
					break;
			}

			Enemy en = col.gameObject.GetComponent<Enemy>();
			if (en != null)
			{


			}
		}
	}    
}
