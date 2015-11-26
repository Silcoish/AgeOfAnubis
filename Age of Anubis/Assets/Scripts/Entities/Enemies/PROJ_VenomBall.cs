using UnityEngine;
using System.Collections;

public class PROJ_VenomBall : MonoBehaviour 
{
    public Attack m_attack;
    public float m_fallLimit = -5;
    private int m_penetration = 3;
    private int m_curPen = 0;

    private Rigidbody2D m_rb2D;

    void Awake()
    {
        m_rb2D = GetComponent<Rigidbody2D>();
    }

	void Update () 
    {
        // Clamp falling speed
        if (m_rb2D.velocity.y < m_fallLimit)
            m_rb2D.velocity = new Vector2(m_rb2D.velocity.x, m_fallLimit);
	}

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.tag == "Player")
        {
            col.gameObject.GetComponent<Damageable>().OnTakeDamage(m_attack.GetDamage(gameObject.transform));
            Destroy(gameObject);
        }

        if(col.gameObject.tag == "Solid" || col.gameObject.tag == "NotSolid")
        {
            m_curPen++;
        }

        if(m_curPen >= m_penetration)
        {
            Destroy(gameObject);
        }
    }
}
