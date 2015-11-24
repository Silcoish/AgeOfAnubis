using UnityEngine;
using System.Collections;

public class HealthPotion : MonoBehaviour 
{
    public int m_healthValue = 2;
    public float m_animationFrequency = 5F;
    private float m_curTimer = 0;
    private Animator m_anim;

    void Awake()
    {
        m_anim = GetComponent<Animator>();
    }

    void Update()
    {
        if (m_curTimer <= 0)
        {
            m_anim.SetTrigger("Animate");
            m_curTimer = m_animationFrequency;
        }
        else
            m_curTimer -= Time.deltaTime;
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.tag == "Player")
        {
            col.gameObject.GetComponent<Damageable>().RecoverHealth(GameManager.inst.player.GetComponent<Player>().m_maxHitpoints / 100 * m_healthValue);
            AudioManager.Inst.PlaySFX(AudioManager.Inst.a_healthPot);
			if(LastRunStats.inst != null)
			{
				LastRunStats.inst.hpPickups++;
			}
            Destroy(gameObject);
        }
    }
}
