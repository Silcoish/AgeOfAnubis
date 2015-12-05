using UnityEngine;
using System.Collections;

public class PROJ_Laser : MonoBehaviour 
{
    public Attack m_attack;
    public float m_duration = 1;
    private float m_timer = 0;
    private bool m_isFacingRight = true;
    private Vector3 m_startScale;
    public float m_finalScale = 1;

    void Awake()
    {
        m_startScale = transform.localScale;
    }

	void Update () 
    {
	    // expand the scale of the x and y over the duration
        if (m_timer >= m_duration)
            Destroy(gameObject);
        else
        {
            Vector3 scale = new Vector3(0, 0, 0);
            scale.y = Mathf.Lerp(m_startScale.y, m_finalScale, m_timer);
            if(m_isFacingRight)
                scale.x = Mathf.Lerp(m_startScale.x, m_finalScale, m_timer);
            else
                scale.x = Mathf.Lerp(m_startScale.x, -m_finalScale, m_timer);

            transform.localScale = scale;
        }

        m_timer += Time.deltaTime;
	}

    public void SetFacing(float dir)
    {
        if(dir < 0)
        {
            m_isFacingRight = false;
            transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
        }
            
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.tag == "Player")
        {
            col.gameObject.GetComponent<Damageable>().OnTakeDamage(m_attack.GetDamage(gameObject.transform));
        }

    }
}
